//==================================================================================================
// Validation rule that ensures a comparable value is within a specific range.
// Sealed class implementation for range validation with inclusive bounds.
//==================================================================================================
using System.Linq.Expressions;
using Fox.ConfigKit.Errors;

namespace Fox.ConfigKit.Validation.Rules;

//==================================================================================================
/// <summary>
/// Validates that a comparable value is within a specified range (inclusive).
/// </summary>
/// <typeparam name="T">The type of the configuration class.</typeparam>
/// <typeparam name="TValue">The type of the comparable value.</typeparam>
/// <param name="selector">Expression that selects the property to validate.</param>
/// <param name="minimum">The minimum allowed value (inclusive).</param>
/// <param name="maximum">The maximum allowed value (inclusive).</param>
/// <param name="customMessage">Optional custom error message.</param>
//==================================================================================================
internal sealed class RangeRule<T, TValue>(Expression<Func<T, TValue>> selector, TValue minimum, TValue maximum, string? customMessage = null) : ValidationRuleBase, IValidationRule<T> where T : class where TValue : IComparable<TValue>
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

        if (value.CompareTo(minimum) < 0 || value.CompareTo(maximum) > 0)
        {
            var key = $"{sectionName}:{propertyName}";
            var message = customMessage ?? $"{propertyName} must be between {minimum} and {maximum} (current: {value})";

            var suggestions = new List<string>
            {
                $"Valid range: {minimum}-{maximum}",
                $"Current value: {value}"
            };

            return new ConfigValidationError(key, message, value, suggestions);
        }

        return null;
    }

    #endregion
}
