//==================================================================================================
// Tests for ConfigValidationError class.
// Verifies error representation and formatting.
//==================================================================================================
using FluentAssertions;
using Fox.ConfigKit.Errors;

namespace Fox.ConfigKit.Tests;

//==================================================================================================
/// <summary>
/// Tests for <see cref="ConfigValidationError"/>.
/// </summary>
//==================================================================================================
public sealed class ConfigValidationErrorTests
{
    #region Constructor Tests

    //==============================================================================================
    /// <summary>
    /// Constructor should initialize properties correctly.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void Constructor_should_initialize_properties()
    {
        var error = new ConfigValidationError("Test:Key", "Test message", "current", ["Fix 1", "Fix 2"]);

        error.Key.Should().Be("Test:Key");
        error.Message.Should().Be("Test message");
        error.CurrentValue.Should().Be("current");
        error.Suggestions.Should().HaveCount(2);
        error.Suggestions[0].Should().Be("Fix 1");
        error.Suggestions[1].Should().Be("Fix 2");
    }

    //==============================================================================================
    /// <summary>
    /// Constructor should handle null currentValue.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void Constructor_should_handle_null_current_value()
    {
        var error = new ConfigValidationError("Test:Key", "Test message", null, ["Fix"]);

        error.CurrentValue.Should().BeNull();
    }

    //==============================================================================================
    /// <summary>
    /// Constructor should handle null suggestions.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void Constructor_should_handle_null_suggestions()
    {
        var error = new ConfigValidationError("Test:Key", "Test message", "current", null);

        error.Suggestions.Should().BeEmpty();
    }

    #endregion

    #region ToString Tests

    //==============================================================================================
    /// <summary>
    /// ToString() should format error with key and message.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ToString_should_format_error_with_key_and_message()
    {
        var error = new ConfigValidationError("App:Timeout", "Value must be positive");

        var result = error.ToString();

        result.Should().Contain("✗ App:Timeout: Value must be positive");
    }

    //==============================================================================================
    /// <summary>
    /// ToString() should include current value when provided.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ToString_should_include_current_value_when_provided()
    {
        var error = new ConfigValidationError("App:Port", "Invalid port", -1);

        var result = error.ToString();

        result.Should().Contain("✗ App:Port: Invalid port");
        result.Should().Contain("Current value: -1");
    }

    //==============================================================================================
    /// <summary>
    /// ToString() should not include current value line when null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ToString_should_not_include_current_value_when_null()
    {
        var error = new ConfigValidationError("App:Name", "Name is required", null);

        var result = error.ToString();

        result.Should().Contain("✗ App:Name: Name is required");
        result.Should().NotContain("Current value:");
    }

    //==============================================================================================
    /// <summary>
    /// ToString() should include suggestions when provided.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ToString_should_include_suggestions_when_provided()
    {
        var error = new ConfigValidationError("App:ApiKey", "Plain-text secret detected", "abc123", ["Use Azure Key Vault", "Use environment variables"]);

        var result = error.ToString();

        result.Should().Contain("✗ App:ApiKey: Plain-text secret detected");
        result.Should().Contain("Current value: abc123");
        result.Should().Contain("→ Use Azure Key Vault");
        result.Should().Contain("→ Use environment variables");
    }

    //==============================================================================================
    /// <summary>
    /// ToString() should handle empty suggestions list.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ToString_should_handle_empty_suggestions()
    {
        var error = new ConfigValidationError("App:Value", "Invalid value", "test", []);

        var result = error.ToString();

        result.Should().Contain("✗ App:Value: Invalid value");
        result.Should().NotContain("→");
    }

    //==============================================================================================
    /// <summary>
    /// ToString() should handle multiple suggestions.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ToString_should_handle_multiple_suggestions()
    {
        var suggestions = new[] { "Suggestion 1", "Suggestion 2", "Suggestion 3" };
        var error = new ConfigValidationError("Test:Key", "Error", null, suggestions);

        var result = error.ToString();

        result.Should().Contain("→ Suggestion 1");
        result.Should().Contain("→ Suggestion 2");
        result.Should().Contain("→ Suggestion 3");
    }

    #endregion
}
