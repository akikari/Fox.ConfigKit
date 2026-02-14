//==================================================================================================
// Validation rule that ensures secrets follow a specific secure format.
// Sealed class implementation for validating secret storage patterns.
//==================================================================================================
using System.Linq.Expressions;
using Fox.ConfigKit.Errors;
using Fox.ConfigKit.Security;

namespace Fox.ConfigKit.Validation.Rules;

//==================================================================================================
/// <summary>
/// Validates that a secret follows a specific secure format.
/// </summary>
/// <typeparam name="T">The type of the configuration class.</typeparam>
/// <param name="selector">Expression that selects the secret property.</param>
/// <param name="expectedFormat">The expected secret format.</param>
/// <param name="customMessage">Optional custom error message.</param>
//==================================================================================================
internal sealed class SecretFormatRule<T>(Expression<Func<T, string?>> selector, SecretFormat expectedFormat, string? customMessage = null) : ValidationRuleBase, IValidationRule<T> where T : class
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
            return null;
        }

        var isValid = expectedFormat switch
        {
            SecretFormat.AzureKeyVault => value.StartsWith("@Microsoft.KeyVault", StringComparison.OrdinalIgnoreCase),
            SecretFormat.AwsSecretsManager => value.StartsWith("arn:aws:secretsmanager", StringComparison.OrdinalIgnoreCase),
            SecretFormat.EnvironmentVariable => value.StartsWith("${") && value.EndsWith('}'),
            SecretFormat.UserSecrets => !SecretDetector.IsLikelySecret(value, propertyName),
            _ => false
        };

        if (!isValid)
        {
            var key = $"{sectionName}:{propertyName}";
            var message = customMessage ?? $"{propertyName} does not follow {expectedFormat} format";

            var suggestions = GetFormatSuggestions(expectedFormat);

            return new ConfigValidationError(key, message, "[REDACTED]", suggestions);
        }

        return null;
    }

    #endregion

    #region Private Methods

    //==============================================================================================
    /// <summary>
    /// Gets format-specific suggestions.
    /// </summary>
    /// <param name="format">The secret format type.</param>
    /// <returns>A list of format-specific suggestions.</returns>
    //==============================================================================================
    private static List<string> GetFormatSuggestions(SecretFormat format)
    {
        return format switch
        {
            SecretFormat.AzureKeyVault => ["Use format: @Microsoft.KeyVault(SecretUri=https://...)"],
            SecretFormat.AwsSecretsManager => ["Use format: arn:aws:secretsmanager:region:account:secret:name"],
            SecretFormat.EnvironmentVariable => ["Use format: ${VARIABLE_NAME}"],
            SecretFormat.UserSecrets => ["Use: dotnet user-secrets set \"key\" \"value\""],
            _ => ["Use a secure secret storage format"]
        };
    }

    #endregion
}
