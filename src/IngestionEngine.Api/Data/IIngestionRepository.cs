using IngestionEngine.Api.Models;

namespace IngestionEngine.Api.Data;

public interface IIngestionRepository
{
    Task InsertSingleAsync(IngestionRecord record);
    Task InsertBatchBulkAsync(IEnumerable<IngestionRecord> records);
    Task<IEnumerable<IngestionRecord>> GetAllAsync();
}