//==================================================================================================
// Unit tests for ConfigValidator static factory and standalone validation.
// Validates functional-style configuration validation without dependency injection.
//==================================================================================================
using FluentAssertions;
using Fox.ConfigKit.Validation;
using Fox.ResultKit;

namespace Fox.ConfigKit.ResultKit.Tests;

public sealed class ConfigValidatorTests
{
    private sealed class DatabaseConfig
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int MaxPoolSize { get; set; }
        public int CommandTimeoutSeconds { get; set; }
    }

    private sealed class ApiConfig
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; }
    }

    [Fact]
    public void Validate_should_create_builder_for_config_type()
    {
        var builder = ConfigValidator.Validate<DatabaseConfig>();

        builder.Should().NotBeNull();
    }

    [Fact]
    public void Validate_should_use_type_name_as_default_section()
    {
        var config = new DatabaseConfig { ConnectionString = "" };

        var result = ConfigValidator.Validate<DatabaseConfig>()
            .NotEmpty(c => c.ConnectionString, "Connection string is required")
            .ToResult(config);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Validate_should_use_custom_section_name_when_provided()
    {
        var builder = ConfigValidator.Validate<DatabaseConfig>("CustomDatabase");

        builder.Should().NotBeNull();
    }

    [Fact]
    public void Validate_should_chain_validation_rules()
    {
        var config = new DatabaseConfig
        {
            ConnectionString = "",
            MaxPoolSize = -1,
            CommandTimeoutSeconds = 700
        };

        var result = ConfigValidator.Validate<DatabaseConfig>()
            .NotEmpty(c => c.ConnectionString, "Connection string is required")
            .InRange(c => c.MaxPoolSize, 1, 1000, "Max pool size must be between 1 and 1000")
            .InRange(c => c.CommandTimeoutSeconds, 1, 600, "Timeout must be between 1 and 600 seconds")
            .ToErrorsResult(config);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().HaveCount(3);
    }

    [Fact]
    public void Validate_should_return_success_for_valid_config()
    {
        var config = new DatabaseConfig
        {
            ConnectionString = "Server=localhost;Database=test",
            MaxPoolSize = 100,
            CommandTimeoutSeconds = 30
        };

        var result = ConfigValidator.Validate<DatabaseConfig>()
            .NotEmpty(c => c.ConnectionString, "Connection string is required")
            .InRange(c => c.MaxPoolSize, 1, 1000, "Max pool size must be between 1 and 1000")
            .InRange(c => c.CommandTimeoutSeconds, 1, 600, "Timeout must be between 1 and 600 seconds")
            .ToResult(config);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(config);
    }

    [Fact]
    public void Validate_should_support_conditional_validation()
    {
        var config = new ApiConfig
        {
            BaseUrl = "https://api.example.com",
            ApiKey = "",
            TimeoutSeconds = 30
        };

        var result = ConfigValidator.Validate<ApiConfig>()
            .NotEmpty(c => c.BaseUrl, "Base URL is required")
            .When(c => c.BaseUrl.StartsWith("https"), b =>
            {
                b.NotEmpty(c => c.ApiKey, "API key required for HTTPS");
            })
            .ToResult(config);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("API key required for HTTPS");
    }

    [Fact]
    public void Validate_should_compose_with_result_bind()
    {
        var config = new DatabaseConfig
        {
            ConnectionString = "Server=localhost;Database=test",
            MaxPoolSize = 100,
            CommandTimeoutSeconds = 30
        };

        var result = ConfigValidator.Validate<DatabaseConfig>()
            .NotEmpty(c => c.ConnectionString, "Connection string is required")
            .ToResult(config)
            .Bind(cfg => Fox.ResultKit.Result<string>.Success($"DB: {cfg.ConnectionString}"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().StartWith("DB: Server=");
    }

    [Fact]
    public void Validate_should_stop_at_first_error_with_bind()
    {
        var config1 = new DatabaseConfig { ConnectionString = "" };
        var config2 = new ApiConfig { BaseUrl = "https://api.example.com" };

        var result = ConfigValidator.Validate<DatabaseConfig>()
            .NotEmpty(c => c.ConnectionString, "Connection string is required")
            .ToValidationResult(config1)
            .Bind(() => ConfigValidator.Validate<ApiConfig>()
                .NotEmpty(c => c.ApiKey, "API key required")
                .ToValidationResult(config2));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Connection string is required");
    }

    [Fact]
    public void Validate_should_work_with_match_pattern()
    {
        var validConfig = new DatabaseConfig
        {
            ConnectionString = "Server=localhost",
            MaxPoolSize = 100,
            CommandTimeoutSeconds = 30
        };

        var message = ConfigValidator.Validate<DatabaseConfig>()
            .NotEmpty(c => c.ConnectionString, "Connection string is required")
            .ToResult(validConfig)
            .Match(
                onSuccess: cfg => $"Config valid: {cfg.MaxPoolSize}",
                onFailure: error => $"Config invalid: {error}"
            );

        message.Should().Be("Config valid: 100");
    }
}
