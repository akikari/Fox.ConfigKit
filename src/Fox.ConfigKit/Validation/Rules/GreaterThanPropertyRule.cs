//==================================================================================================
// Validation rule that compares two properties ensuring one is greater than the other.
// Sealed class implementation for property-to-property comparison with exclusive bound.
//==================================================================================================
using System.Linq.Expressions;
using Fox.ConfigKit.Errors;

namespace Fox.ConfigKit.Validation.Rules;

//==================================================================================================
/// <summary>
/// Validates that one property value is greater than another property value.
/// </summary>
/// <typeparam name="T">The type of the configuration class.</typeparam>
/// <typeparam name="TValue">The type of the comparable values.</typeparam>
/// <param name="selector">Expression that selects the property to validate.</param>
/// <param name="compareToSelector">Expression that selects the property to compare against.</param>
/// <param name="customMessage">Optional custom error message.</param>
//==================================================================================================
internal sealed class GreaterThanPropertyRule<T, TValue>(Expression<Func<T, TValue>> selector, Expression<Func<T, TValue>> compareToSelector, string? customMessage = null) : ValidationRuleBase, IValidationRule<T> where T : class where TValue : IComparable<TValue>
{
    #region Fields

    private readonly Func<T, TValue> getValue = selector.Compile();
    private readonly Func<T, TValue> getCompareToValue = compareToSelector.Compile();
    private readonly string propertyName = GetPropertyName(selector);
    private readonly string compareToPropertyName = GetPropertyName(compareToSelector);

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
        var compareToValue = getCompareToValue(options);

        if (value.CompareTo(compareToValue) <= 0)
        {
            var key = $"{sectionName}:{propertyName}";
            var message = customMessage ?? $"{propertyName} must be > {compareToPropertyName} (current: {propertyName}={value}, {compareToPropertyName}={compareToValue})";

            var suggestions = new List<string>
            {
                $"{propertyName} must be greater than {compareToPropertyName}",
                $"Current {propertyName}: {value}",
                $"Current {compareToPropertyName}: {compareToValue}"
            };

            return new ConfigValidationError(key, message, value, suggestions);
        }

        return null;
    }

    #endregion
}
