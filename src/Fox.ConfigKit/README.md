# đźŽŻ Fox.ConfigKit

> **Lightweight .NET configuration validation library with fail-fast startup validation**

Fox.ConfigKit validates `IOptions<T>` configurations at application startup, catching errors before they cause runtime failures.

## đź“¦ Installation

```bash
dotnet add package Fox.ConfigKit
```

**NuGet Package Manager:**
```
Install-Package Fox.ConfigKit
```

**PackageReference:**
```xml
<PackageReference Include="Fox.ConfigKit" Version="1.0.0" />
```

## đźŽŻ Core Concepts

### Fail-Fast Validation

Configuration errors are caught at application startup, not at runtime:

```csharp
builder.Services.AddConfigKit<DatabaseConfig>("Database")
    .NotEmpty(c => c.ConnectionString, "Connection string is required")
    .InRange(c => c.MaxPoolSize, 1, 1000, "Max pool size must be between 1 and 1000")
    .ValidateOnStartup();

// Application won't start if configuration is invalid!
```

### Fluent API

Type-safe, chainable validation rules:

```csharp
builder.Services.AddConfigKit<ApiConfig>("Api")
    .NotEmpty(c => c.BaseUrl, "API URL is required")
    .UrlReachable(c => c.BaseUrl, message: "API is not reachable")
    .InRange(c => c.TimeoutSeconds, 5, 300, "Timeout must be between 5 and 300 seconds")
    .ValidateOnStartup();
```

### IOptions Integration

Seamlessly integrates with `Microsoft.Extensions.Options`:

```csharp
public class MyService
{
    private readonly ApiConfig config;

    public MyService(IOptions<ApiConfig> config)
    {
        this.config = config.Value; // Already validated at startup!
    }
}
```

## đź”‘ Key Validation Rules

### String Validation

```csharp
builder.Services.AddConfigKit<AppConfig>("App")
    .NotEmpty(c => c.Name, "Application name is required")
    .NotNull(c => c.Version, "Version is required")
    .MatchesPattern(c => c.Version, @"^\d+\.\d+\.\d+$", "Version must be X.Y.Z format")
    .ValidateOnStartup();
```

### Comparable Value Validation

All comparison methods support any `IComparable<T>` type (int, decimal, DateTime, TimeSpan, etc.):

**Integer validation:**
```csharp
builder.Services.AddConfigKit<ApiConfig>("Api")
    .GreaterThan(c => c.Port, 1024, "Port must be > 1024")
    .Maximum(c => c.Port, 65535, "Port must be <= 65535")
    .InRange(c => c.MaxRetries, 0, 10, "Max retries must be 0-10")
    .ValidateOnStartup();
```

**Decimal validation:**
```csharp
builder.Services.AddConfigKit<ProductConfig>("Product")
    .Minimum(c => c.Price, 0.01m, "Price must be at least $0.01")
    .Maximum(c => c.Discount, 0.5m, "Discount cannot exceed 50%")
    .ValidateOnStartup();
```

**DateTime validation:**
```csharp
builder.Services.AddConfigKit<CampaignConfig>("Campaign")
    .Minimum(c => c.StartDate, DateTime.Today, "Campaign must start today or later")
    .LessThan(c => c.EndDate, new DateTime(2025, 12, 31), "Campaign must end before 2025")
    .ValidateOnStartup();
```

**TimeSpan validation:**
```csharp
builder.Services.AddConfigKit<ApiConfig>("Api")
    .GreaterThan(c => c.Timeout, TimeSpan.Zero, "Timeout must be positive")
    .Maximum(c => c.Timeout, TimeSpan.FromMinutes(5), "Timeout cannot exceed 5 minutes")
    .ValidateOnStartup();
```

**Available methods:**
- `GreaterThan(selector, min)` - Exclusive boundary (>)
- `LessThan(selector, max)` - Exclusive boundary (<)
- `Minimum(selector, min)` - Inclusive boundary (>=)
- `Maximum(selector, max)` - Inclusive boundary (<=)
- `InRange(selector, min, max)` - Inclusive boundaries (>=, <=)

### File System Validation

```csharp
builder.Services.AddConfigKit<LoggingConfig>("Logging")
    .NotEmpty(c => c.LogDirectory, "Log directory is required")
    .DirectoryExists(c => c.LogDirectory, message: "Log directory does not exist")
    .ValidateOnStartup();
```

### Network Validation

```csharp
builder.Services.AddConfigKit<ExternalApiConfig>("ExternalApi")
    .NotEmpty(c => c.BaseUrl, "API URL is required")
    .UrlReachable(c => c.BaseUrl, timeout: TimeSpan.FromSeconds(10), message: "API is not reachable")
    .ValidateOnStartup();
```

### Security Validation

```csharp
builder.Services.AddConfigKit<SecurityConfig>("Security")
    .NotEmpty(c => c.ApiKey, "API key is required")
    .NoPlainTextSecrets(c => c.ApiKey, "API key appears to be a plain-text secret")
    .ValidateOnStartup();
```

## đź”Ą Common Scenarios

### Database Configuration

```csharp
public sealed class DatabaseConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public int MaxPoolSize { get; set; }
    public int CommandTimeoutSeconds { get; set; }
}

builder.Services.AddConfigKit<DatabaseConfig>("Database")
    .NotEmpty(c => c.ConnectionString, "Connection string is required")
    .InRange(c => c.MaxPoolSize, 1, 1000, "Max pool size must be between 1 and 1000")
    .InRange(c => c.CommandTimeoutSeconds, 1, 600, "Timeout must be between 1 and 600 seconds")
    .ValidateOnStartup();
```

### Conditional Validation

Apply validation rules based on environment or configuration values:

```csharp
builder.Services.AddConfigKit<SecurityConfig>("Security")
    .NotEmpty(c => c.Environment, "Environment is required")
    .When(c => c.Environment == "Production", b =>
    {
        // Rules only apply in production
        b.NotEmpty(c => c.CertificatePath, "Certificate is required in production")
         .FileExists(c => c.CertificatePath, message: "Certificate file not found");
    })
    .ValidateOnStartup();
```

### Cross-Property Validation

```csharp
builder.Services.AddConfigKit<DatabaseConfig>("Database")
    .NotEmpty(c => c.ConnectionString, "Connection string is required")
    .When(c => c.RequireSsl, b =>
    {
        b.MatchesPattern(c => c.ConnectionString, "Encrypt=True|Encrypt=true", 
            "SSL required but connection string does not specify Encrypt=True");
    })
    .ValidateOnStartup();
```

## đź“Š When to Use Fox.ConfigKit

### âś… Use Fox.ConfigKit when:

- You want to catch configuration errors at startup, not runtime
- You need to validate `IOptions<T>` configuration classes
- You want type-safe, fluent validation rules
- You need environment-specific validation rules
- You want to verify external dependencies (URLs, files) at startup
- You need to enforce configuration constraints across environments

### âťŚ Don't use Fox.ConfigKit when:

- You need runtime validation of user input (use FluentValidation, DataAnnotations)
- You need complex, multi-step validation workflows
- You want to validate DTOs or request models (not configuration)
- You need async validation beyond startup checks

## đź”— Related Packages

- **[Fox.ConfigKit.ResultKit](https://www.nuget.org/packages/Fox.ConfigKit.ResultKit/)** - Integration with Result pattern for functional configuration validation
- **[Fox.ResultKit](https://www.nuget.org/packages/Fox.ResultKit/)** - Lightweight Result pattern library for Railway Oriented Programming

## đź“š Full Documentation

For comprehensive documentation, advanced scenarios, and API reference, see the [GitHub repository](https://github.com/akikari/Fox.ConfigKit).

## đź“ť License

This project is licensed under the MIT License - see the [LICENSE.txt](https://github.com/akikari/Fox.ConfigKit/blob/main/LICENSE.txt) file for details.

## đź‘¤ Author

**KĂˇroly AkĂˇcz**

- GitHub: [@akikari](https://github.com/akikari)
- Repository: [Fox.ConfigKit](https://github.com/akikari/Fox.ConfigKit)

## đź™Ź Acknowledgments

Inspired by the following concepts:

- **Railway Oriented Programming** by Scott Wlaschin - Fail-fast error handling principles
- **Options Pattern** - Microsoft's recommended approach for configuration
- **Fail-Fast Philosophy** - Catch errors early to prevent cascading failures

