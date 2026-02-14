//==================================================================================================
// Configuration for security and SSL/TLS settings.
// Demonstrates conditional validation based on environment and file existence checks.
//==================================================================================================

namespace Fox.ConfigKit.Samples.WebApi.Configuration;

public sealed class SecurityConfig
{
    public string Environment { get; set; } = "Development";
    public string CertificatePath { get; set; } = string.Empty;
    public string CertificatePassword { get; set; } = string.Empty;
    public bool RequireHttps { get; set; }
    public string[] AllowedOrigins { get; set; } = [];
}
