# Fox.ConfigKit.ResultKit

> Integration package combining Fox.ConfigKit configuration validation with Fox.ResultKit Railway Oriented Programming

This package bridges Fox.ConfigKit's configuration validation with Fox.ResultKit's Result pattern, enabling functional-style configuration validation workflows.

##  Installation

```bash
dotnet add package Fox.ConfigKit.ResultKit
```

**NuGet Package Manager:**
```
Install-Package Fox.ConfigKit.ResultKit
```

**PackageReference:**
```xml
<PackageReference Include="Fox.ConfigKit.ResultKit" Version="1.0.0" />
```

## Core Concepts

### Configuration Validation with Result Pattern

Validate configurations and return `Result<T>` for functional composition:

```csharp
using Fox.ConfigKit.ResultKit;
using Fox.ResultKit;

// Standalone validation without dependency injection
var result = ConfigValidator.Validate<DatabaseConfig>()
    .NotEmpty(c => c.ConnectionString, "Connection string is required")
    .InRange(c => c.MaxPoolSize, 1, 1000, "Max pool size must be between 1 and 1000")
    .ToResult(databaseConfig);

// Chain with other operations
var finalResult = result
    .Bind(config => DatabaseService.Connect(config))
    .Bind(connection => connection.ExecuteQuery())
    .Match(
        onSuccess: data => $"Query returned {data.Count} records",
        onFailure: error => $"Failed: {error}"
    );
```

**Note:** `ConfigValidator.Validate<T>()` is useful for:
- Console applications without dependency injection
- Unit tests requiring standalone validation
- Functional composition with other Result-based operations
- Scripts or utilities that need configuration validation

For ASP.NET Core applications with DI, use `AddConfigKit<T>()` with `ValidateOnStartup()` instead.

### Startup Validation with Result

Validate configurations at startup and handle failures gracefully:

```csharp
var configValidation = ConfigValidator.Validate<ApiConfig>()
    .NotEmpty(c => c.BaseUrl, "API URL is required")
    .NotEmpty(c => c.ApiKey, "API key is required")
    .ToResult(apiConfig);

if (configValidation.IsFailure)
{
    var errors = configValidation.Error;
    throw new InvalidOperationException($"Configuration validation failed: {errors}");
}
```

##  Key Usage Patterns

### Functional Configuration Validation

```csharp
using Fox.ConfigKit.ResultKit;
using Fox.ResultKit;

public Result<AppConfig> ValidateAppConfiguration(AppConfig config)
{
    return ConfigValidator.Validate<AppConfig>()
        .NotEmpty(c => c.Name, "Application name is required")
        .MatchesPattern(c => c.Version, @"^\d+\.\d+\.\d+$", "Version must be X.Y.Z format")
        .InRange(c => c.Port, 1, 65535, "Port must be between 1 and 65535")
        .ToResult(config);
}

// Usage with Railway Oriented Programming
var result = ValidateAppConfiguration(appConfig)
    .Bind(config => InitializeApplication(config))
    .Bind(app => app.Start())
    .Match(
        onSuccess: app => $"Application {app.Name} started successfully",
        onFailure: error => $"Startup failed: {error}"
    );
```

### Conditional Validation with Results

```csharp
public Result<SecurityConfig> ValidateSecurityConfig(SecurityConfig config, string environment)
{
    var builder = ConfigValidator.Validate<SecurityConfig>()
        .NotEmpty(c => c.Environment, "Environment is required");

    if (environment == "Production")
    {
        builder = builder
            .NotEmpty(c => c.CertificatePath, "Certificate is required in production")
            .FileExists(c => c.CertificatePath, message: "Certificate file not found");
    }

    return builder.ToResult(config);
}
```

### Aggregating Multiple Configuration Results

```csharp
public Result<AppSettings> ValidateAllConfigurations(
    DatabaseConfig dbConfig,
    ApiConfig apiConfig,
    LoggingConfig loggingConfig)
{
    var dbResult = ConfigValidator.Validate<DatabaseConfig>()
        .NotEmpty(c => c.ConnectionString, "DB connection string is required")
        .ToValidationResult(dbConfig);

    var apiResult = ConfigValidator.Validate<ApiConfig>()
        .NotEmpty(c => c.BaseUrl, "API URL is required")
        .ToValidationResult(apiConfig);

    var loggingResult = ConfigValidator.Validate<LoggingConfig>()
        .DirectoryExists(c => c.LogDirectory, message: "Log directory not found")
        .ToValidationResult(loggingConfig);

    // Fail-fast: stop at first error
    return dbResult
        .Bind(() => apiResult)
        .Bind(() => loggingResult)
        .Bind(() => Result<AppSettings>.Success(new AppSettings(dbConfig, apiConfig, loggingConfig)));
}
```

##  Common Scenarios

### Startup Validation Pipeline

```csharp
var startupResult = ConfigValidator.Validate<DatabaseConfig>()
    .NotEmpty(c => c.ConnectionString, "Connection string is required")
    .InRange(c => c.MaxPoolSize, 1, 1000, "Max pool size must be between 1 and 1000")
    .ToResult(dbConfig)
    .BindAsync(config => DatabaseService.TestConnection(config))
    .BindAsync(connection => MigrationService.EnsureSchemaUpToDate(connection))
    .Match(
        onSuccess: _ => "Database ready",
        onFailure: error => throw new InvalidOperationException($"Startup failed: {error}")
    );
```

### Environment-Specific Validation

```csharp
public Result<SecurityConfig> ValidateForEnvironment(SecurityConfig config, IHostEnvironment env)
{
    var validator = ConfigValidator.Validate<SecurityConfig>();

    if (env.IsProduction())
    {
        validator = validator
            .NotEmpty(c => c.CertificatePath, "Certificate is required in production")
            .FileExists(c => c.CertificatePath, message: "Certificate not found")
            .NoPlainTextSecrets(c => c.ApiKey, "Plain-text secrets not allowed in production");
    }
    else if (env.IsDevelopment())
    {
        validator = validator
            .NotEmpty(c => c.ApiKey, "API key is required");
    }

    return validator.ToResult(config);
}
```

### Collecting All Errors

```csharp
// Use ToErrorsResult() to collect all validation errors instead of stopping at first error
var validationResult = ConfigValidator.Validate<AppConfig>()
    .NotEmpty(c => c.Name, "Name is required")
    .InRange(c => c.Port, 1, 65535, "Invalid port")
    .NotEmpty(c => c.ApiKey, "API key is required")
    .ToErrorsResult(config);

if (validationResult.IsFailure)
{
    foreach (var error in validationResult.Errors)
    {
        Console.WriteLine($"Validation error: {error}");
    }
}
```

##  When to Use Fox.ConfigKit.ResultKit

###  Use Fox.ConfigKit.ResultKit when:

- You're already using Fox.ResultKit for Railway Oriented Programming
- You want functional-style configuration validation
- You need to compose configuration validation with other Result-based operations
- You want to aggregate multiple configuration validation results
- You prefer explicit error handling over exceptions
- You want to chain configuration validation with application startup logic

###  Don't use Fox.ConfigKit.ResultKit when:

- You don't use Fox.ResultKit or Result pattern
- You prefer exception-based error handling
- You only need simple startup validation (use Fox.ConfigKit directly with `ValidateOnStartup`)
- You're not familiar with Railway Oriented Programming

##  Standalone vs. DI Usage

### Standalone Validation (ConfigValidator)

Use `ConfigValidator.Validate<T>()` for:

```csharp
//  Console applications
static void Main(string[] args)
{
    var config = LoadConfiguration();
    var result = ConfigValidator.Validate<AppConfig>()
        .NotEmpty(c => c.Name, "Name is required")
        .ToResult(config);

    if (result.IsFailure)
    {
        Console.WriteLine($"Config error: {result.Error}");
        return;
    }
}

//  Unit tests
[Fact]
public void Configuration_should_be_valid()
{
    var config = new DatabaseConfig { ConnectionString = "..." };
    var result = ConfigValidator.Validate<DatabaseConfig>()
        .NotEmpty(c => c.ConnectionString, "Required")
        .ToResult(config);

    result.IsSuccess.Should().BeTrue();
}

//  Functional composition
public Result<Database> InitializeDatabase(DatabaseConfig config)
{
    return ConfigValidator.Validate<DatabaseConfig>()
        .NotEmpty(c => c.ConnectionString, "Connection string required")
        .ToResult(config)
        .Bind(cfg => Database.Connect(cfg));
}
```

### DI-Based Validation (AddConfigKit)

Use `AddConfigKit<T>()` with `ValidateOnStartup()` for ASP.NET Core:

```csharp
//  ASP.NET Core applications with dependency injection
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConfigKit<DatabaseConfig>("Database")
    .NotEmpty(c => c.ConnectionString, "Connection string is required")
    .InRange(c => c.MaxPoolSize, 1, 1000, "Pool size must be between 1 and 1000")
    .ValidateOnStartup();

var app = builder.Build(); //  Validation happens here (fail-fast)
app.Run();
```

**Key Differences:**

| Feature | `ConfigValidator.Validate<T>()` | `AddConfigKit<T>()` |
|---------|--------------------------------|---------------------|
| **Use Case** | Standalone, functional, testing | ASP.NET Core with DI |
| **Returns** | `ConfigValidationBuilder<T>` | `ConfigValidationBuilder<T>` |
| **Validation Timing** | Explicit (when you call `.ToResult()`) | Automatic at startup |
| **Result Type** | `Result<T>`, `ErrorsResult` | Exception on failure |
| **DI Required** |  No |  Yes |
| **IOptions Integration** |  No |  Yes |

##  Related Packages

- **[Fox.ConfigKit](https://www.nuget.org/packages/Fox.ConfigKit/)** - Core configuration validation library
- **[Fox.ResultKit](https://www.nuget.org/packages/Fox.ResultKit/)** - Lightweight Result pattern library for Railway Oriented Programming

##  Full Documentation

For comprehensive documentation, advanced scenarios, and API reference, see the [GitHub repository](https://github.com/akikari/Fox.ConfigKit).

##  License

This project is licensed under the MIT License - see the [LICENSE.txt](https://github.com/akikari/Fox.ConfigKit/blob/main/LICENSE.txt) file for details.

##  Author

**Károly Akácz**

- GitHub: [@akikari](https://github.com/akikari)
- Repository: [Fox.ConfigKit](https://github.com/akikari/Fox.ConfigKit)

##  Acknowledgments

Inspired by:

- **Railway Oriented Programming** by Scott Wlaschin - Functional error handling principles
- **F# Result Type** - The foundation for functional error handling
- **Options Pattern** - Microsoft's recommended approach for configuration

