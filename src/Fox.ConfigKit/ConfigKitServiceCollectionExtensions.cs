//==================================================================================================
// Service collection extensions for registering Fox.ConfigKit validation.
// Entry point for fluent configuration validation API.
//==================================================================================================
using Fox.ConfigKit.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fox.ConfigKit;

//==================================================================================================
/// <summary>
/// Extension methods for registering configuration validation in the service collection.
/// </summary>
//==================================================================================================
public static class ConfigKitServiceCollectionExtensions
{
    #region Public Methods

    //==============================================================================================
    /// <summary>
    /// Adds Fox.ConfigKit configuration validation services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    //==============================================================================================
    public static IServiceCollection AddConfigKit(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }

    //==============================================================================================
    /// <summary>
    /// Registers and binds a configuration section with validation that runs at startup.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to configure.</typeparam>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="sectionName">The configuration section name to bind.</param>
    /// <returns>A builder for configuring validation rules.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<TOptions> AddConfigKit<TOptions>(this IServiceCollection services, string sectionName) where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(sectionName);

        services.AddOptions<TOptions>()
            .BindConfiguration(sectionName);

        return new ConfigValidationBuilder<TOptions>(services, sectionName);
    }

    //==============================================================================================
    /// <summary>
    /// Configures validation rules that will be executed at application startup.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to validate.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <returns>The service collection for method chaining.</returns>
    //==============================================================================================
    public static IServiceCollection ValidateOnStartup<TOptions>(this ConfigValidationBuilder<TOptions> builder) where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddSingleton<IValidateOptions<TOptions>>(new ValidatedOptionsConfiguration<TOptions>(builder));

        return builder.Services;
    }

    #endregion
}
