using Fox.ConfigKit;
using Fox.ConfigKit.Samples.WebApi.Configuration;
using Fox.ConfigKit.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register and validate configurations using Fox.ConfigKit

// Example 1: Integer validation with Minimum/Maximum (inclusive boundaries)
builder.Services.AddConfigKit<ApplicationConfig>("Application")
    .NotEmpty(c => c.Name, "Application name is required")
    .MatchesPattern(c => c.Version, @"^\d+\.\d+\.\d+$", "Version must be in format X.Y.Z")
    .Minimum(c => c.MaxConcurrentRequests, 1, "Max concurrent requests must be at least 1")
    .Maximum(c => c.MaxConcurrentRequests, 1000, "Max concurrent requests cannot exceed 1000")
    .InRange(c => c.RequestTimeoutSeconds, 5, 300, "Request timeout must be between 5 and 300 seconds")
    .ValidateOnStartup();

// Example 2: Traditional InRange validation
builder.Services.AddConfigKit<DatabaseConfig>("Database")
    .NotEmpty(c => c.ConnectionString, "Database connection string is required")
    .InRange(c => c.CommandTimeoutSeconds, 1, 600, "Command timeout must be between 1 and 600 seconds")
    .InRange(c => c.MaxPoolSize, 1, 1000, "Max pool size must be between 1 and 1000")
    .When(c => c.RequireSsl, b =>
    {
        b.MatchesPattern(c => c.ConnectionString, "Encrypt=True|Encrypt=true", "SSL is required but connection string does not specify Encrypt=True");
    })
    .ValidateOnStartup();

// Example 3: GreaterThan/LessThan with exclusive boundaries
builder.Services.AddConfigKit<ExternalApiConfig>("ExternalApi")
    .NotEmpty(c => c.BaseUrl, "External API base URL is required")
    //.UrlReachable(c => c.BaseUrl, timeout: TimeSpan.FromSeconds(10), message: "External API base URL is not reachable")
    .NotEmpty(c => c.ApiKey, "External API key is required")
    .NoPlainTextSecrets(c => c.ApiKey, "API key appears to be a plain-text secret")
    .GreaterThan(c => c.TimeoutSeconds, 0, "API timeout must be greater than 0")
    .LessThan(c => c.TimeoutSeconds, 600, "API timeout must be less than 600 seconds")
    .InRange(c => c.MaxRetries, 0, 10, "Max retries must be between 0 and 10")
    .ValidateOnStartup();

// Example 4: File system validation
builder.Services.AddConfigKit<LoggingConfig>("CustomLogging")
    .NotEmpty(c => c.LogDirectory, "Log directory path is required")
    .DirectoryExists(c => c.LogDirectory, message: "Log directory does not exist")
    .InRange(c => c.RetentionDays, 1, 365, "Retention days must be between 1 and 365")
    .InRange(c => c.MaxFileSizeMB, 1, 10000, "Max file size must be between 1 and 10000 MB")
    .ValidateOnStartup();

// Example 5: Conditional validation with environment checks
builder.Services.AddConfigKit<SecurityConfig>("Security")
    .NotEmpty(c => c.Environment, "Environment is required")
    .When(c => c.Environment == "Production", b =>
    {
        b.NotEmpty(c => c.CertificatePath, "Certificate path is required in production")
         .FileExists(c => c.CertificatePath, message: "Certificate file does not exist");
    })
    .When(c => c.RequireHttps && !string.IsNullOrEmpty(c.CertificatePath), b =>
    {
        b.FileExists(c => c.CertificatePath, message: "Certificate file must exist when HTTPS is required");
    })
    .ValidateOnStartup();

// Example 6: Generic validation with decimal, DateTime, and TimeSpan types
builder.Services.AddConfigKit<CampaignConfig>("Campaign")
    .NotEmpty(c => c.Name, "Campaign name is required")
    .Minimum(c => c.StartDate, DateTime.Today, "Campaign must start today or later")
    .GreaterThan(c => c.EndDate, DateTime.Today, "Campaign end date must be in the future")
    .Minimum(c => c.MinimumPurchaseAmount, 0.01m, "Minimum purchase amount must be at least $0.01")
    .Maximum(c => c.MaximumDiscountPercentage, 0.75m, "Discount percentage cannot exceed 75%")
    .GreaterThan(c => c.EmailReminderInterval, TimeSpan.Zero, "Email reminder interval must be positive")
    .Maximum(c => c.CacheDuration, TimeSpan.FromHours(24), "Cache duration cannot exceed 24 hours")
    .ValidateOnStartup();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
