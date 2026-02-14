//==================================================================================================
// Validation rule that detects plain-text secrets in configuration.
// Sealed class implementation for security-focused secret detection.
//==================================================================================================
using System.Linq.Expressions;
using Fox.ConfigKit.Errors;
using Fox.ConfigKit.Security;

namespace Fox.ConfigKit.Validation.Rules;

//==================================================================================================
/// <summary>
/// Validates that a value does not contain plain-text secrets.
/// </summary>
/// <typeparam name="T">The type of the configuration class.</typeparam>
/// <param name="selector">Expression that selects the string property to check.</param>
/// <param name="customMessage">Optional custom error message.</param>
//==================================================================================================
internal sealed class NoPlainTextSecretsRule<T>(Expression<Func<T, string?>> selector, string? customMessage = null) : ValidationRuleBase, IValidationRule<T> where T : class
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

        if (!string.IsNullOrWhiteSpace(value) && SecretDetector.IsLikelySecret(value, propertyName))
        {
            var key = $"{sectionName}:{propertyName}";
            var message = customMessage ?? $"{propertyName} appears to contain a plain-text secret";

            var suggestions = new List<string>
            {
                "Use Azure Key Vault: @Microsoft.KeyVault(SecretUri=...)",
                "Use User Secrets: dotnet user-secrets set",
                "Use environment variables for sensitive data"
            };

            return new ConfigValidationError(key, message, "[REDACTED]", suggestions);
        }

        return null;
    }

    #endregion
}
