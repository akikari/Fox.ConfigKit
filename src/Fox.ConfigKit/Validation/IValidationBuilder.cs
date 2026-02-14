//==================================================================================================
// Common interface for validation builders supporting rule addition.
// Abstraction for fluent validation API composition.
//==================================================================================================

namespace Fox.ConfigKit.Validation;

//==================================================================================================
/// <summary>
/// Common interface for validation builders.
/// </summary>
//==================================================================================================
public interface IValidationBuilder<T> where T : class
{
    //==============================================================================================
    /// <summary>
    /// Adds a validation rule.
    /// </summary>
    /// <param name="rule">The validation rule to add.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    IValidationBuilder<T> AddRule(IValidationRule<T> rule);
}
