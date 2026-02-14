//==================================================================================================
// Validation rule that ensures a URL is reachable via HTTP request.
// Sealed class implementation with configurable timeout.
//==================================================================================================
using System.Linq.Expressions;
using Fox.ConfigKit.Errors;

namespace Fox.ConfigKit.Validation.Rules;

//==================================================================================================
/// <summary>
/// Validates that a URL is reachable.
/// </summary>
/// <typeparam name="T">The type of the configuration class.</typeparam>
/// <param name="selector">Expression that selects the URL property.</param>
/// <param name="timeout">The timeout for the HTTP request.</param>
/// <param name="customMessage">Optional custom error message.</param>
//==================================================================================================
internal sealed class UrlReachableRule<T>(Expression<Func<T, string?>> selector, TimeSpan timeout, string? customMessage = null) : ValidationRuleBase, IValidationRule<T> where T : class
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
        var url = getValue(options);

        if (string.IsNullOrWhiteSpace(url))
        {
            var key = $"{sectionName}:{propertyName}";
            var message = customMessage ?? $"{propertyName} URL is not specified";
            return new ConfigValidationError(key, message, url, ["Specify a valid URL"]);
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            var key = $"{sectionName}:{propertyName}";
            var message = $"Invalid URL format: {url}";
            return new ConfigValidationError(key, message, url, ["Use format: http://example.com or https://example.com"]);
        }

        try
        {
            using var httpClient = new HttpClient { Timeout = timeout };
            var response = httpClient.GetAsync(uri).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                var key = $"{sectionName}:{propertyName}";
                var message = customMessage ?? $"URL returned {(int)response.StatusCode}: {url}";
                return new ConfigValidationError(key, message, url, ["Check URL availability and network connectivity"]);
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
        {
            var key = $"{sectionName}:{propertyName}";
            var message = customMessage ?? $"Failed to reach URL: {ex.Message}";
            return new ConfigValidationError(key, message, url, ["Check URL availability and network connectivity"]);
        }

        return null;
    }

    #endregion
}
