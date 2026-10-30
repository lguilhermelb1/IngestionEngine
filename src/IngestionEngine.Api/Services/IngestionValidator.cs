using System.Text.RegularExpressions;
using IngestionEngine.Api.Common;
using IngestionEngine.Api.Models;

namespace IngestionEngine.Api.Services;

public partial class IngestionValidator : IIngestionValidator
{
    [GeneratedRegex(@"^\d{11}$")]
    private static partial Regex ApenasOnzeDigitosRegex();

    public ValidationResult Validate(IngestionRecord record)
    {
        var errors = new List<string>();

        if (record is null)
        {
            return ValidationResult.Failure("Registro não pode ser nulo.");
        }

        if (string.IsNullOrWhiteSpace(record.DocumentNumber))
        {
            errors.Add("DocumentNumber é obrigatório.");
        }
        else if (!ApenasOnzeDigitosRegex().IsMatch(record.DocumentNumber))
        {
            errors.Add("DocumentNumber deve conter exatamente 11 dígitos numéricos.");
        }

        if (record.Amount <= 0)
        {
            errors.Add("Amount deve ser maior que zero.");
        }

        if (string.IsNullOrWhiteSpace(record.Name))
        {
            errors.Add("Name é obrigatório.");
        }

        return errors.Count == 0
            ? ValidationResult.Success()
            : ValidationResult.Failure(errors);
    }
}