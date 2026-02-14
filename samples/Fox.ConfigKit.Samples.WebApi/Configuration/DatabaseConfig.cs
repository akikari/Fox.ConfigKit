//==================================================================================================
// Configuration for database connection settings.
// Demonstrates connection string validation, timeout constraints, and SSL requirements.
//==================================================================================================

namespace Fox.ConfigKit.Samples.WebApi.Configuration;

public sealed class DatabaseConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeoutSeconds { get; set; }
    public int MaxPoolSize { get; set; }
    public bool EnableSensitiveDataLogging { get; set; }
    public bool RequireSsl { get; set; }
}
