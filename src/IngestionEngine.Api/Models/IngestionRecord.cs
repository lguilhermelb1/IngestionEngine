namespace IngestionEngine.Api.Models;

public class IngestionRecord
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

    public IngestionRecord() { }

    public IngestionRecord(string documentNumber, string name, decimal amount)
    {
        DocumentNumber = documentNumber;
        Name = name;
        Amount = amount;
        ProcessedAt = DateTime.UtcNow;
    }
}