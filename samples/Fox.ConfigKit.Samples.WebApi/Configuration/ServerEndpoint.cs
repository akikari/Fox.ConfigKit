//==================================================================================================
// Represents a single server endpoint with health check configuration.
// Used as collection item in ServerConfig for endpoint validation demonstration.
//==================================================================================================

namespace Fox.ConfigKit.Samples.WebApi.Configuration;

public sealed class ServerEndpoint
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool Enabled { get; set; }
    public int HealthCheckIntervalSeconds { get; set; }
}
