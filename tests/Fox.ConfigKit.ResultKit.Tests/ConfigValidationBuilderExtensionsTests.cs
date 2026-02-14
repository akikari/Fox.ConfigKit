//==================================================================================================
// Unit tests for ConfigValidationBuilder Result pattern extensions.
// Validates ToResult and ToValidationResult behavior with success and failure scenarios.
//==================================================================================================
using FluentAssertions;
using Fox.ConfigKit.Validation;
using Fox.ResultKit;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ConfigKit.ResultKit.Tests;

public sealed class ConfigValidationBuilderExtensionsTests
{
    private sealed class TestConfig
    {
        public string Name { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    [Fact]
    public void ToResult_should_return_success_when_validation_passes()
    {
        var config = new TestConfig { Name = "TestApp", Port = 8080, Email = "test@example.com" };
        var builder = new ConfigValidationBuilder<TestConfig>(new ServiceCollection(), "TestConfig")
            .NotEmpty(c => c.Name, "Name is required");

        var result = builder.ToResult(config);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(config);
    }

    [Fact]
    public void ToResult_should_return_failure_when_validation_fails()
    {
        var config = new TestConfig { Name = "", Port = 8080 };
        var builder = new ConfigValidationBuilder<TestConfig>(new ServiceCollection(), "TestConfig")
            .NotEmpty(c => c.Name, "Name is required");

        var result = builder.ToResult(config);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Name is required");
    }

    [Fact]
    public void ToResult_should_return_first_error_when_multiple_validations_fail()
    {
        var config = new TestConfig { Name = "", Port = -1, Email = "" };
        var builder = new ConfigValidationBuilder<TestConfig>(new ServiceCollection(), "TestConfig")
            .NotEmpty(c => c.Name, "Name is required")
            .InRange(c => c.Port, 1, 65535, "Port must be between 1 and 65535")
            .NotEmpty(c => c.Email, "Email is required");

        var result = builder.ToResult(config);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Name is required");
    }

    [Fact]
    public void ToErrorsResult_should_return_all_errors_when_multiple_validations_fail()
    {
        var config = new TestConfig { Name = "", Port = -1, Email = "" };
        var builder = new ConfigValidationBuilder<TestConfig>(new ServiceCollection(), "TestConfig")
            .NotEmpty(c => c.Name, "Name is required")
            .InRange(c => c.Port, 1, 65535, "Port must be between 1 and 65535")
            .NotEmpty(c => c.Email, "Email is required");

        var result = builder.ToErrorsResult(config);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().Contain(e => e.Contains("Name is required"));
        result.Errors.Should().Contain(e => e.Contains("Port must be between 1 and 65535"));
        result.Errors.Should().Contain(e => e.Contains("Email is required"));
    }

    [Fact]
    public void ToResult_should_generate_correct_error_codes()
    {
        var config = new TestConfig { Name = "" };
        var builder = new ConfigValidationBuilder<TestConfig>(new ServiceCollection(), "TestConfig")
            .NotEmpty(c => c.Name, "Name is required");

        var result = builder.ToResult(config);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().StartWith("VALIDATION_");
    }

    [Fact]
    public void ToValidationResult_should_return_success_when_validation_passes()
    {
        var config = new TestConfig { Name = "TestApp", Port = 8080 };
        var builder = new ConfigValidationBuilder<TestConfig>(new ServiceCollection(), "TestConfig")
            .NotEmpty(c => c.Name, "Name is required");

        var result = builder.ToValidationResult(config);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ToValidationResult_should_return_failure_with_single_error()
    {
        var config = new TestConfig { Name = "" };
        var builder = new ConfigValidationBuilder<TestConfig>(new ServiceCollection(), "TestConfig")
            .NotEmpty(c => c.Name, "Name is required");

        var result = builder.ToValidationResult(config);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Name is required");
    }

    [Fact]
    public void ToErrorsResult_should_return_failure_with_multiple_errors()
    {
        var config = new TestConfig { Name = "", Port = -1 };
        var builder = new ConfigValidationBuilder<TestConfig>(new ServiceCollection(), "TestConfig")
            .NotEmpty(c => c.Name, "Name is required")
            .InRange(c => c.Port, 1, 65535, "Port must be between 1 and 65535");

        var result = builder.ToErrorsResult(config);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void ToResult_should_work_with_conditional_validation()
    {
        var config = new TestConfig { Name = "TestApp", Port = 443, Email = "" };
        var builder = new ConfigValidationBuilder<TestConfig>(new ServiceCollection(), "TestConfig")
            .NotEmpty(c => c.Name, "Name is required")
            .When(c => c.Port == 443, b =>
            {
                b.NotEmpty(c => c.Email, "Email required when using HTTPS");
            });

        var result = builder.ToResult(config);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Email required when using HTTPS");
    }

    [Fact]
    public void ToResult_should_chain_with_bind()
    {
        var config = new TestConfig { Name = "TestApp", Port = 8080 };
        var builder = new ConfigValidationBuilder<TestConfig>(new ServiceCollection(), "TestConfig")
            .NotEmpty(c => c.Name, "Name is required");

        var result = builder.ToResult(config)
            .Bind(cfg => Result<string>.Success($"Validated: {cfg.Name}"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("Validated: TestApp");
    }
}
