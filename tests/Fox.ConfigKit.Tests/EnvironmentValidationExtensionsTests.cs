//==================================================================================================
// Tests for environment-specific validation extension methods.
// Verifies environment-conditional rules (Development, Production, Staging).
//==================================================================================================
using FluentAssertions;
using Fox.ConfigKit.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ConfigKit.Tests;

//==================================================================================================
/// <summary>
/// Tests for <see cref="EnvironmentValidationExtensions"/>.
/// </summary>
//==================================================================================================
public sealed class EnvironmentValidationExtensionsTests
{
    #region Test Classes

    private sealed class ApiConfig
    {
        public string? ApiKey { get; set; }
        public string? DebugMode { get; set; }
        public int Timeout { get; set; }
    }

    #endregion

    #region WhenEnvironment Tests

    //==============================================================================================
    /// <summary>
    /// WhenEnvironment() should apply rules when environment matches.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WhenEnvironment_should_apply_rules_when_environment_matches()
    {
        var originalEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        try
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

            var services = new ServiceCollection();
            var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

            builder.WhenEnvironment("Testing", test => test
                .NotNull(o => o.ApiKey, "API key required in Testing"));

            var errors = builder.Validate(new ApiConfig { ApiKey = null });

            errors.Should().ContainSingle()
                .Which.Message.Should().Contain("API key required in Testing");
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalEnv);
        }
    }

    //==============================================================================================
    /// <summary>
    /// WhenEnvironment() should skip rules when environment does not match.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WhenEnvironment_should_skip_rules_when_environment_does_not_match()
    {
        var originalEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        try
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");

            var services = new ServiceCollection();
            var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

            builder.WhenEnvironment("Development", dev => dev
                .NotNull(o => o.ApiKey, "API key required in Development"));

            var errors = builder.Validate(new ApiConfig { ApiKey = null });

            errors.Should().BeEmpty();
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalEnv);
        }
    }

    //==============================================================================================
    /// <summary>
    /// WhenEnvironment() should be case-insensitive.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WhenEnvironment_should_be_case_insensitive()
    {
        var originalEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        try
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "PRODUCTION");

            var services = new ServiceCollection();
            var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

            builder.WhenEnvironment("production", prod => prod
                .NotNull(o => o.ApiKey, "API key required"));

            var errors = builder.Validate(new ApiConfig { ApiKey = null });

            errors.Should().ContainSingle();
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalEnv);
        }
    }

    //==============================================================================================
    /// <summary>
    /// WhenEnvironment() should default to Production when no environment is set.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WhenEnvironment_should_default_to_production_when_no_environment_set()
    {
        var originalAspNetCore = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var originalDotNet = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        try
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);

            var services = new ServiceCollection();
            var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

            builder.WhenEnvironment("Production", prod => prod
                .NotNull(o => o.ApiKey, "API key required in Production"));

            var errors = builder.Validate(new ApiConfig { ApiKey = null });

            errors.Should().ContainSingle();
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalAspNetCore);
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", originalDotNet);
        }
    }

    //==============================================================================================
    /// <summary>
    /// WhenEnvironment() should throw ArgumentNullException when builder is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WhenEnvironment_should_throw_when_builder_is_null()
    {
        ConfigValidationBuilder<ApiConfig> builder = null!;

        var act = () => builder.WhenEnvironment("Development", dev => dev.NotNull(o => o.ApiKey));

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("builder");
    }

    //==============================================================================================
    /// <summary>
    /// WhenEnvironment() should throw ArgumentNullException when environmentName is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WhenEnvironment_should_throw_when_environment_name_is_null()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        var act = () => builder.WhenEnvironment(null!, dev => dev.NotNull(o => o.ApiKey));

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("environmentName");
    }

    //==============================================================================================
    /// <summary>
    /// WhenEnvironment() should throw ArgumentNullException when configure is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WhenEnvironment_should_throw_when_configure_is_null()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        var act = () => builder.WhenEnvironment("Development", null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("configure");
    }

    #endregion

    #region WhenDevelopment Tests

    //==============================================================================================
    /// <summary>
    /// WhenDevelopment() should apply rules in Development environment.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WhenDevelopment_should_apply_rules_in_development()
    {
        var originalEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        try
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            var services = new ServiceCollection();
            var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

            builder.WhenDevelopment(dev => dev
                .NotNull(o => o.DebugMode, "Debug mode required in Development"));

            var errors = builder.Validate(new ApiConfig { DebugMode = null });

            errors.Should().ContainSingle();
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalEnv);
        }
    }

    //==============================================================================================
    /// <summary>
    /// WhenDevelopment() should skip rules in non-Development environment.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WhenDevelopment_should_skip_rules_in_non_development()
    {
        var originalEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        try
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");

            var services = new ServiceCollection();
            var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

            builder.WhenDevelopment(dev => dev
                .NotNull(o => o.DebugMode, "Debug mode required"));

            var errors = builder.Validate(new ApiConfig { DebugMode = null });

            errors.Should().BeEmpty();
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalEnv);
        }
    }

    //==============================================================================================
    /// <summary>
    /// WhenDevelopment() should throw ArgumentNullException when builder is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WhenDevelopment_should_throw_when_builder_is_null()
    {
        ConfigValidationBuilder<ApiConfig> builder = null!;

        var act = () => builder.WhenDevelopment(dev => dev.NotNull(o => o.ApiKey));

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("builder");
    }

    //==============================================================================================
    /// <summary>
    /// WhenDevelopment() should throw ArgumentNullException when configure is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WhenDevelopment_should_throw_when_configure_is_null()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        var act = () => builder.WhenDevelopment(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("configure");
    }

    #endregion

    #region WhenProduction Tests

    //==============================================================================================
    /// <summary>
    /// WhenProduction() should apply rules in Production environment.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WhenProduction_should_apply_rules_in_production()
    {
        var originalEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        try
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");

            var services = new ServiceCollection();
            var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

            builder.WhenProduction(prod => prod
                .GreaterThan(o => o.Timeout, 0, "Timeout must be positive in Production"));

            var errors = builder.Validate(new ApiConfig { Timeout = 0 });

            errors.Should().ContainSingle();
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalEnv);
        }
    }

    //==============================================================================================
    /// <summary>
    /// WhenProduction() should throw ArgumentNullException when builder is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WhenProduction_should_throw_when_builder_is_null()
    {
        ConfigValidationBuilder<ApiConfig> builder = null!;

        var act = () => builder.WhenProduction(prod => prod.NotNull(o => o.ApiKey));

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("builder");
    }

    //==============================================================================================
    /// <summary>
    /// WhenProduction() should throw ArgumentNullException when configure is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WhenProduction_should_throw_when_configure_is_null()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        var act = () => builder.WhenProduction(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("configure");
    }

    #endregion

    #region WhenStaging Tests

    //==============================================================================================
    /// <summary>
    /// WhenStaging() should apply rules in Staging environment.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WhenStaging_should_apply_rules_in_staging()
    {
        var originalEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        try
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Staging");

            var services = new ServiceCollection();
            var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

            builder.WhenStaging(stage => stage
                .NotNull(o => o.ApiKey, "API key required in Staging"));

            var errors = builder.Validate(new ApiConfig { ApiKey = null });

            errors.Should().ContainSingle();
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalEnv);
        }
    }

    //==============================================================================================
    /// <summary>
    /// WhenStaging() should throw ArgumentNullException when builder is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WhenStaging_should_throw_when_builder_is_null()
    {
        ConfigValidationBuilder<ApiConfig> builder = null!;

        var act = () => builder.WhenStaging(stage => stage.NotNull(o => o.ApiKey));

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("builder");
    }

    //==============================================================================================
    /// <summary>
    /// WhenStaging() should throw ArgumentNullException when configure is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WhenStaging_should_throw_when_configure_is_null()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        var act = () => builder.WhenStaging(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("configure");
    }

    #endregion
}
