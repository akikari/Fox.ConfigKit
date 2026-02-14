//==================================================================================================
// Provides fluent API for building configuration validation rules.
// Builder pattern implementation for type-safe validation composition.
//==================================================================================================
using Fox.ConfigKit.Errors;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ConfigKit.Validation;

//==================================================================================================
/// <summary>
/// Fluent builder for configuring validation rules on options.
/// </summary>
/// <typeparam name="T">The type of options to validate.</typeparam>
/// <param name="services">The service collection being configured.</param>
/// <param name="sectionName">The configuration section name.</param>
//==================================================================================================
public sealed class ConfigValidationBuilder<T>(IServiceCollection services, string sectionName) : IValidationBuilder<T> where T : class
{
    #region Fields

    private readonly List<IValidationRule<T>> rules = [];
    private readonly string sectionName = sectionName;

    #endregion

    #region Properties

    //==============================================================================================
    /// <summary>
    /// Gets the service collection being configured.
    /// </summary>
    //==============================================================================================
    internal IServiceCollection Services { get; } = services;

    #endregion

    #region Public Methods

    //==============================================================================================
    /// <summary>
    /// Adds a custom validation rule.
    /// </summary>
    /// <param name="rule">The validation rule to add.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public ConfigValidationBuilder<T> AddRule(IValidationRule<T> rule)
    {
        rules.Add(rule);
        return this;
    }

    //==============================================================================================
    /// <summary>
    /// Adds a custom validation rule (explicit interface implementation).
    /// </summary>
    /// <param name="rule">The validation rule to add.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    IValidationBuilder<T> IValidationBuilder<T>.AddRule(IValidationRule<T> rule)
    {
        return AddRule(rule);
    }

    //==============================================================================================
    /// <summary>
    /// Validates the options and returns any errors.
    /// </summary>
    /// <param name="options">The options instance to validate.</param>
    /// <returns>A collection of validation errors.</returns>
    //==============================================================================================
    public IEnumerable<ConfigValidationError> Validate(T options)
    {
        foreach (var rule in rules)
        {
            var error = rule.Validate(options, sectionName);
            if (error != null)
            {
                yield return error;
            }
        }
    }

    #endregion
}
