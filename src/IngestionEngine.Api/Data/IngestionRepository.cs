using System.Data;
using Dapper;
using IngestionEngine.Api.Models;
using Microsoft.Data.SqlClient;

namespace IngestionEngine.Api.Data;

public class IngestionRepository : IIngestionRepository
{
    private readonly string _connectionString;

    public IngestionRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");
    }

    public async Task InsertSingleAsync(IngestionRecord record)
    {
        const string sql = @"
            INSERT INTO IngestionRecords (DocumentNumber, Name, Amount, ProcessedAt)
            VALUES (@DocumentNumber, @Name, @Amount, @ProcessedAt);";

        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(sql, record);
    }

    public async Task InsertBatchBulkAsync(IEnumerable<IngestionRecord> records)
    {
        var recordList = records.ToList();
        if (recordList.Count == 0) return;

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

        try
        {
            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction)
            {
                DestinationTableName = "dbo.IngestionRecords",
                BatchSize = 1000,
                BulkCopyTimeout = 60
            };

            bulkCopy.ColumnMappings.Add(nameof(IngestionRecord.DocumentNumber), "DocumentNumber");
            bulkCopy.ColumnMappings.Add(nameof(IngestionRecord.Name), "Name");
            bulkCopy.ColumnMappings.Add(nameof(IngestionRecord.Amount), "Amount");
            bulkCopy.ColumnMappings.Add(nameof(IngestionRecord.ProcessedAt), "ProcessedAt");

            using var dataTable = new DataTable();
            dataTable.Columns.Add("DocumentNumber", typeof(string));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Amount", typeof(decimal));
            dataTable.Columns.Add("ProcessedAt", typeof(DateTime));

            foreach (var item in recordList)
            {
                dataTable.Rows.Add(item.DocumentNumber, item.Name, item.Amount, item.ProcessedAt);
            }

            await bulkCopy.WriteToServerAsync(dataTable);

            await transaction.CommitAsync();
        }
        catch
        {

            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<IngestionRecord>> GetAllAsync()
    {
        const string sql = "SELECT DocumentNumber, Name, Amount, ProcessedAt FROM IngestionRecords ORDER BY ProcessedAt DESC;";
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<IngestionRecord>(sql);
    }
}