//==================================================================================================
// Validation rule that warns if a value matches a known default/insecure value.
// Sealed class implementation for default value detection.
//==================================================================================================
using System.Linq.Expressions;
using Fox.ConfigKit.Errors;
using Fox.ConfigKit.Security;

namespace Fox.ConfigKit.Validation.Rules;

//==================================================================================================
/// <summary>
/// Validates that a value is not a known default/insecure value.
/// </summary>
/// <typeparam name="T">The type of the configuration class.</typeparam>
/// <param name="selector">Expression that selects the property to validate.</param>
/// <param name="defaultValue">The known default value to check against.</param>
/// <param name="level">The security level for the validation.</param>
/// <param name="customMessage">Optional custom error message.</param>
//==================================================================================================
internal sealed class DefaultValueWarningRule<T>(Expression<Func<T, string?>> selector, string defaultValue, SecurityLevel level = SecurityLevel.Warning, string? customMessage = null) : ValidationRuleBase, IValidationRule<T> where T : class
{
    #region Fields

    private readonly Func<T, string?> getValue = selector.Compile();
    private readonly string propertyName = GetPropertyName(selector);
    private readonly string defaultValue = defaultValue;
    private readonly SecurityLevel level = level;
    private readonly string? customMessage = customMessage;

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

        if (string.Equals(value, defaultValue, StringComparison.OrdinalIgnoreCase))
        {
            var key = $"{sectionName}:{propertyName}";
            var levelPrefix = level switch
            {
                SecurityLevel.Critical => "CRITICAL",
                SecurityLevel.Warning => "WARNING",
                SecurityLevel.Info => "INFO",
                _ => "WARNING"
            };

            var message = customMessage ?? $"[{levelPrefix}] {propertyName} is using default/insecure value";

            var suggestions = new List<string>
            {
                "Change to a secure value",
                $"Default value '{defaultValue}' should not be used in production"
            };

            return new ConfigValidationError(key, message, "[REDACTED]", suggestions);
        }

        return null;
    }

    #endregion
}
