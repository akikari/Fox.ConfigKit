//==================================================================================================
// Provides extension methods for conditional and cross-property validation rules.
// Fluent API for validating properties based on other property values.
//==================================================================================================
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Fox.ConfigKit.Validation.Rules;

namespace Fox.ConfigKit.Validation;

//==============================================================================================
/// <summary>
/// Extension methods for conditional validation based on property values.
/// </summary>
//==============================================================================================
public static class ConditionalValidationExtensions
{
    #region Public Methods

    //==============================================================================================
    /// <summary>
    /// Applies validation rules only when a condition is met.
    /// </summary>
    /// <typeparam name="T">The options type.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="condition">Expression that evaluates the condition.</param>
    /// <param name="configure">Action to configure conditional validation rules.</param>
    /// <returns>The builder for method chaining.</returns>
    /// <example>
    /// <code>
    /// builder.When(o => o.UseHttps, https => https
    ///     .NotNull(o => o.CertificatePath, "HTTPS requires certificate")
    ///     .FileExists(o => o.CertificatePath));
    /// </code>
    /// </example>
    //==============================================================================================
    public static ConfigValidationBuilder<T> When<T>(this ConfigValidationBuilder<T> builder, Expression<Func<T, bool>> condition, Action<ConfigValidationBuilder<T>> configure) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(condition);
        ArgumentNullException.ThrowIfNull(configure);

        var wrapper = new ConditionalValidationContext<T>(builder, condition);
        configure(builder);
        wrapper.Apply();

        return builder;
    }

    #endregion
}

//==============================================================================================
/// <summary>
/// Internal context for collecting and wrapping rules added during conditional block.
/// </summary>
/// <typeparam name="T">The type of options being validated.</typeparam>
//==============================================================================================
internal sealed class ConditionalValidationContext<T> where T : class
{
    #region Fields

    private readonly ConfigValidationBuilder<T> builder;
    private readonly Expression<Func<T, bool>> condition;
    private readonly int ruleCountBefore;

    #endregion

    #region Public Methods

    //==============================================================================================
    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionalValidationContext{T}"/> class.
    /// </summary>
    /// <param name="builder">The validation builder to track.</param>
    /// <param name="condition">The condition expression for rule application.</param>
    //==============================================================================================
    public ConditionalValidationContext(ConfigValidationBuilder<T> builder, Expression<Func<T, bool>> condition)
    {
        this.builder = builder;
        this.condition = condition;

        // Get current rule count via reflection
        var rulesField = typeof(ConfigValidationBuilder<T>).GetField("rules", BindingFlags.NonPublic | BindingFlags.Instance);
        var rules = (ICollection)(rulesField?.GetValue(builder) ?? throw new InvalidOperationException());
        ruleCountBefore = rules.Count;
    }

    //==============================================================================================
    /// <summary>
    /// Applies the condition wrapper to rules added during the conditional block.
    /// </summary>
    //==============================================================================================
    public void Apply()
    {
        // Get rules field
        var rulesField = typeof(ConfigValidationBuilder<T>).GetField("rules", BindingFlags.NonPublic | BindingFlags.Instance);
        var rules = (List<IValidationRule<T>>)(rulesField?.GetValue(builder) ?? throw new InvalidOperationException());

        // Get newly added rules
        var newRules = rules.Skip(ruleCountBefore).ToList();

        // Remove them from the builder
        rules.RemoveRange(ruleCountBefore, newRules.Count);

        // Re-add them wrapped in ConditionalValidationRule
        foreach (var rule in newRules)
        {
            rules.Add(new ConditionalValidationRule<T>(condition, rule));
        }
    }

    #endregion
}
