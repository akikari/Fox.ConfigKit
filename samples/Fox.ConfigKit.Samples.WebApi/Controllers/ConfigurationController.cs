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
    IOptions<SecurityConfig> securityConfig,
    IOptions<CampaignConfig> campaignConfig,
    IOptions<MigrationConfig> migrationConfig,
    IOptions<ServerConfig> serverConfig) : ControllerBase
{
    private readonly ApplicationConfig applicationConfig = applicationConfig.Value;
    private readonly DatabaseConfig databaseConfig = databaseConfig.Value;
    private readonly ExternalApiConfig externalApiConfig = externalApiConfig.Value;
    private readonly LoggingConfig loggingConfig = loggingConfig.Value;
    private readonly SecurityConfig securityConfig = securityConfig.Value;
    private readonly CampaignConfig campaignConfig = campaignConfig.Value;
    private readonly MigrationConfig migrationConfig = migrationConfig.Value;
    private readonly ServerConfig serverConfig = serverConfig.Value;

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

    [HttpGet("campaign")]
    public IActionResult GetCampaignConfig()
    {
        return Ok(new
        {
            campaignConfig.Name,
            campaignConfig.StartDate,
            campaignConfig.EndDate,
            campaignConfig.MinimumPurchaseAmount,
            campaignConfig.MaximumDiscountPercentage,
            campaignConfig.EmailReminderInterval,
            campaignConfig.CacheDuration
        });
    }

    [HttpGet("migration")]
    public IActionResult GetMigrationConfig()
    {
        return Ok(new
        {
            migrationConfig.RecordsPerRun,
            migrationConfig.BatchSize,
            migrationConfig.MaxRetryAttempts,
            migrationConfig.RetryDelaySeconds,
            migrationConfig.CommandTimeoutSeconds,
            IsValid = migrationConfig.RecordsPerRun == 0 || migrationConfig.RecordsPerRun >= migrationConfig.BatchSize
        });
    }

    [HttpGet("servers")]
    public IActionResult GetServerConfig()
    {
        return Ok(new
        {
            serverConfig.MaxRetries,
            serverConfig.TimeoutSeconds,
            EndpointCount = serverConfig.Endpoints.Count,
            EnabledEndpoints = serverConfig.Endpoints.Count(e => e.Enabled),
            Endpoints = serverConfig.Endpoints.Select(e => new
            {
                e.Name,
                e.Url,
                e.Port,
                e.Enabled,
                e.HealthCheckIntervalSeconds
            })
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
            },
            Campaign = new
            {
                campaignConfig.Name,
                campaignConfig.StartDate,
                campaignConfig.EndDate
            },
            Migration = new
            {
                migrationConfig.RecordsPerRun,
                migrationConfig.BatchSize,
                IsValid = migrationConfig.RecordsPerRun == 0 || migrationConfig.RecordsPerRun >= migrationConfig.BatchSize
            },
            Servers = new
            {
                EndpointCount = serverConfig.Endpoints.Count,
                EnabledEndpoints = serverConfig.Endpoints.Count(e => e.Enabled)
            }
        });
    }
}
