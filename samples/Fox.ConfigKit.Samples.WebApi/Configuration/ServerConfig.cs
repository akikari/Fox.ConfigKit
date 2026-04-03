//==================================================================================================
// Configuration for server endpoints with health check settings.
// Demonstrates collection validation with ValidateEach for endpoint lists.
//==================================================================================================

namespace Fox.ConfigKit.Samples.WebApi.Configuration;

public sealed class ServerConfig
{
    public List<ServerEndpoint> Endpoints { get; set; } = [];
    public int MaxRetries { get; set; }
    public int TimeoutSeconds { get; set; }
}
