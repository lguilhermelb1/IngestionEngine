using IngestionEngine.Api.Models;
using IngestionEngine.Api.Services;
using Xunit;

namespace IngestionEngine.Tests;

public class IngestionValidatorTests
{
	private readonly IngestionValidator _validator = new();

	[Fact]
	public void Validate_ValidRecord_ShouldReturnSuccess()
	{

		var record = new IngestionRecord("12345678901", "João Silva", 150.50m);

		var result = _validator.Validate(record);

		Assert.True(result.IsValid);
		Assert.Empty(result.Errors);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData("12345")]
	[InlineData("123456789012")]
	[InlineData("1234567890A")]
	public void Validate_InvalidDocumentNumber_ShouldReturnFailure(string invalidDoc)
	{

		var record = new IngestionRecord(invalidDoc, "João Silva", 100m);

		var result = _validator.Validate(record);

		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.Contains("DocumentNumber"));
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-0.01)]
	[InlineData(-500)]
	public void Validate_AmountZeroOrNegative_ShouldReturnFailure(decimal invalidAmount)
	{

		var record = new IngestionRecord("12345678901", "João Silva", invalidAmount);

		var result = _validator.Validate(record);

		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.Contains("Amount"));
	}
}