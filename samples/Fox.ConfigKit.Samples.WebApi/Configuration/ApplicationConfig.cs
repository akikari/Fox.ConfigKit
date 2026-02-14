//==================================================================================================
// Configuration for application-wide settings.
// Demonstrates string pattern validation and range constraints.
//==================================================================================================

namespace Fox.ConfigKit.Samples.WebApi.Configuration;

public sealed class ApplicationConfig
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public int MaxConcurrentRequests { get; set; }
    public int RequestTimeoutSeconds { get; set; }
    public bool EnableMetrics { get; set; }
}
