//==================================================================================================
// Extension methods for ConfigValidationBuilder to support Result pattern integration.
// Enables functional error handling with Fox.ResultKit for configuration validation.
//==================================================================================================
using Fox.ConfigKit.Validation;
using Fox.ResultKit;

namespace Fox.ConfigKit.ResultKit;

//==============================================================================================
/// <summary>
/// Extension methods for <see cref="ConfigValidationBuilder{T}"/> to support Result pattern.
/// </summary>
//==============================================================================================
public static class ConfigValidationBuilderExtensions
{
    //==========================================================================================
    /// <summary>
    /// Validates the configuration and returns a <see cref="Result{T}"/> with the configuration object.
    /// </summary>
    /// <typeparam name="T">The type of configuration to validate.</typeparam>
    /// <param name="builder">The validation builder.</param>
    /// <param name="options">The configuration instance to validate.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the configuration object if validation succeeds,
    /// or the first validation error if validation fails.
    /// </returns>
    /// <example>
    /// <code>
    /// var configResult = builder
    ///     .NotEmpty(c => c.Name, "Name is required")
    ///     .InRange(c => c.Port, 1, 65535, "Invalid port")
    ///     .ToResult(config);
    ///
    /// return configResult.Match(
    ///     onSuccess: cfg => $"Config valid: {cfg.Name}",
    ///     onFailure: error => $"Validation failed: {error}"
    /// );
    /// </code>
    /// </example>
    /// <remarks>
    /// If multiple validation errors occur, only the first error is returned.
    /// Use <see cref="ToErrorsResult"/> to collect all errors.
    /// </remarks>
    //==========================================================================================
    public static Result<T> ToResult<T>(this ConfigValidationBuilder<T> builder, T options) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        var errors = builder.Validate(options).ToList();

        if (errors.Count == 0)
        {
            return Result<T>.Success(options);
        }

        var firstError = errors.First().ToResultError();
        return Result<T>.Failure(firstError);
    }

    //==========================================================================================
    /// <summary>
    /// Validates the configuration and returns an <see cref="ErrorsResult"/> with all validation errors.
    /// </summary>
    /// <typeparam name="T">The type of configuration to validate.</typeparam>
    /// <param name="builder">The validation builder.</param>
    /// <param name="options">The configuration instance to validate.</param>
    /// <returns>
    /// An <see cref="ErrorsResult"/> containing all validation errors, or success if validation passes.
    /// </returns>
    /// <example>
    /// <code>
    /// var validationResult = builder
    ///     .NotEmpty(c => c.ConnectionString, "Connection string required")
    ///     .InRange(c => c.Port, 1, 65535, "Invalid port")
    ///     .ToErrorsResult(config);
    ///
    /// if (validationResult.IsFailure)
    /// {
    ///     foreach (var error in validationResult.Errors)
    ///     {
    ///         Console.WriteLine(error);
    ///     }
    /// }
    /// </code>
    /// </example>
    //==========================================================================================
    public static ErrorsResult ToErrorsResult<T>(this ConfigValidationBuilder<T> builder, T options) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        var errors = builder.Validate(options).ToList();

        if (errors.Count == 0)
        {
            return ErrorsResult.Success();
        }

        var resultErrors = errors.ToResultErrors().ToArray();
        return new ErrorsResult(false, resultErrors);
    }

    //==========================================================================================
    /// <summary>
    /// Validates the configuration and returns a <see cref="Result"/> without a value.
    /// </summary>
    /// <typeparam name="T">The type of configuration to validate.</typeparam>
    /// <param name="builder">The validation builder.</param>
    /// <param name="options">The configuration instance to validate.</param>
    /// <returns>
    /// A <see cref="Result"/> indicating success or failure with the first validation error.
    /// </returns>
    /// <example>
    /// <code>
    /// var validationResult = builder
    ///     .NotEmpty(c => c.ConnectionString, "Connection string required")
    ///     .ToValidationResult(config);
    ///
    /// if (validationResult.IsFailure)
    /// {
    ///     throw new InvalidOperationException($"Invalid config: {validationResult.Error}");
    /// }
    /// </code>
    /// </example>
    /// <remarks>
    /// If multiple validation errors occur, only the first error is returned.
    /// Use <see cref="ToErrorsResult"/> to collect all errors.
    /// </remarks>
    //==========================================================================================
    public static Result ToValidationResult<T>(this ConfigValidationBuilder<T> builder, T options) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        var errors = builder.Validate(options).ToList();

        if (errors.Count == 0)
        {
            return Result.Success();
        }

        var firstError = errors.First().ToResultError();
        return Result.Failure(firstError);
    }
}
