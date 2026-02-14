//==================================================================================================
// Validation rule that executes conditionally based on a predicate.
// Wrapper rule implementation for cross-property validation scenarios.
//==================================================================================================
using System.Linq.Expressions;
using Fox.ConfigKit.Errors;

namespace Fox.ConfigKit.Validation.Rules;

//==================================================================================================
/// <summary>
/// Validation rule that only executes when a condition is met.
/// </summary>
/// <typeparam name="T">The type of the configuration class.</typeparam>
/// <param name="condition">Expression that determines whether to execute validation.</param>
/// <param name="innerRule">The validation rule to execute conditionally.</param>
//==================================================================================================
internal sealed class ConditionalValidationRule<T>(Expression<Func<T, bool>> condition, IValidationRule<T> innerRule) : IValidationRule<T> where T : class
{
    #region Fields

    private readonly Func<T, bool> compiledCondition = condition.Compile();
    private readonly IValidationRule<T> innerRule = innerRule;

    #endregion

    #region Public Methods

    //==============================================================================================
    /// <summary>
    /// Validates the options only if the condition is true.
    /// </summary>
    /// <param name="options">The configuration object to validate.</param>
    /// <param name="sectionName">The configuration section name.</param>
    /// <returns>A validation error if the rule fails; otherwise, null.</returns>
    //==============================================================================================
    public ConfigValidationError? Validate(T options, string sectionName)
    {
        if (compiledCondition(options))
        {
            return innerRule.Validate(options, sectionName);
        }

        return null;
    }

    #endregion
}
