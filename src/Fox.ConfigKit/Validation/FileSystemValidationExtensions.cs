//==================================================================================================
// Provides extension methods for file system and network validation rules.
// Fluent API extensions for infrastructure validation scenarios.
//==================================================================================================
using System.Linq.Expressions;
using Fox.ConfigKit.Validation.Rules;

namespace Fox.ConfigKit.Validation;

//==============================================================================================
/// <summary>
/// Extension methods for file system and network validation rules.
/// </summary>
//==============================================================================================
public static class FileSystemValidationExtensions
{
    #region Public Methods

    //==============================================================================================
    /// <summary>
    /// Validates that a file exists at the specified path.
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="selector">Expression to select the file path.</param>
    /// <param name="message">Custom error message.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> FileExists<T>(this ConfigValidationBuilder<T> builder, Expression<Func<T, string?>> selector, string? message = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new FileExistsRule<T>(selector, message));
    }

    //==============================================================================================
    /// <summary>
    /// Validates that a directory exists at the specified path.
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="selector">Expression to select the directory path.</param>
    /// <param name="message">Custom error message.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> DirectoryExists<T>(this ConfigValidationBuilder<T> builder, Expression<Func<T, string?>> selector, string? message = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new DirectoryExistsRule<T>(selector, message));
    }

    //==============================================================================================
    /// <summary>
    /// Validates that a URL is reachable.
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="selector">Expression to select the URL.</param>
    /// <param name="timeout">Maximum time to wait for response.</param>
    /// <param name="message">Custom error message.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> UrlReachable<T>(this ConfigValidationBuilder<T> builder, Expression<Func<T, string?>> selector, TimeSpan? timeout = null, string? message = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new UrlReachableRule<T>(selector, timeout ?? TimeSpan.FromSeconds(5), message));
    }

    //==============================================================================================
    /// <summary>
    /// Validates that a network port is available.
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="selector">Expression to select the port number.</param>
    /// <param name="message">Custom error message.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> PortAvailable<T>(this ConfigValidationBuilder<T> builder, Expression<Func<T, int>> selector, string? message = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new PortAvailableRule<T>(selector, message));
    }

    #endregion
}
