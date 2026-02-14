//==================================================================================================
// Unit tests for core configuration validation features.
// Comprehensive tests for fluent validation API and rule execution.
//==================================================================================================
using FluentAssertions;
using Fox.ConfigKit.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ConfigKit.Tests;

//==============================================================================================
/// <summary>
/// Tests for basic validation rules (NotEmpty, NotNull, GreaterThan, etc.).
/// </summary>
//==============================================================================================
public sealed class ValidationTests
{
    //==========================================================================================
    /// <summary>
    /// Test options class for validation scenarios.
    /// </summary>
    //==========================================================================================
    private sealed class TestOptions
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Timeout { get; set; }
        public int? OptionalValue { get; set; }
        public string? Pattern { get; set; }
    }

    [Fact]
    public void NotEmpty_should_fail_when_string_is_null()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestOptions>(services, "Test");
        builder.NotEmpty(o => o.Name, "Name is required");

        var options = new TestOptions { Name = null };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("Name is required");
    }

    [Fact]
    public void NotEmpty_should_fail_when_string_is_empty()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestOptions>(services, "Test");
        builder.NotEmpty(o => o.Name, "Name is required");

        var options = new TestOptions { Name = string.Empty };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void NotEmpty_should_pass_when_string_has_value()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestOptions>(services, "Test");
        builder.NotEmpty(o => o.Name);

        var options = new TestOptions { Name = "TestName" };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void NotNull_should_fail_when_value_is_null()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestOptions>(services, "Test");
        builder.NotNull(o => o.OptionalValue, "Value is required");

        var options = new TestOptions { OptionalValue = null };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void NotNull_should_pass_when_value_is_not_null()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestOptions>(services, "Test");
        builder.NotNull(o => o.OptionalValue);

        var options = new TestOptions { OptionalValue = 42 };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void GreaterThan_should_fail_when_value_is_too_small()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestOptions>(services, "Test");
        builder.GreaterThan(o => o.Timeout, 0, "Timeout must be positive");

        var options = new TestOptions { Timeout = 0 };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void GreaterThan_should_pass_when_value_is_greater()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestOptions>(services, "Test");
        builder.GreaterThan(o => o.Timeout, 0);

        var options = new TestOptions { Timeout = 30 };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void LessThan_should_fail_when_value_is_too_large()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestOptions>(services, "Test");
        builder.LessThan(o => o.Timeout, 100, "Timeout must be less than 100");

        var options = new TestOptions { Timeout = 100 };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void InRange_should_fail_when_value_is_out_of_range()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestOptions>(services, "Test");
        builder.InRange(o => o.Timeout, 1, 300);

        var options = new TestOptions { Timeout = 500 };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void InRange_should_pass_when_value_is_within_range()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestOptions>(services, "Test");
        builder.InRange(o => o.Timeout, 1, 300);

        var options = new TestOptions { Timeout = 150 };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void MatchesPattern_should_fail_when_value_does_not_match_regex()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestOptions>(services, "Test");
        builder.MatchesPattern(o => o.Pattern, @"^[a-zA-Z]+$", "Pattern must be letters only");

        var options = new TestOptions { Pattern = "invalid123" };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void MatchesPattern_should_pass_when_value_matches_regex()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestOptions>(services, "Test");
        builder.MatchesPattern(o => o.Pattern, @"^[a-zA-Z]+$");

        var options = new TestOptions { Pattern = "ValidPattern" };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Multiple_validations_should_all_be_executed()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestOptions>(services, "Test");
        builder
            .NotEmpty(o => o.Name)
            .GreaterThan(o => o.Timeout, 0)
            .LessThan(o => o.Timeout, 100);

        var options = new TestOptions
        {
            Name = "Test",
            Timeout = 30
        };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }
}
