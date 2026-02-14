//==================================================================================================
// Validation rule that ensures a string property is not null or empty.
// Sealed class implementation for non-empty string validation.
//==================================================================================================
using System.Linq.Expressions;
using Fox.ConfigKit.Errors;

namespace Fox.ConfigKit.Validation.Rules;

//==================================================================================================
/// <summary>
/// Validates that a string property is not null, empty, or whitespace.
/// </summary>
/// <typeparam name="T">The type of the configuration class.</typeparam>
/// <param name="selector">Expression that selects the string property.</param>
/// <param name="customMessage">Optional custom error message.</param>
//==================================================================================================
internal sealed class NotEmptyRule<T>(Expression<Func<T, string?>> selector, string? customMessage = null) : ValidationRuleBase, IValidationRule<T> where T : class
{
    #region Fields

    private readonly Func<T, string?> getValue = selector.Compile();
    private readonly string propertyName = GetPropertyName(selector);

    #endregion

    #region Public Methods

    //==============================================================================================
    /// <summary>
    /// Validates the rule.
    /// </summary>
    /// <param name="options">The configuration object to validate.</param>
    /// <param name="sectionName">The configuration section name.</param>
    /// <returns>A validation error if the rule fails; otherwise, null.</returns>
    //==============================================================================================
    public ConfigValidationError? Validate(T options, string sectionName)
    {
        var value = getValue(options);

        if (string.IsNullOrWhiteSpace(value))
        {
            var key = $"{sectionName}:{propertyName}";
            var message = customMessage ?? $"{propertyName} must not be empty";
            var envVar = key.Replace(":", "__").ToUpperInvariant();

            var suggestions = new List<string>
            {
                $"Set via: dotnet user-secrets set \"{key}\" \"<value>\"",
                $"Or set environment variable: {envVar}",
                "Or update appsettings.json"
            };

            return new ConfigValidationError(key, message, value, suggestions);
        }

        return null;
    }

    #endregion
}
