//==================================================================================================
// Validation rule that ensures a property is not null.
// Sealed class implementation for null checking validation.
//==================================================================================================
using System.Linq.Expressions;
using Fox.ConfigKit.Errors;

namespace Fox.ConfigKit.Validation.Rules;

//==================================================================================================
/// <summary>
/// Validates that a property is not null.
/// </summary>
/// <typeparam name="T">The type of the configuration class.</typeparam>
/// <typeparam name="TProperty">The type of the property to validate.</typeparam>
/// <param name="selector">Expression that selects the property.</param>
/// <param name="customMessage">Optional custom error message.</param>
//==================================================================================================
internal sealed class NotNullRule<T, TProperty>(Expression<Func<T, TProperty?>> selector, string? customMessage = null) : ValidationRuleBase, IValidationRule<T> where T : class
{
    #region Fields

    private readonly Func<T, TProperty?> getValue = selector.Compile();
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

        if (value is null)
        {
            var key = $"{sectionName}:{propertyName}";
            var message = customMessage ?? $"{propertyName} must not be null";
            var envVar = key.Replace(":", "__").ToUpperInvariant();

            var suggestions = new List<string>
            {
                $"Set via: dotnet user-secrets set \"{key}\" \"<value>\"",
                $"Or set environment variable: {envVar}",
                "Or update appsettings.json"
            };

            return new ConfigValidationError(key, message, null, suggestions);
        }

        return null;
    }

    #endregion
}
