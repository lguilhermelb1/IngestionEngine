namespace IngestionEngine.Api.Common;

public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; } = new();

    public static ValidationResult Success() => new();

    public static ValidationResult Failure(IEnumerable<string> errors)
    {
        var result = new ValidationResult();
        result.Errors.AddRange(errors);
        return result;
    }

    public static ValidationResult Failure(string error) => Failure(new[] { error });
}