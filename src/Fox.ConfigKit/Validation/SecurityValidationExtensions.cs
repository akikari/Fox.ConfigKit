//==================================================================================================
// Provides extension methods for security-focused validation rules.
// Fluent API extensions for detecting secrets and security issues in configuration.
//==================================================================================================
using System.Linq.Expressions;
using Fox.ConfigKit.Security;
using Fox.ConfigKit.Validation.Rules;

namespace Fox.ConfigKit.Validation;

//==============================================================================================
/// <summary>
/// Extension methods for security validation rules.
/// </summary>
//==============================================================================================
public static class SecurityValidationExtensions
{
    #region Public Methods

    //==============================================================================================
    /// <summary>
    /// Validates that a value does not contain plain-text secrets.
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="selector">Expression to select the property to check.</param>
    /// <param name="message">Custom error message.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> NoPlainTextSecrets<T>(this ConfigValidationBuilder<T> builder, Expression<Func<T, string?>> selector, string? message = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new NoPlainTextSecretsRule<T>(selector, message));
    }

    //==============================================================================================
    /// <summary>
    /// Validates that a secret follows a specific format (e.g., Azure Key Vault reference).
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="selector">Expression to select the secret property.</param>
    /// <param name="format">The expected secret format.</param>
    /// <param name="message">Custom error message.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> ValidateSecretFormat<T>(this ConfigValidationBuilder<T> builder, Expression<Func<T, string?>> selector, SecretFormat format, string? message = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new SecretFormatRule<T>(selector, format, message));
    }

    //==============================================================================================
    /// <summary>
    /// Warns if a value matches a known default/insecure value.
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="selector">Expression to select the property to check.</param>
    /// <param name="defaultValue">The default value to check against.</param>
    /// <param name="level">The security level of the warning.</param>
    /// <param name="message">Custom error message.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> WarnIfDefaultValue<T>(this ConfigValidationBuilder<T> builder, Expression<Func<T, string?>> selector, string defaultValue, SecurityLevel level = SecurityLevel.Warning, string? message = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new DefaultValueWarningRule<T>(selector, defaultValue, level, message));
    }

    #endregion
}
