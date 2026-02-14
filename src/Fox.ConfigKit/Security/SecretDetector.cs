//==================================================================================================
// Detects plain-text secrets in configuration values.
// Sealed class implementation for identifying common secret patterns.
//==================================================================================================
using System.Text.RegularExpressions;

namespace Fox.ConfigKit.Security;

//==============================================================================================
/// <summary>
/// Detects potential secrets in configuration values.
/// </summary>
//==============================================================================================
public static partial class SecretDetector
{
    #region Fields

    [GeneratedRegex(@"^sk-[a-zA-Z0-9]{20,}$")]
    private static partial Regex OpenAiApiKeyPattern();

    [GeneratedRegex(@"^[a-zA-Z0-9]{32,}$")]
    private static partial Regex GenericBase64Pattern();

    [GeneratedRegex(@"^Bearer\s+[a-zA-Z0-9\-._~+/]+=*$", RegexOptions.IgnoreCase)]
    private static partial Regex BearerTokenPattern();

    [GeneratedRegex(@"^[a-f0-9]{64}$")]
    private static partial Regex Hex64Pattern();

    [GeneratedRegex(@"^AIza[0-9A-Za-z\-_]{35}$")]
    private static partial Regex GoogleApiKeyPattern();

    [GeneratedRegex(@"AKIA[0-9A-Z]{16}")]
    private static partial Regex AwsAccessKeyPattern();

    private static readonly Func<Regex>[] SecretPatterns =
    [
        OpenAiApiKeyPattern,
        GenericBase64Pattern,
        BearerTokenPattern,
        Hex64Pattern,
        GoogleApiKeyPattern,
        AwsAccessKeyPattern,
    ];

    private static readonly string[] SecretKeywords =
    [
        "password", "passwd", "pwd", "secret", "token", "apikey", "api_key",
        "private_key", "privatekey", "client_secret", "clientsecret"
    ];

    #endregion

    #region Public Methods

    //==============================================================================================
    /// <summary>
    /// Detects if a value appears to be a plain-text secret.
    /// </summary>
    /// <param name="value">The value to check for secret patterns.</param>
    /// <param name="propertyName">The name of the property being validated.</param>
    /// <returns>True if the value appears to be a plain-text secret; otherwise, false.</returns>
    //==============================================================================================
    public static bool IsLikelySecret(string? value, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var lowerPropertyName = propertyName.ToLowerInvariant();
        if (SecretKeywords.Any(keyword => lowerPropertyName.Contains(keyword)))
        {
            if (IsSecureReference(value))
            {
                return false;
            }

            return SecretPatterns.Any(patternFactory => patternFactory().IsMatch(value));
        }

        return false;
    }

    //==============================================================================================
    /// <summary>
    /// Checks if a value is a secure reference (e.g., Azure Key Vault).
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>True if the value is a secure reference; otherwise, false.</returns>
    //==============================================================================================
    public static bool IsSecureReference(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.StartsWith("@Microsoft.KeyVault", StringComparison.OrdinalIgnoreCase) ||
               value.StartsWith("arn:aws:secretsmanager", StringComparison.OrdinalIgnoreCase) ||
               value.StartsWith("${", StringComparison.OrdinalIgnoreCase);
    }

    #endregion
}
