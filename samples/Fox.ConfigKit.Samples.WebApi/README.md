# 🎯 Fox.ConfigKit Sample - WebApi

> **Real-world demonstration of configuration validation with fail-fast startup in ASP.NET Core**

This sample application demonstrates how to use **Fox.ConfigKit** to validate application configurations at startup, catching errors before they cause runtime issues.

## 📋 Table of Contents

- [Overview](#-overview)
- [Quick Start](#-quick-start)
- [Project Structure](#-project-structure)
- [Configuration Examples](#-configuration-examples)
- [API Endpoints](#-api-endpoints)
- [Fail-Fast Behavior](#-fail-fast-behavior)
- [Testing Scenarios](#-testing-scenarios)
- [Real-World Benefits](#-real-world-benefits)

## 🎯 Overview

This sample shows how to:

- ✅ **Configure and validate** application settings at startup
- ✅ **Use fail-fast validation** to catch configuration errors early
- ✅ **Apply conditional validation** rules based on environment
- ✅ **Validate file system paths**, URLs, and security settings
- ✅ **Use validated configurations** in controllers
- ✅ **Detect plain-text secrets** in configuration values

## 🚀 Quick Start

### Prerequisites

- .NET 8.0 SDK or later
- Valid log directory (create `C:\Logs\ConfigKitSample` on Windows or update path in appsettings)

### Important Notes

⚠️ **Log Directory:** The sample validates that log directories exist at startup. You **must create the log directory manually** before running the application, or it will fail at startup with a validation error.

⚠️ **External API URL:** The `ExternalApi.BaseUrl` is set to `https://api-dev.example.com` (a non-existent example URL). The application will fail at startup because the URL is not reachable. This demonstrates the `UrlReachable()` validation. To make the sample run successfully:
- Comment out the `.UrlReachable()` line in `Program.cs` (line 33), OR
- Change the URL to a real, reachable API (e.g., `https://jsonplaceholder.typicode.com`)

### Steps

1. **Create log directory** (Windows)
   ```powershell
   New-Item -Path "C:\Logs\ConfigKitSample\Dev" -ItemType Directory -Force
   ```

   Or (Linux/macOS)
   ```bash
   mkdir -p ~/logs/ConfigKitSample/Dev
   ```

2. **Fix External API URL** (choose one option)
   - Option A: Comment out `.UrlReachable()` in `Program.cs`
   - Option B: Change `BaseUrl` in `appsettings.Development.json` to a real URL

3. **Run the application**
   ```bash
   cd samples/Fox.ConfigKit.Samples.WebApi
   dotnet run
   ```

4. **Access Swagger UI**
   ```
   https://localhost:5001/swagger
   ```

5. **Explore the `/api/configuration/*` endpoints**

## 📁 Project Structure

```
Configuration/
├── ApplicationConfig.cs       # Application-wide settings (Minimum/Maximum validation)
├── DatabaseConfig.cs          # Database connection settings (InRange validation)
├── ExternalApiConfig.cs       # External API integration (GreaterThan/LessThan validation)
├── LoggingConfig.cs           # Custom logging configuration (File system validation)
├── SecurityConfig.cs          # Security and SSL/TLS settings (Conditional validation)
└── CampaignConfig.cs          # Marketing campaign with generic types (decimal, DateTime, TimeSpan)

Controllers/
└── ConfigurationController.cs # Endpoints to view validated configurations

appsettings.json              # Base configuration
appsettings.Development.json  # Development-specific overrides
appsettings.Production.json   # Production configuration example
Program.cs                    # Startup with Fox.ConfigKit validation (6 examples)
```

## 🔧 Configuration Examples

### 1. Application Configuration - Minimum/Maximum (Inclusive Boundaries)

**Demonstrates:** `Minimum()`, `Maximum()` with inclusive boundaries (>=, <=)

```csharp
builder.Services.AddConfigKit<ApplicationConfig>("Application")
    .NotEmpty(c => c.Name, "Application name is required")
    .MatchesPattern(c => c.Version, @"^\d+\.\d+\.\d+$", "Version must be in format X.Y.Z")
    .Minimum(c => c.MaxConcurrentRequests, 1, "Max concurrent requests must be at least 1")
    .Maximum(c => c.MaxConcurrentRequests, 1000, "Max concurrent requests cannot exceed 1000")
    .ValidateOnStartup();
```

**Configuration class:**

```csharp
public sealed class ApplicationConfig
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public int MaxConcurrentRequests { get; set; }
    public int RequestTimeoutSeconds { get; set; }
    public bool EnableMetrics { get; set; }
}
```

**appsettings.json:**

```json
{
  "Application": {
    "Name": "Fox.ConfigKit Sample API",
    "Version": "1.0.0",
    "MaxConcurrentRequests": 100,
    "RequestTimeoutSeconds": 30,
    "EnableMetrics": true
  }
}
```

### 2. Database Configuration - InRange (Inclusive Boundaries)

**Demonstrates:** Traditional `InRange()` validation with inclusive boundaries

```csharp
builder.Services.AddConfigKit<DatabaseConfig>("Database")
    .NotEmpty(c => c.ConnectionString, "Database connection string is required")
    .InRange(c => c.CommandTimeoutSeconds, 1, 600, "Command timeout must be between 1 and 600 seconds")
    .InRange(c => c.MaxPoolSize, 1, 1000, "Max pool size must be between 1 and 1000")
    .ValidateOnStartup();
```

**Configuration class:**

```csharp
public sealed class DatabaseConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeoutSeconds { get; set; }
    public int MaxPoolSize { get; set; }
    public bool EnableSensitiveDataLogging { get; set; }
    public bool RequireSsl { get; set; }
}
```

**appsettings.json:**

```json
{
  "Database": {
    "ConnectionString": "Server=localhost;Database=SampleDb;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True",
    "CommandTimeoutSeconds": 30,
    "MaxPoolSize": 100,
    "EnableSensitiveDataLogging": false,
    "RequireSsl": false
  }
}
```

### 3. External API Configuration - GreaterThan/LessThan (Exclusive Boundaries)

**Demonstrates:** `GreaterThan()`, `LessThan()` with exclusive boundaries (>, <)

```csharp
builder.Services.AddConfigKit<ExternalApiConfig>("ExternalApi")
    .NotEmpty(c => c.BaseUrl, "External API base URL is required")
    .NotEmpty(c => c.ApiKey, "External API key is required")
    .NoPlainTextSecrets(c => c.ApiKey, "API key appears to be a plain-text secret")
    .GreaterThan(c => c.TimeoutSeconds, 0, "API timeout must be greater than 0")
    .LessThan(c => c.TimeoutSeconds, 600, "API timeout must be less than 600 seconds")
    .ValidateOnStartup();
```

**Configuration class:**

```csharp
public sealed class ExternalApiConfig
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; }
    public int MaxRetries { get; set; }
}
```

**appsettings.json:**

```json
{
  "ExternalApi": {
    "BaseUrl": "https://api.example.com",
    "ApiKey": "your-api-key-here",
    "TimeoutSeconds": 30,
    "MaxRetries": 3
  }
}
```

### 4. Logging Configuration - File System Validation

**Demonstrates:** `DirectoryExists()` with fail-fast behavior

```csharp
builder.Services.AddConfigKit<LoggingConfig>("CustomLogging")
    .NotEmpty(c => c.LogDirectory, "Log directory path is required")
    .DirectoryExists(c => c.LogDirectory, message: "Log directory does not exist")
    .InRange(c => c.RetentionDays, 1, 365, "Retention days must be between 1 and 365")
    .ValidateOnStartup();
```

**Configuration class:**

```csharp
public sealed class LoggingConfig
{
    public string LogDirectory { get; set; } = string.Empty;
    public string MinimumLevel { get; set; } = "Information";
    public int RetentionDays { get; set; }
    public int MaxFileSizeMB { get; set; }
}
```

**appsettings.json:**

```json
{
  "CustomLogging": {
    "LogDirectory": "C:\\Logs\\ConfigKitSample",
    "MinimumLevel": "Information",
    "RetentionDays": 30,
    "MaxFileSizeMB": 100
  }
}
```

### 5. Security Configuration - Conditional Validation

**Demonstrates:** `When()` for environment-specific rules

```csharp
builder.Services.AddConfigKit<SecurityConfig>("Security")
    .NotEmpty(c => c.Environment, "Environment is required")
    .When(c => c.Environment == "Production", b =>
    {
        b.NotEmpty(c => c.CertificatePath, "Certificate path is required in production")
         .FileExists(c => c.CertificatePath, message: "Certificate file does not exist");
    })
    .ValidateOnStartup();
```

**Configuration class:**

```csharp
public sealed class SecurityConfig
{
    public string Environment { get; set; } = "Development";
    public string CertificatePath { get; set; } = string.Empty;
    public string CertificatePassword { get; set; } = string.Empty;
    public bool RequireHttps { get; set; }
    public string[] AllowedOrigins { get; set; } = [];
}
```

**appsettings.json:**

```json
{
  "Security": {
    "Environment": "Development",
    "CertificatePath": "",
    "CertificatePassword": "",
    "RequireHttps": false,
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:4200"
    ]
  }
}
```

### 6. Campaign Configuration - Generic Types (decimal, DateTime, TimeSpan)

**Demonstrates:** Generic validation with `decimal`, `DateTime`, and `TimeSpan`

```csharp
builder.Services.AddConfigKit<CampaignConfig>("Campaign")
    .NotEmpty(c => c.Name, "Campaign name is required")
    .Minimum(c => c.StartDate, DateTime.Today, "Campaign must start today or later")
    .GreaterThan(c => c.EndDate, DateTime.Today, "Campaign end date must be in the future")
    .Minimum(c => c.MinimumPurchaseAmount, 0.01m, "Minimum purchase amount must be at least $0.01")
    .Maximum(c => c.MaximumDiscountPercentage, 0.75m, "Discount percentage cannot exceed 75%")
    .GreaterThan(c => c.EmailReminderInterval, TimeSpan.Zero, "Email reminder interval must be positive")
    .Maximum(c => c.CacheDuration, TimeSpan.FromHours(24), "Cache duration cannot exceed 24 hours")
    .ValidateOnStartup();
```

**Configuration class:**

```csharp
public sealed class CampaignConfig
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal MinimumPurchaseAmount { get; set; }
    public decimal MaximumDiscountPercentage { get; set; }
    public TimeSpan EmailReminderInterval { get; set; }
    public TimeSpan CacheDuration { get; set; }
}
```

**appsettings.json:**

```json
{
  "Campaign": {
    "Name": "Summer Sale 2024",
    "StartDate": "2024-06-01T00:00:00",
    "EndDate": "2024-08-31T23:59:59",
    "MinimumPurchaseAmount": 25.00,
    "MaximumDiscountPercentage": 0.30,
    "EmailReminderInterval": "7.00:00:00",
    "CacheDuration": "01:00:00"
  }
}
```

## 🌐 API Endpoints

| Endpoint | Description |
|----------|-------------|
| `GET /api/configuration/application` | View application configuration |
| `GET /api/configuration/database` | View database configuration (sensitive data hidden) |
| `GET /api/configuration/external-api` | View external API configuration |
| `GET /api/configuration/logging` | View logging configuration |
| `GET /api/configuration/security` | View security configuration |
| `GET /api/configuration/campaign` | View campaign configuration |
| `GET /api/configuration/all` | View summary of all configurations |

## ⚡ Fail-Fast Behavior

When you start the application, **Fox.ConfigKit validates all configurations immediately**. If any validation fails, the application **will not start** and you'll see detailed error messages:

```
Unhandled exception. Microsoft.Extensions.Options.OptionsValidationException:
  - Database connection string is required
  - Log directory does not exist
  - External API base URL is not reachable
```

### Why Fail-Fast?

✅ **Catch errors at startup**, not at runtime  
✅ **No partial deployments** with broken configurations  
✅ **Clear error messages** for DevOps teams  
✅ **Prevent production incidents** caused by misconfiguration

## 🧪 Testing Scenarios

### Scenario 1: Missing Configuration

**Test:** Remove the `Database.ConnectionString` from `appsettings.json`

**Expected result:**
```
❌ Application fails to start with error: "Database connection string is required"
```

### Scenario 2: Invalid Range

**Test:** Set `Application.MaxConcurrentRequests` to `2000` (exceeds limit of 1000)

**Expected result:**
```
❌ Application fails to start with error: "Max concurrent requests must be between 1 and 1000"
```

### Scenario 3: Missing Directory

**Test:** Set `CustomLogging.LogDirectory` to a non-existent path

**Expected result:**
```
❌ Application fails to start with error: "Log directory does not exist"
```

### Scenario 4: Production Environment

**Test:** Set `Security.Environment` to `"Production"` but leave `CertificatePath` empty

**Expected result:**
```
❌ Application fails to start with error: "Certificate path is required in production"
```

### Scenario 5: URL Unreachable

**Test:** Set `ExternalApi.BaseUrl` to an unreachable URL

**Expected result:**
```
❌ Application fails to start with error: "External API base URL is not reachable"
```

## 💡 Real-World Benefits

| Benefit | Description |
|---------|-------------|
| **Early Error Detection** | Find configuration issues before deployment |
| **Security** | Detect plain-text secrets in configuration |
| **Infrastructure Validation** | Verify files, directories, and URLs exist |
| **Environment-Specific Rules** | Different validation for dev/staging/prod |
| **Living Documentation** | Configuration rules serve as living documentation |
| **DevOps Friendly** | Clear error messages for deployment automation |

## 🎯 Key Validation Features Demonstrated

| Feature | Example in Sample |
|---------|-------------------|
| **String Validation** | `NotEmpty`, `MatchesPattern` |
| **Integer Validation** | `Minimum`, `Maximum`, `GreaterThan`, `LessThan`, `InRange` |
| **Decimal Validation** | `Minimum` for prices, `Maximum` for percentages |
| **DateTime Validation** | `Minimum` for campaign dates, `GreaterThan` for future dates |
| **TimeSpan Validation** | `GreaterThan` for positive intervals, `Maximum` for durations |
| **File System** | `DirectoryExists`, `FileExists` |
| **Network** | `UrlReachable` |
| **Security** | `NoPlainTextSecrets` |
| **Conditional Logic** | `When` for environment-based rules |

## 📚 Learn More

- [Fox.ConfigKit GitHub Repository](https://github.com/akikari/Fox.ConfigKit)
- [Fox.ConfigKit NuGet Package](https://www.nuget.org/packages/Fox.ConfigKit)
- [Main Documentation](https://github.com/akikari/Fox.ConfigKit/blob/main/README.md)

---

**Built with Fox.ConfigKit** - Lightweight .NET configuration validation library

