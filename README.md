# 🎯 Fox.ConfigKit

[![.NET](https://img.shields.io/badge/.NET-8%20%7C%209%20%7C%2010-512BD4)](https://dotnet.microsoft.com/)
[![Build and Test](https://img.shields.io/github/actions/workflow/status/akikari/Fox.ResultKit/build-and-test.yml?branch=main&label=build%20and%20test&color=darkgreen)](https://github.com/akikari/Fox.ResultKit/actions/workflows/build-and-test.yml)
[![NuGet](https://img.shields.io/nuget/v/Fox.ConfigKit.svg)](https://www.nuget.org/packages/Fox.ConfigKit/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Fox.ConfigKit?label=downloads&color=darkgreen)](https://www.nuget.org/packages/Fox.ConfigKit/)
[![License: MIT](https://img.shields.io/badge/license-MIT-orange.svg)](https://opensource.org/licenses/MIT)

> **Lightweight .NET configuration validation library with fail-fast startup validation**

Fox.ConfigKit is a lightweight library for validating `IOptions<T>` configurations at application startup. Catch configuration errors before they cause runtime failures with fluent, expressive validation rules.

## 📋 Table of Contents

- [Why Fox.ConfigKit?](#-why-foxconfigkit)
- [Features](#-features)
- [Installation](#-installation)
- [Quick Start](#-quick-start)
- [Validation Rules](#-validation-rules)
- [Advanced Scenarios](#-advanced-scenarios)
- [Sample Application](#-sample-application)
- [Design Principles](#-design-principles)
- [Requirements](#-requirements)
- [Contributing](#-contributing)
- [License](#-license)

## 🤔 Why Fox.ConfigKit?

**Traditional approach:**
```csharp
// ❌ Configuration errors discovered at runtime
public class MyService
{
    private readonly MyConfig config;

    public void DoWork()
    {
        // Crashes here if config.ApiUrl is null or invalid!
        var client = new HttpClient { BaseAddress = new Uri(config.ApiUrl) };
    }
}
```

**Fox.ConfigKit approach:**
```csharp
// ✅ Configuration errors caught at startup
builder.Services.AddConfigKit<MyConfig>("MyConfig")
    .NotEmpty(c => c.ApiUrl, "API URL is required")
    .UrlReachable(c => c.ApiUrl, message: "API is not reachable")
    .ValidateOnStartup();

// Application won't start if configuration is invalid!
```

## ✨ Features

- ✅ **Fail-Fast Validation** - Catch configuration errors at application startup
- ✅ **Fluent API** - Intuitive, type-safe configuration validation
- ✅ **Generic Type Support** - Validate any `IComparable<T>` type (int, decimal, DateTime, TimeSpan, etc.)
- ✅ **Conditional Validation** - Apply rules based on environment or other properties
- ✅ **File System Validation** - Verify directories and files exist
- ✅ **Network Validation** - Check URL reachability at startup
- ✅ **Security Validation** - Detect plain-text secrets in configuration
- ✅ **IOptions Integration** - Seamless integration with `Microsoft.Extensions.Options`
- ✅ **No Third-Party Dependencies** - Only uses core Microsoft.Extensions packages
- ✅ **Multi-Targeting** - Supports .NET 8.0, .NET 9.0, and .NET 10.0

## 📦 Installation

```bash
dotnet add package Fox.ConfigKit
```

**NuGet Package Manager:**
```
Install-Package Fox.ConfigKit
```

**PackageReference:**
```xml
<PackageReference Include="Fox.ConfigKit" Version="1.0.4" />
```

## 🚀 Quick Start

### 1. Define Your Configuration Class

```csharp
public sealed class DatabaseConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public int MaxPoolSize { get; set; }
    public int CommandTimeoutSeconds { get; set; }
}
```

### 2. Configure appsettings.json

```json
{
  "Database": {
    "ConnectionString": "Server=localhost;Database=MyDb;User Id=sa;Password=YourPassword123;",
    "MaxPoolSize": 100,
    "CommandTimeoutSeconds": 30
  },
  "Api": {
    "BaseUrl": "https://api.example.com",
    "ApiKey": "your-api-key-here",
    "TimeoutSeconds": 30,
    "MaxRetries": 3
  },
  "Logging": {
    "LogDirectory": "C:\\Logs\\MyApp",
    "MinimumLevel": "Information",
    "RetentionDays": 30
  }
}
```

### 3. Register and Validate in Program.cs

```csharp
using Fox.ConfigKit;
using Fox.ConfigKit.Validation;

var builder = WebApplication.CreateBuilder(args);

// Register configuration with validation
builder.Services.AddConfigKit<DatabaseConfig>("Database")
    .NotEmpty(c => c.ConnectionString, "Connection string is required")
    .InRange(c => c.MaxPoolSize, 1, 1000, "Max pool size must be between 1 and 1000")
    .InRange(c => c.CommandTimeoutSeconds, 1, 600, "Command timeout must be between 1 and 600 seconds")
    .ValidateOnStartup();

var app = builder.Build();
app.Run();
```

### 4. Inject and Use

```csharp
public class MyService
{
    private readonly DatabaseConfig config;

    public MyService(IOptions<DatabaseConfig> config)
    {
        this.config = config.Value; // Already validated at startup!
    }
}
```

## 📚 Validation Rules

### String Validation

| Method | Description |
|--------|-------------|
| `NotEmpty(selector, message)` | Ensures string is not null or empty |
| `NotNull(selector, message)` | Ensures value is not null |
| `MatchesPattern(selector, regex, message)` | Validates string against regex pattern |

**Example:**

```csharp
builder.Services.AddConfigKit<AppConfig>("App")
    .NotEmpty(c => c.Name, "Application name is required")
    .MatchesPattern(c => c.Version, @"^\d+\.\d+\.\d+$", "Version must be X.Y.Z format")
    .ValidateOnStartup();
```

### Comparable Value Validation

All comparison validation methods support any `IComparable<T>` type including `int`, `decimal`, `DateTime`, `TimeSpan`, and more.

| Method | Description | Boundary |
|--------|-------------|----------|
| `GreaterThan(selector, min, message)` | Ensures value is greater than minimum | Exclusive (>) |
| `LessThan(selector, max, message)` | Ensures value is less than maximum | Exclusive (<) |
| `Minimum(selector, min, message)` | Ensures value is at least minimum | Inclusive (>=) |
| `Maximum(selector, max, message)` | Ensures value is at most maximum | Inclusive (<=) |
| `InRange(selector, min, max, message)` | Ensures value is within range | Inclusive (>=, <=) |

**Example with integers:**

```csharp
builder.Services.AddConfigKit<ApiConfig>("Api")
    .GreaterThan(c => c.Port, 1024, "Port must be > 1024")
    .Maximum(c => c.Port, 65535, "Port must be <= 65535")
    .InRange(c => c.MaxRetries, 0, 10, "Max retries must be between 0 and 10")
    .ValidateOnStartup();
```

**Example with decimals:**

```csharp
builder.Services.AddConfigKit<ProductConfig>("Product")
    .Minimum(c => c.Price, 0.01m, "Price must be at least $0.01")
    .Maximum(c => c.Discount, 0.5m, "Discount cannot exceed 50%")
    .ValidateOnStartup();
```

**Example with DateTime:**

```csharp
builder.Services.AddConfigKit<CampaignConfig>("Campaign")
    .Minimum(c => c.StartDate, DateTime.Today, "Campaign must start today or later")
    .LessThan(c => c.EndDate, new DateTime(2025, 12, 31), "Campaign must end before 2025")
    .ValidateOnStartup();
```

**Example with TimeSpan:**

```csharp
builder.Services.AddConfigKit<ApiConfig>("Api")
    .GreaterThan(c => c.Timeout, TimeSpan.Zero, "Timeout must be positive")
    .Maximum(c => c.Timeout, TimeSpan.FromMinutes(5), "Timeout cannot exceed 5 minutes")
    .ValidateOnStartup();
```

### File System Validation

| Method | Description |
|--------|-------------|
| `FileExists(selector, message)` | Validates file exists at path |
| `DirectoryExists(selector, message)` | Validates directory exists at path |

**Example:**

```csharp
builder.Services.AddConfigKit<LoggingConfig>("Logging")
    .NotEmpty(c => c.LogDirectory, "Log directory is required")
    .DirectoryExists(c => c.LogDirectory, message: "Log directory does not exist")
    .ValidateOnStartup();
```

### Network Validation

| Method | Description |
|--------|-------------|
| `UrlReachable(selector, timeout, message)` | Validates URL is reachable via HTTP HEAD request |

**Example:**

```csharp
builder.Services.AddConfigKit<ExternalApiConfig>("ExternalApi")
    .NotEmpty(c => c.BaseUrl, "API URL is required")
    .UrlReachable(c => c.BaseUrl, timeout: TimeSpan.FromSeconds(10), message: "API is not reachable")
    .ValidateOnStartup();
```

### Security Validation

| Method | Description |
|--------|-------------|
| `NoPlainTextSecrets(selector, message)` | Detects plain-text secrets using pattern matching |

**Example:**

```csharp
builder.Services.AddConfigKit<ApiConfig>("Api")
    .NotEmpty(c => c.ApiKey, "API key is required")
    .NoPlainTextSecrets(c => c.ApiKey, "API key appears to be a plain-text secret")
    .ValidateOnStartup();
```

**Detected secret patterns:**
- OpenAI API keys (`sk-...`)
- Google API keys (`AIza...`)
- AWS Access Keys (`AKIA...`)
- Bearer tokens
- Generic Base64 secrets
- Hexadecimal secrets (64 chars)

### Environment Validation

| Method | Description |
|--------|-------------|
| `EnvironmentVariableExists(selector, message)` | Validates environment variable exists |

**Example:**

```csharp
builder.Services.AddConfigKit<AppConfig>("App")
    .NotEmpty(c => c.EnvironmentName, "Environment name is required")
    .EnvironmentVariableExists(c => c.RequiredEnvVar, "Required environment variable not set")
    .ValidateOnStartup();
```

## 🔥 Advanced Scenarios

### Conditional Validation

Apply validation rules conditionally based on configuration values:

```csharp
builder.Services.AddConfigKit<SecurityConfig>("Security")
    .NotEmpty(c => c.Environment, "Environment is required")
    .When(c => c.Environment == "Production", b =>
    {
        // These rules only apply in production
        b.NotEmpty(c => c.CertificatePath, "Certificate is required in production")
         .FileExists(c => c.CertificatePath, message: "Certificate file not found");
    })
    .When(c => c.RequireHttps, b =>
    {
        // These rules only apply when HTTPS is required
        b.FileExists(c => c.CertificatePath, message: "HTTPS requires certificate");
    })
    .ValidateOnStartup();
```

### Cross-Property Validation

```csharp
public sealed class DatabaseConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public bool RequireSsl { get; set; }
}

builder.Services.AddConfigKit<DatabaseConfig>("Database")
    .NotEmpty(c => c.ConnectionString, "Connection string is required")
    .When(c => c.RequireSsl, b =>
    {
        b.MatchesPattern(c => c.ConnectionString, "Encrypt=True|Encrypt=true", 
            "SSL required but connection string does not specify Encrypt=True");
    })
    .ValidateOnStartup();
```

### Environment-Specific Configuration Structures

When different environments require structurally different configurations (e.g., local authentication in DEV, OAuth in PROD/TEST), you have two approaches:

#### Approach 1: Union Configuration with Conditional Validation

Use a single configuration class with nullable properties for environment-specific fields:

```csharp
public sealed class AuthConfig
{
    public string Type { get; set; } = string.Empty; // "Local" or "OAuth"

    // DEV environment only
    public string? LocalUsername { get; set; }
    public string? LocalPassword { get; set; }

    // PROD/TEST environment only
    public string? OAuthClientId { get; set; }
    public string? OAuthClientSecret { get; set; }
    public string? OAuthAuthority { get; set; }
}

builder.Services.AddConfigKit<AuthConfig>("Auth")
    .NotEmpty(c => c.Type, "Authentication type is required")
    .When(c => c.Type == "Local", b =>
    {
        b.NotEmpty(c => c.LocalUsername, "Username required for local auth")
         .NotEmpty(c => c.LocalPassword, "Password required for local auth");
    })
    .When(c => c.Type == "OAuth", b =>
    {
        b.NotEmpty(c => c.OAuthClientId, "ClientId required for OAuth")
         .NotEmpty(c => c.OAuthClientSecret, "ClientSecret required for OAuth")
         .NotEmpty(c => c.OAuthAuthority, "Authority URL required for OAuth")
         .UrlReachable(c => c.OAuthAuthority!, TimeSpan.FromSeconds(10), 
                      "OAuth authority not reachable");
    })
    .ValidateOnStartup();
```

**Pros:**
- ✅ Single configuration class
- ✅ `appsettings.{Environment}.json` automatically overrides values
- ✅ Works with existing `When()` API

**Cons:**
- ❌ Nullable properties for environment-specific fields
- ❌ Less type-safe (union type instead of separate types)

#### Approach 2: Environment-Specific Configuration Classes

Use separate configuration classes for each environment and register conditionally:

```csharp
public interface IAuthConfig 
{ 
    string Type { get; }
}

public sealed class LocalAuthConfig : IAuthConfig
{
    public string Type => "Local";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class OAuthConfig : IAuthConfig
{
    public string Type => "OAuth";
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Authority { get; set; } = string.Empty;
}

// Program.cs - conditional registration based on environment
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddConfigKit<LocalAuthConfig>("Auth")
        .NotEmpty(c => c.Username, "Username required")
        .NotEmpty(c => c.Password, "Password required")
        .ValidateOnStartup();

    builder.Services.AddSingleton<IAuthConfig>(sp => 
        sp.GetRequiredService<IOptions<LocalAuthConfig>>().Value);
}
else // Production/Test
{
    builder.Services.AddConfigKit<OAuthConfig>("Auth")
        .NotEmpty(c => c.ClientId, "ClientId required")
        .NotEmpty(c => c.ClientSecret, "ClientSecret required")
        .NotEmpty(c => c.Authority, "Authority URL required")
        .UrlReachable(c => c.Authority, TimeSpan.FromSeconds(10), 
                     "OAuth authority not reachable")
        .ValidateOnStartup();

    builder.Services.AddSingleton<IAuthConfig>(sp => 
        sp.GetRequiredService<IOptions<OAuthConfig>>().Value);
}
```

**Pros:**
- ✅ Type-safe configuration classes without nullable properties
- ✅ Clear separation of environment-specific concerns
- ✅ Compile-time enforcement of configuration structure

**Cons:**
- ❌ More complex DI registration (if/else in startup)
- ❌ Requires separate `appsettings.{Environment}.json` files with different schemas
- ❌ Two separate validation builders

**Recommendation:** Use **Approach 1** for simple environment differences (different values). Use **Approach 2** when environments have fundamentally different authentication mechanisms or infrastructure dependencies.

### Custom Validation Rules

Create custom validation rules by inheriting from `ValidationRuleBase`:

```csharp
using Fox.ConfigKit.Validation;

internal sealed class CustomRule<T> : ValidationRuleBase, IValidationRule<T> where T : class
{
    private readonly Func<T, string?> getValue;
    private readonly string propertyName;
    private readonly string? message;

    public CustomRule(Expression<Func<T, string?>> selector, string? message = null)
    {
        this.getValue = selector.Compile();
        this.propertyName = GetPropertyName(selector);
        this.message = message;
    }

    public ConfigValidationError? Validate(T options, string sectionName)
    {
        var value = getValue(options);

        if (string.IsNullOrEmpty(value))
        {
            var key = $"{sectionName}:{propertyName}";
            var errorMessage = message ?? $"{propertyName} is invalid";
            return new ConfigValidationError(key, errorMessage, value, ["Provide a valid value"]);
        }

        return null;
    }
}

// Extension method
public static class CustomValidationExtensions
{
    public static ConfigValidationBuilder<T> MyCustomValidation<T>(
        this ConfigValidationBuilder<T> builder,
        Expression<Func<T, string?>> selector,
        string? message = null) where T : class
    {
        return builder.AddRule(new CustomRule<T>(selector, message));
    }
}
```

## 🚂 Result Pattern Integration (Optional)

For applications using [Fox.ResultKit](https://github.com/akikari/Fox.ResultKit), the **Fox.ConfigKit.ResultKit** integration package enables functional-style configuration validation with Railway Oriented Programming.

### Installation

```bash
dotnet add package Fox.ConfigKit.ResultKit
```

### Functional Validation (Alternative to ValidateOnStartup)

Instead of exception-based validation at startup, use `Result<T>` for explicit error handling:

```csharp
using Fox.ConfigKit.ResultKit;
using Fox.ResultKit;

var builder = WebApplication.CreateBuilder(args);

// Register configuration WITHOUT ValidateOnStartup()
builder.Services.AddConfigKit<ApplicationConfig>("Application")
    .NotEmpty(c => c.Name, "Application name is required")
    .Minimum(c => c.MaxConcurrentRequests, 1)
    .Maximum(c => c.MaxConcurrentRequests, 1000);
    // ← No .ValidateOnStartup() call!

builder.Services.AddConfigKit<DatabaseConfig>("Database")
    .NotEmpty(c => c.ConnectionString);

var app = builder.Build();

// Explicit validation with Result pattern
var appConfig = app.Services.GetRequiredService<IOptions<ApplicationConfig>>().Value;
var dbConfig = app.Services.GetRequiredService<IOptions<DatabaseConfig>>().Value;

// Composable validation pipeline
var startupResult = ConfigValidator.Validate<ApplicationConfig>()
    .NotEmpty(c => c.Name, "Application name is required")
    .Minimum(c => c.MaxConcurrentRequests, 1)
    .ToResult(appConfig)
    .Bind(_ => ConfigValidator.Validate<DatabaseConfig>()
        .NotEmpty(c => c.ConnectionString, "Connection string required")
        .ToResult(dbConfig));

// Functional error handling
startupResult.Match(
    onSuccess: _ => 
    {
        app.Logger.LogInformation("All configurations valid");
        app.Run();
    },
    onFailure: error => 
    {
        app.Logger.LogCritical("Configuration validation failed: {Error}", error.Message);
        Environment.Exit(1);
    }
);
```

### Collecting All Validation Errors

Use `ToErrorsResult()` to collect **all** validation errors instead of stopping at the first one:

```csharp
var validationResult = ConfigValidator.Validate<ApplicationConfig>()
    .NotEmpty(c => c.Name, "Name is required")
    .InRange(c => c.MaxConcurrentRequests, 1, 1000)
    .NotEmpty(c => c.Version, "Version is required")
    .ToErrorsResult(appConfig);

if (validationResult.IsFailure)
{
    app.Logger.LogCritical("Configuration has {Count} errors:", validationResult.Errors.Count);

    foreach (var error in validationResult.Errors)
    {
        app.Logger.LogError("  - {ErrorMessage}", error.Message);
    }

    Environment.Exit(1);
}

app.Run();
```

### When to Use Result Pattern vs ValidateOnStartup

| Scenario | Use `ValidateOnStartup()` | Use `ToResult()` / Result Pattern |
|----------|--------------------------|----------------------------------|
| **Simple ASP.NET Core app** | ✅ Recommended (fail-fast) | ❌ Overkill |
| **Console/Worker apps** | ❌ No DI startup | ✅ Recommended |
| **Complex startup logic** | 🟡 Limited control | ✅ Full control over errors |
| **Multiple configs to validate** | 🟡 Sequential exceptions | ✅ Composable chain with `Bind()` |
| **Need all errors at once** | ❌ Stops at first error | ✅ `ToErrorsResult()` collects all |
| **Unit testing validation** | 🟡 Exception-based asserts | ✅ Result-based asserts |
| **Graceful shutdown on error** | ❌ Exception crash | ✅ `Environment.Exit(1)` |
| **Already using Railway pattern** | 🟡 Inconsistent style | ✅ Consistent functional style |

### Standalone Validation (Without DI)

For console apps, scripts, or unit tests where DI is not available:

```csharp
using Fox.ConfigKit.ResultKit;

// Load configuration from file/environment
var config = LoadConfigurationFromFile("appsettings.json");

// Validate without dependency injection
var result = ConfigValidator.Validate<AppConfig>()
    .NotEmpty(c => c.Name, "Name is required")
    .InRange(c => c.Port, 1, 65535, "Invalid port")
    .ToResult(config);

return result.Match(
    onSuccess: validConfig => RunApplication(validConfig),
    onFailure: error => 
    {
        Console.WriteLine($"Configuration error: {error}");
        return 1; // Exit code
    }
);
```

**Learn more:** See the [Fox.ConfigKit.ResultKit README](https://github.com/akikari/Fox.ConfigKit/tree/main/src/Fox.ConfigKit.ResultKit) for detailed examples and advanced patterns.

## 📖 Sample Application

A comprehensive sample WebApi application is available in the repository demonstrating:

- ✅ Application-wide configuration validation
- ✅ Database connection string validation
- ✅ External API configuration with URL reachability checks
- ✅ File system validation for log directories
- ✅ Environment-based conditional validation
- ✅ Security configuration with certificate validation

**Run the sample:**

```bash
cd samples/Fox.ConfigKit.Samples.WebApi
dotnet run
```

**Explore:**
- Open `https://localhost:5001/swagger`
- View validated configurations via `/api/configuration/*` endpoints
- See [sample README](samples/Fox.ConfigKit.Samples.WebApi/README.md) for details

## 🎯 Design Principles

1. **Fail-Fast** - Catch configuration errors at startup, not at runtime
2. **Explicit Validation** - Make validation rules clear and discoverable
3. **Type-Safe** - Leverage C# type system for compile-time safety
4. **Fluent API** - Intuitive, chainable validation rules
5. **No Third-Party Dependencies** - Only core Microsoft.Extensions packages, no external libraries
6. **Developer-Friendly** - Clear error messages, excellent IntelliSense

## 🔧 Requirements

- .NET 8.0 or higher
- C# 12 or higher (for modern language features)
- Nullable reference types enabled (recommended)

## 🤝 Contributing

**Fox.ConfigKit is intentionally lightweight and feature-focused.** The goal is to remain a simple library with minimal dependencies for configuration validation.

### What We Welcome

- ✅ **Bug fixes** - Issues with existing functionality
- ✅ **Documentation improvements** - Clarifications, examples, typo fixes
- ✅ **Performance optimizations** - Without breaking API compatibility
- ✅ **New validation rules** - Following existing patterns

### What We Generally Do Not Accept

- ❌ New dependencies or third-party packages
- ❌ Large feature additions that increase complexity
- ❌ Breaking API changes

If you want to propose a significant change, please open an issue first to discuss whether it aligns with the project's philosophy.

### Build Policy

The project enforces a **strict build policy** to ensure code quality:

- ❌ **No errors allowed** - Build must be error-free
- ❌ **No warnings allowed** - All compiler warnings must be resolved
- ❌ **No messages allowed** - Informational messages must be suppressed or addressed

All pull requests must pass this requirement.

### Code Style

- Follow the existing code style (see `.github/copilot-instructions.md`)
- Use file-scoped namespaces
- Enable nullable reference types
- Add XML documentation for public APIs
- Write unit tests for new features
- Use expression-bodied members for simple properties
- Auto-properties preferred over backing fields

### How to Contribute

1. Fork the repository
2. Create a feature branch from `main`
3. Follow the coding standards in `.github/copilot-instructions.md`
4. Ensure all tests pass (`dotnet test`)
5. Submit a pull request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

## 👤 Author

**Károly Akácz**

- GitHub: [@akikari](https://github.com/akikari)
- Repository: [Fox.ConfigKit](https://github.com/akikari/Fox.ConfigKit)

## 📊 Project Status

[![NuGet Version](https://img.shields.io/nuget/v/Fox.ConfigKit.svg?label=version&color=blue)](https://www.nuget.org/packages/Fox.ConfigKit/)

See [CHANGELOG.md](CHANGELOG.md) for version history.

## 🔗 Related Projects

- [Fox.ResultKit](https://github.com/akikari/Fox.ResultKit) - Lightweight Result pattern library for Railway Oriented Programming
- [Fox.ConfigKit.ResultKit](https://www.nuget.org/packages/Fox.ConfigKit.ResultKit) - Integration package for using ConfigKit with Result pattern

## 📞 Support

For issues, questions, or feature requests, please open an issue in the [GitHub repository](https://github.com/akikari/Fox.ConfigKit/issues).

