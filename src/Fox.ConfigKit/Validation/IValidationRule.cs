//==================================================================================================
// Defines the contract for configuration validation rules.
// Interface for implementing custom validation logic.
//==================================================================================================
using Fox.ConfigKit.Errors;

namespace Fox.ConfigKit.Validation;

//==================================================================================================
/// <summary>
/// Defines a validation rule for configuration options.
/// </summary>
//==================================================================================================
public interface IValidationRule<in T> where T : class
{
    //==============================================================================================
    /// <summary>
    /// Validates the configuration options.
    /// </summary>
    /// <param name="options">The options instance to validate.</param>
    /// <param name="sectionName">The configuration section name.</param>
    /// <returns>A validation error if validation fails; otherwise, null.</returns>
    //==============================================================================================
    ConfigValidationError? Validate(T options, string sectionName);
}
