//==================================================================================================
// Wrapper for options with validation configuration.
// Internal class for storing validation builders per options type.
//==================================================================================================
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace Fox.ConfigKit.Validation;

//==================================================================================================
/// <summary>
/// Wrapper that combines options configuration with validation.
/// </summary>
/// <typeparam name="T">The type of options to validate.</typeparam>
/// <param name="builder">The validation builder containing the rules.</param>
//==================================================================================================
[ExcludeFromCodeCoverage]
internal sealed class ValidatedOptionsConfiguration<T>(ConfigValidationBuilder<T> builder) : IValidateOptions<T> where T : class
{
    #region Public Methods

    //==============================================================================================
    /// <summary>
    /// Validates the options instance.
    /// </summary>
    /// <param name="name">The name of the options instance.</param>
    /// <param name="options">The options instance to validate.</param>
    /// <returns>The validation result.</returns>
    //==============================================================================================
    public ValidateOptionsResult Validate(string? name, T options)
    {
        var errors = builder.Validate(options).ToList();

        if (errors.Count > 0)
        {
            var failureMessages = errors.Select(e => e.Message);
            return ValidateOptionsResult.Fail(failureMessages);
        }

        return ValidateOptionsResult.Success;
    }

    #endregion
}
