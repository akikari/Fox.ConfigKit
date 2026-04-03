//==================================================================================================
// Validation rule for validating each item in a collection with nested validation rules.
// Sealed class implementation for collection item validation with configurable builder.
//==================================================================================================
using System.Linq.Expressions;
using Fox.ConfigKit.Errors;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ConfigKit.Validation.Rules;

//==================================================================================================
/// <summary>
/// Validates each item in a collection using nested validation rules.
/// </summary>
/// <typeparam name="T">The type of the configuration class.</typeparam>
/// <typeparam name="TItem">The type of items in the collection.</typeparam>
/// <param name="collectionSelector">Expression that selects the collection property.</param>
/// <param name="configureItemValidator">Action to configure validation rules for each item.</param>
/// <param name="minCount">Minimum number of items required in the collection.</param>
/// <param name="emptyMessage">Custom error message when collection has fewer items than minCount.</param>
//==================================================================================================
internal sealed class CollectionValidationRule<T, TItem>(Expression<Func<T, IEnumerable<TItem>>> collectionSelector, Action<ConfigValidationBuilder<TItem>> configureItemValidator, int minCount = 0, string? emptyMessage = null) : IValidationRule<T> where T : class where TItem : class
{
    #region Fields

    private readonly Func<T, IEnumerable<TItem>> getCollection = collectionSelector.Compile();
    private readonly string propertyName = ExtractCollectionName(collectionSelector);

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
        var collection = getCollection(options);
        var items = collection?.ToList() ?? [];

        if (minCount > 0 && items.Count < minCount)
        {
            var key = $"{sectionName}:{propertyName}";
            var message = emptyMessage ?? $"{propertyName} must contain at least {minCount} item(s) (current: {items.Count})";

            var suggestions = new List<string>
            {
                $"Required minimum: {minCount} item(s)",
                $"Current count: {items.Count}"
            };

            return new ConfigValidationError(key, message, items.Count, suggestions);
        }

        var errors = new List<string>();
        var services = new ServiceCollection();

        foreach (var item in items)
        {
            var builder = new ConfigValidationBuilder<TItem>(services, typeof(TItem).Name);
            configureItemValidator(builder);

            var validationErrors = builder.Validate(item).ToList();
            if (validationErrors.Count > 0)
            {
                errors.AddRange(validationErrors.Select(e => e.Message));
            }
        }

        if (errors.Count > 0)
        {
            var key = $"{sectionName}:{propertyName}";
            var message = $"{propertyName} contains invalid items: {string.Join("; ", errors)}";

            return new ConfigValidationError(key, message, items, errors);
        }

        return null;
    }

    #endregion

    #region Private Methods

    //==============================================================================================
    /// <summary>
    /// Extracts collection name from expression (supports simple property access and LINQ methods).
    /// </summary>
    //==============================================================================================
    private static string ExtractCollectionName(Expression<Func<T, IEnumerable<TItem>>> selector)
    {
        var expression = selector.Body;

        // If it's a method call (e.g., Where, Select), get the object it's called on
        if (expression is MethodCallExpression methodCall)
        {
            expression = methodCall.Arguments[0];
        }

        // Now extract the member name
        if (expression is MemberExpression memberExpr)
        {
            return memberExpr.Member.Name;
        }

        // Fallback to a generic name if we can't determine it
        return "Collection";
    }

    #endregion
}
