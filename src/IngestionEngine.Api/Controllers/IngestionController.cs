using IngestionEngine.Api.Data;
using IngestionEngine.Api.Models;
using IngestionEngine.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace IngestionEngine.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngestionController : ControllerBase
{
    private readonly IIngestionValidator _validator;
    private readonly IIngestionRepository _repository;

    public IngestionController(IIngestionValidator validator, IIngestionRepository repository)
    {
        _validator = validator;
        _repository = repository;
    }

    [HttpPost("single")]
    public async Task<IActionResult> IngestSingle([FromBody] IngestionRecord record)
    {
        if (record.ProcessedAt == default)
        {
            record.ProcessedAt = DateTime.UtcNow;
        }

        var validation = _validator.Validate(record);
        if (!validation.IsValid)
        {
            return BadRequest(new { success = false, errors = validation.Errors });
        }

        await _repository.InsertSingleAsync(record);
        return StatusCode(StatusCodes.Status201Created, new { success = true, data = record });
    }

    [HttpPost("batch")]
    public async Task<IActionResult> IngestBatch([FromBody] List<IngestionRecord> records)
    {
        if (records == null || records.Count == 0)
        {
            return BadRequest(new { success = false, message = "Lote não pode estar vazio." });
        }

        var invalidRecords = new List<object>();

        for (int i = 0; i < records.Count; i++)
        {
            if (records[i].ProcessedAt == default)
            {
                records[i].ProcessedAt = DateTime.UtcNow;
            }

            var validation = _validator.Validate(records[i]);
            if (!validation.IsValid)
            {
                invalidRecords.Add(new { Index = i, Errors = validation.Errors });
            }
        }

        if (invalidRecords.Count > 0)
        {
            return UnprocessableEntity(new
            {
                success = false,
                message = "Lote contém registros inválidos. Nenhuma alteração foi persistida.",
                invalidRecords
            });
        }

        await _repository.InsertBatchBulkAsync(records);
        return Ok(new { success = true, count = records.Count });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _repository.GetAllAsync();
        return Ok(data);
    }
}