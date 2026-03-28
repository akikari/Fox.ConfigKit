//==================================================================================================
// Configuration for data migration with batch processing settings.
// Demonstrates property-to-property validation where RecordsPerRun must be >= BatchSize.
//==================================================================================================

namespace Fox.ConfigKit.Samples.WebApi.Configuration;

public sealed class MigrationConfig
{
    public int RecordsPerRun { get; set; }
    public int BatchSize { get; set; }
    public int MaxRetryAttempts { get; set; }
    public int RetryDelaySeconds { get; set; }
    public int CommandTimeoutSeconds { get; set; }
}
