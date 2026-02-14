//==================================================================================================
// Validation rule that ensures a comparable value is at least a minimum value.
// Sealed class implementation for minimum value validation with inclusive bound.
//==================================================================================================
using System.Linq.Expressions;
using Fox.ConfigKit.Errors;

namespace Fox.ConfigKit.Validation.Rules;

//==================================================================================================
/// <summary>
/// Validates that a comparable value is greater than or equal to a specified minimum.
/// </summary>
/// <typeparam name="T">The type of the configuration class.</typeparam>
/// <typeparam name="TValue">The type of the comparable value.</typeparam>
/// <param name="selector">Expression that selects the property to validate.</param>
/// <param name="minimum">The minimum allowed value (inclusive).</param>
/// <param name="customMessage">Optional custom error message.</param>
//==================================================================================================
internal sealed class MinimumRule<T, TValue>(Expression<Func<T, TValue>> selector, TValue minimum, string? customMessage = null) : ValidationRuleBase, IValidationRule<T> where T : class where TValue : IComparable<TValue>
{
    #region Fields

    private readonly Func<T, TValue> getValue = selector.Compile();
    private readonly string propertyName = GetPropertyName(selector);

    #endregion

    #region Public Methods

    //==============================================================================================
    /// <summary>
    /// Validates the rule.
    /// </summary>
    /// <param name="options">The configuration object to validate.</param>
    /// <param name="sectionName">The configuration section name.</param>
    /// <returns>A validation error if the rule fails; otherwise, null.</returns>
    //==============================================================================================
    public ConfigValidationError? Validate(T options, string sectionName)
    {
        var value = getValue(options);

        if (value.CompareTo(minimum) < 0)
        {
            var key = $"{sectionName}:{propertyName}";
            var message = customMessage ?? $"{propertyName} must be at least {minimum} (current: {value})";

            var suggestions = new List<string>
            {
                $"Must be at least {minimum}",
                $"Current value: {value}"
            };

            return new ConfigValidationError(key, message, value, suggestions);
        }

        return null;
    }

    #endregion
}
