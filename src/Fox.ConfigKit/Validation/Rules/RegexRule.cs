//==================================================================================================
// Validation rule that ensures a string matches a regular expression pattern.
// Sealed class implementation for regex pattern validation.
//==================================================================================================
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Fox.ConfigKit.Errors;

namespace Fox.ConfigKit.Validation.Rules;

//==================================================================================================
/// <summary>
/// Validates that a string matches a specified regular expression pattern.
/// </summary>
/// <typeparam name="T">The type of the configuration class.</typeparam>
/// <param name="selector">Expression that selects the string property.</param>
/// <param name="pattern">The regular expression pattern.</param>
/// <param name="customMessage">Optional custom error message.</param>
//==================================================================================================
internal sealed class RegexRule<T>(Expression<Func<T, string?>> selector, string pattern, string? customMessage = null) : ValidationRuleBase, IValidationRule<T> where T : class
{
    #region Fields

    private readonly Func<T, string?> getValue = selector.Compile();
    private readonly string propertyName = GetPropertyName(selector);
    private readonly Regex regex = new(pattern, RegexOptions.Compiled);

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

        if (value != null && !regex.IsMatch(value))
        {
            var key = $"{sectionName}:{propertyName}";
            var message = customMessage ?? $"{propertyName} does not match required pattern";

            var suggestions = new List<string>
            {
                $"Required pattern: {pattern}",
                $"Current value: {value}"
            };

            return new ConfigValidationError(key, message, value, suggestions);
        }

        return null;
    }

    #endregion
}
