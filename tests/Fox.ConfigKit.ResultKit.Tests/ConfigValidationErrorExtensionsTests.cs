//==================================================================================================
// Unit tests for ConfigValidationError to Result pattern error conversion extensions.
// Validates error code generation and message preservation.
//==================================================================================================
using FluentAssertions;
using Fox.ConfigKit.Errors;
using Fox.ResultKit;

namespace Fox.ConfigKit.ResultKit.Tests;

public sealed class ConfigValidationErrorExtensionsTests
{
    [Fact]
    public void ToResultError_should_convert_simple_key_to_uppercase_error_code()
    {
        var error = new ConfigValidationError("Name", "Name is required");

        var result = error.ToResultError();
        var (code, message) = ResultError.Parse(result);

        code.Should().Be("VALIDATION_NAME");
        message.Should().Be("Name is required");
    }

    [Fact]
    public void ToResultError_should_convert_dotted_key_to_underscored_error_code()
    {
        var error = new ConfigValidationError("Database.ConnectionString", "Connection string is required");

        var result = error.ToResultError();
        var (code, message) = ResultError.Parse(result);

        code.Should().Be("VALIDATION_DATABASE_CONNECTIONSTRING");
        message.Should().Be("Connection string is required");
    }

    [Fact]
    public void ToResultError_should_convert_nested_key_to_error_code()
    {
        var error = new ConfigValidationError("App.Security.Certificate.Path", "Certificate path not found");

        var result = error.ToResultError();
        var (code, message) = ResultError.Parse(result);

        code.Should().Be("VALIDATION_APP_SECURITY_CERTIFICATE_PATH");
        message.Should().Be("Certificate path not found");
    }

    [Fact]
    public void ToResultError_should_preserve_message()
    {
        var expectedMessage = "Port must be between 1 and 65535";
        var error = new ConfigValidationError("Port", expectedMessage);

        var result = error.ToResultError();
        var (_, message) = ResultError.Parse(result);

        message.Should().Be(expectedMessage);
    }

    [Fact]
    public void ToResultErrors_should_convert_multiple_errors()
    {
        var errors = new[]
        {
            new ConfigValidationError("Name", "Name is required"),
            new ConfigValidationError("Email", "Email is required"),
            new ConfigValidationError("Database.ConnectionString", "Connection string is required")
        };

        var results = errors.ToResultErrors().ToList();

        results.Should().HaveCount(3);
        var (code1, _) = ResultError.Parse(results[0]);
        var (code2, _) = ResultError.Parse(results[1]);
        var (code3, _) = ResultError.Parse(results[2]);

        code1.Should().Be("VALIDATION_NAME");
        code2.Should().Be("VALIDATION_EMAIL");
        code3.Should().Be("VALIDATION_DATABASE_CONNECTIONSTRING");
    }

    [Fact]
    public void ToResultErrors_should_return_empty_for_empty_input()
    {
        var errors = Enumerable.Empty<ConfigValidationError>();

        var results = errors.ToResultErrors().ToList();

        results.Should().BeEmpty();
    }

    [Fact]
    public void ToResultError_should_handle_lowercase_keys()
    {
        var error = new ConfigValidationError("apikey", "API key is required");

        var result = error.ToResultError();
        var (code, _) = ResultError.Parse(result);

        code.Should().Be("VALIDATION_APIKEY");
    }

    [Fact]
    public void ToResultError_should_handle_mixed_case_keys()
    {
        var error = new ConfigValidationError("Api.BaseUrl", "Base URL is required");

        var result = error.ToResultError();
        var (code, _) = ResultError.Parse(result);

        code.Should().Be("VALIDATION_API_BASEURL");
    }
}
