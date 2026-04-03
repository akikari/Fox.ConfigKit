//==================================================================================================
// Extension methods for validating collections within configuration objects.
// Fluent API for nested collection validation with per-item rules.
//==================================================================================================
using System.Linq.Expressions;
using Fox.ConfigKit.Validation.Rules;

namespace Fox.ConfigKit.Validation;

//==============================================================================================
/// <summary>
/// Extension methods for collection validation in configuration objects.
/// </summary>
//==============================================================================================
public static class CollectionValidationExtensions
{
    #region Public Methods

    //==============================================================================================
    /// <summary>
    /// Validates each item in a collection property using nested validation rules.
    /// </summary>
    /// <typeparam name="T">The type of configuration to validate.</typeparam>
    /// <typeparam name="TItem">The type of items in the collection.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="collectionSelector">Expression to select the collection property.</param>
    /// <param name="configureItemValidator">Action to configure validation rules for each item.</param>
    /// <param name="minCount">Minimum number of items required in the collection (default: 0).</param>
    /// <param name="emptyMessage">Custom error message when collection has fewer items than minCount.</param>
    /// <returns>The builder for method chaining.</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddConfigKit&lt;ServerConfig&gt;("Servers")
    ///     .ValidateEach(c => c.Endpoints,
    ///         itemBuilder => itemBuilder
    ///             .NotEmpty(e => e.Url, "Endpoint URL is required")
    ///             .InRange(e => e.Port, 1, 65535, "Port must be between 1 and 65535"),
    ///         minCount: 1,
    ///         emptyMessage: "At least one endpoint must be configured")
    ///     .ValidateOnStartup();
    /// </code>
    /// </example>
    //==============================================================================================
    public static ConfigValidationBuilder<T> ValidateEach<T, TItem>(this ConfigValidationBuilder<T> builder, Expression<Func<T, IEnumerable<TItem>>> collectionSelector, Action<ConfigValidationBuilder<TItem>> configureItemValidator, int minCount = 0, string? emptyMessage = null) where T : class where TItem : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(collectionSelector);
        ArgumentNullException.ThrowIfNull(configureItemValidator);

        return builder.AddRule(new CollectionValidationRule<T, TItem>(collectionSelector, configureItemValidator, minCount, emptyMessage));
    }

    #endregion
}
