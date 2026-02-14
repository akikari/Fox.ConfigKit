//==================================================================================================
// Configuration for logging settings.
// Demonstrates file system validation for log directories and file paths.
//==================================================================================================

namespace Fox.ConfigKit.Samples.WebApi.Configuration;

public sealed class LoggingConfig
{
    public string LogDirectory { get; set; } = string.Empty;
    public string MinimumLevel { get; set; } = "Information";
    public int RetentionDays { get; set; }
    public int MaxFileSizeMB { get; set; }
}
