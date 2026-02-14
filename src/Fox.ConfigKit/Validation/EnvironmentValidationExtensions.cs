//==================================================================================================
// Provides extension methods for environment-specific validation rules.
// Fluent API for conditional validation based on application environment.
//==================================================================================================

namespace Fox.ConfigKit.Validation;

//==============================================================================================
/// <summary>
/// Extension methods for environment-specific validation.
/// </summary>
//==============================================================================================
public static class EnvironmentValidationExtensions
{
    #region Public Methods

    //==============================================================================================
    /// <summary>
    /// Applies validation rules only when the application is running in a specific environment.
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="environmentName">The environment name (e.g., "Development", "Production").</param>
    /// <param name="configure">Configuration action for environment-specific rules.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> WhenEnvironment<T>(this ConfigValidationBuilder<T> builder, string environmentName, Action<ConfigValidationBuilder<T>> configure) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(environmentName);
        ArgumentNullException.ThrowIfNull(configure);

        var currentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                                 Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                                 "Production";

        if (string.Equals(currentEnvironment, environmentName, StringComparison.OrdinalIgnoreCase))
        {
            configure(builder);
        }

        return builder;
    }

    //==============================================================================================
    /// <summary>
    /// Applies validation rules for Development environment.
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="configure">Configuration action for development rules.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> WhenDevelopment<T>(this ConfigValidationBuilder<T> builder, Action<ConfigValidationBuilder<T>> configure) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        return builder.WhenEnvironment("Development", configure);
    }

    //==============================================================================================
    /// <summary>
    /// Applies validation rules for Production environment.
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="configure">Configuration action for production rules.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> WhenProduction<T>(this ConfigValidationBuilder<T> builder, Action<ConfigValidationBuilder<T>> configure) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        return builder.WhenEnvironment("Production", configure);
    }

    //==============================================================================================
    /// <summary>
    /// Applies validation rules for Staging environment.
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="configure">Configuration action for staging rules.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> WhenStaging<T>(this ConfigValidationBuilder<T> builder, Action<ConfigValidationBuilder<T>> configure) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        return builder.WhenEnvironment("Staging", configure);
    }

    #endregion
}
