//==================================================================================================
// Extension methods for converting ConfigValidationError to Result pattern errors.
// Provides seamless integration between Fox.ConfigKit validation and Fox.ResultKit error handling.
//==================================================================================================
using Fox.ConfigKit.Errors;
using Fox.ResultKit;

namespace Fox.ConfigKit.ResultKit;

//==============================================================================================
/// <summary>
/// Extension methods for converting <see cref="ConfigValidationError"/> to Result pattern errors.
/// </summary>
//==============================================================================================
public static class ConfigValidationErrorExtensions
{
    //==========================================================================================
    /// <summary>
    /// Converts a <see cref="ConfigValidationError"/> to a structured error string with error code.
    /// </summary>
    /// <param name="error">The configuration validation error to convert.</param>
    /// <returns>A structured error string in "ERROR_CODE: message" format.</returns>
    /// <remarks>
    /// The error code follows the pattern: VALIDATION_{KEY} where KEY is the configuration key in uppercase.
    /// Example: For key "Database.ConnectionString", the error code will be "VALIDATION_DATABASE_CONNECTIONSTRING".
    /// </remarks>
    //==========================================================================================
    public static string ToResultError(this ConfigValidationError error)
    {
        ArgumentNullException.ThrowIfNull(error);

        var errorCode = $"VALIDATION_{error.Key.Replace(".", "_").ToUpperInvariant()}";
        return ResultError.Create(errorCode, error.Message);
    }

    //==========================================================================================
    /// <summary>
    /// Converts a collection of <see cref="ConfigValidationError"/> to structured error strings.
    /// </summary>
    /// <param name="errors">The configuration validation errors to convert.</param>
    /// <returns>A collection of structured error strings.</returns>
    //==========================================================================================
    public static IEnumerable<string> ToResultErrors(this IEnumerable<ConfigValidationError> errors)
    {
        return errors.Select(e => e.ToResultError());
    }
}
