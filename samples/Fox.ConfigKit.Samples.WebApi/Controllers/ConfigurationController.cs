//==================================================================================================
// Controller to demonstrate configuration usage and validation.
// Shows how validated configurations are injected and used in controllers.
//==================================================================================================
using Fox.ConfigKit.Samples.WebApi.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Fox.ConfigKit.Samples.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigurationController(
    IOptions<ApplicationConfig> applicationConfig,
    IOptions<DatabaseConfig> databaseConfig,
    IOptions<ExternalApiConfig> externalApiConfig,
    IOptions<LoggingConfig> loggingConfig,
    IOptions<SecurityConfig> securityConfig) : ControllerBase
{
    private readonly ApplicationConfig applicationConfig = applicationConfig.Value;
    private readonly DatabaseConfig databaseConfig = databaseConfig.Value;
    private readonly ExternalApiConfig externalApiConfig = externalApiConfig.Value;
    private readonly LoggingConfig loggingConfig = loggingConfig.Value;
    private readonly SecurityConfig securityConfig = securityConfig.Value;

    [HttpGet("application")]
    public IActionResult GetApplicationConfig()
    {
        return Ok(new
        {
            applicationConfig.Name,
            applicationConfig.Version,
            applicationConfig.MaxConcurrentRequests,
            applicationConfig.RequestTimeoutSeconds,
            applicationConfig.EnableMetrics
        });
    }

    [HttpGet("database")]
    public IActionResult GetDatabaseConfig()
    {
        return Ok(new
        {
            HasConnectionString = !string.IsNullOrEmpty(databaseConfig.ConnectionString),
            databaseConfig.CommandTimeoutSeconds,
            databaseConfig.MaxPoolSize,
            databaseConfig.EnableSensitiveDataLogging,
            databaseConfig.RequireSsl
        });
    }

    [HttpGet("external-api")]
    public IActionResult GetExternalApiConfig()
    {
        return Ok(new
        {
            externalApiConfig.BaseUrl,
            HasApiKey = !string.IsNullOrEmpty(externalApiConfig.ApiKey),
            externalApiConfig.TimeoutSeconds,
            externalApiConfig.MaxRetries
        });
    }

    [HttpGet("logging")]
    public IActionResult GetLoggingConfig()
    {
        return Ok(new
        {
            loggingConfig.LogDirectory,
            loggingConfig.MinimumLevel,
            loggingConfig.RetentionDays,
            loggingConfig.MaxFileSizeMB
        });
    }

    [HttpGet("security")]
    public IActionResult GetSecurityConfig()
    {
        return Ok(new
        {
            securityConfig.Environment,
            HasCertificate = !string.IsNullOrEmpty(securityConfig.CertificatePath),
            securityConfig.RequireHttps,
            AllowedOriginsCount = securityConfig.AllowedOrigins.Length
        });
    }

    [HttpGet("all")]
    public IActionResult GetAllConfigs()
    {
        return Ok(new
        {
            Application = new
            {
                applicationConfig.Name,
                applicationConfig.Version
            },
            Database = new
            {
                HasConnectionString = !string.IsNullOrEmpty(databaseConfig.ConnectionString),
                databaseConfig.MaxPoolSize
            },
            ExternalApi = new
            {
                externalApiConfig.BaseUrl,
                HasApiKey = !string.IsNullOrEmpty(externalApiConfig.ApiKey)
            },
            Logging = new
            {
                loggingConfig.LogDirectory,
                loggingConfig.RetentionDays
            },
            Security = new
            {
                securityConfig.Environment,
                securityConfig.RequireHttps
            }
        });
    }
}
