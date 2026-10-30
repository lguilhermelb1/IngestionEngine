using IngestionEngine.Api.Common;
using IngestionEngine.Api.Models;

namespace IngestionEngine.Api.Services;

public interface IIngestionValidator
{
    ValidationResult Validate(IngestionRecord record);
}