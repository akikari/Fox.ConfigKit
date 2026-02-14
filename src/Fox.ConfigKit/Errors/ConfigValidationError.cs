//==================================================================================================
// Represents the context for configuration validation failures with detailed error information.
// Sealed class implementation for configuration error tracking.
//==================================================================================================
using System.Text;

namespace Fox.ConfigKit.Errors;

//==================================================================================================
/// <summary>
/// Represents a configuration validation error with detailed context and fix suggestions.
/// </summary>
/// <param name="key">The configuration key that failed validation.</param>
/// <param name="message">The error message describing what went wrong.</param>
/// <param name="currentValue">The current value that failed validation (optional).</param>
/// <param name="suggestions">Suggestions on how to fix the validation error (optional).</param>
//==================================================================================================
public sealed class ConfigValidationError(string key, string message, object? currentValue = null, IEnumerable<string>? suggestions = null)
{
    #region Properties

    //==============================================================================================
    /// <summary>
    /// Gets the configuration key that failed validation.
    /// </summary>
    //==============================================================================================
    public string Key { get; } = key;

    //==============================================================================================
    /// <summary>
    /// Gets the error message describing what went wrong.
    /// </summary>
    //==============================================================================================
    public string Message { get; } = message;

    //==============================================================================================
    /// <summary>
    /// Gets the current value that failed validation (if applicable).
    /// </summary>
    //==============================================================================================
    public object? CurrentValue { get; } = currentValue;

    //==============================================================================================
    /// <summary>
    /// Gets suggestions on how to fix the validation error.
    /// </summary>
    //==============================================================================================
    public IReadOnlyList<string> Suggestions { get; } = suggestions?.ToList() ?? [];

    #endregion

    #region Public Methods

    //==============================================================================================
    /// <summary>
    /// Returns a formatted error message with suggestions.
    /// </summary>
    //==============================================================================================
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"  ✗ {Key}: {Message}");

        if (CurrentValue != null)
        {
            sb.AppendLine($"    Current value: {CurrentValue}");
        }

        foreach (var suggestion in Suggestions)
        {
            sb.AppendLine($"    → {suggestion}");
        }

        return sb.ToString();
    }

    #endregion
}
