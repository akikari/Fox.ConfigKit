//==================================================================================================
// Validation rule that ensures a file exists at the specified path.
// Sealed class implementation for file existence validation.
//==================================================================================================
using System.Linq.Expressions;
using Fox.ConfigKit.Errors;

namespace Fox.ConfigKit.Validation.Rules;

//==================================================================================================
/// <summary>
/// Validates that a file exists at the specified path.
/// </summary>
/// <typeparam name="T">The type of the configuration class.</typeparam>
/// <param name="selector">Expression that selects the file path property.</param>
/// <param name="customMessage">Optional custom error message.</param>
//==================================================================================================
internal sealed class FileExistsRule<T>(Expression<Func<T, string?>> selector, string? customMessage = null) : ValidationRuleBase, IValidationRule<T> where T : class
{
    #region Fields

    private readonly Func<T, string?> getValue = selector.Compile();
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
        var path = getValue(options);

        if (string.IsNullOrWhiteSpace(path))
        {
            var key = $"{sectionName}:{propertyName}";
            var message = customMessage ?? $"{propertyName} file path is not specified";
            return new ConfigValidationError(key, message, path, ["Specify a valid file path"]);
        }

        if (!File.Exists(path))
        {
            var errorKey = $"{sectionName}:{propertyName}";
            var errorMessage = customMessage ?? $"File does not exist: {path}";
            return new ConfigValidationError(errorKey, errorMessage, path, [$"Create file at: {path}"]);
        }

        return null;
    }

    #endregion
}
