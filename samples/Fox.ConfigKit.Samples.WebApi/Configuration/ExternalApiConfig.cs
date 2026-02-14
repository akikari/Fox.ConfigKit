//==================================================================================================
// Configuration for external API integration.
// Demonstrates URL validation, API key security checks, and reachability validation.
//==================================================================================================

namespace Fox.ConfigKit.Samples.WebApi.Configuration;

public sealed class ExternalApiConfig
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; }
    public int MaxRetries { get; set; }
}
