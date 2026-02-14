//==================================================================================================
// Static factory for creating configuration validators with fluent Result-based API.
// Provides standalone validation without dependency injection for functional composition.
//==================================================================================================
using Fox.ConfigKit.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ConfigKit.ResultKit;

//==============================================================================================
/// <summary>
/// Static factory for creating configuration validators with Result pattern support.
/// </summary>
/// <remarks>
/// Provides a fluent API for validating configuration objects without requiring
/// dependency injection or service registration. Useful for standalone validation scenarios
/// and functional composition with Fox.ResultKit.
/// </remarks>
//==============================================================================================
public static class ConfigValidator
{
    //==========================================================================================
    /// <summary>
    /// Creates a new configuration validation builder for the specified configuration type.
    /// </summary>
    /// <typeparam name="T">The type of configuration to validate.</typeparam>
    /// <param name="sectionName">Optional section name for error reporting (defaults to type name).</param>
    /// <returns>A <see cref="ConfigValidationBuilder{T}"/> for chaining validation rules.</returns>
    /// <example>
    /// <code>
    /// var result = ConfigValidator.Validate&lt;DatabaseConfig&gt;()
    ///     .NotEmpty(c => c.ConnectionString, "Connection string is required")
    ///     .InRange(c => c.MaxPoolSize, 1, 1000, "Pool size must be between 1 and 1000")
    ///     .ToResult(dbConfig);
    ///
    /// return result.Match(
    ///     onSuccess: config => InitializeDatabase(config),
    ///     onFailure: errors => throw new InvalidOperationException($"Config invalid: {string.Join(", ", errors)}")
    /// );
    /// </code>
    /// </example>
    //==========================================================================================
    public static ConfigValidationBuilder<T> Validate<T>(string? sectionName = null) where T : class
    {
        var services = new ServiceCollection();
        var section = sectionName ?? typeof(T).Name;
        return new ConfigValidationBuilder<T>(services, section);
    }
}
