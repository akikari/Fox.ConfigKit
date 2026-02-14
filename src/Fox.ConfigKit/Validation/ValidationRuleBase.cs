//==================================================================================================
// Base class for validation rules providing common functionality.
// Abstract class that extracts property names from selector expressions.
//==================================================================================================
using System.Linq.Expressions;

namespace Fox.ConfigKit.Validation;

//==================================================================================================
/// <summary>
/// Base class for validation rules providing common expression utilities.
/// </summary>
//==================================================================================================
public abstract class ValidationRuleBase
{
    #region Protected Methods

    //==============================================================================================
    /// <summary>
    /// Extracts the property name from a selector expression.
    /// </summary>
    /// <typeparam name="T">The type of the configuration class.</typeparam>
    /// <typeparam name="TValue">The type of the property value.</typeparam>
    /// <param name="selector">Expression that selects the property.</param>
    /// <returns>The name of the property.</returns>
    /// <exception cref="ArgumentException">Thrown when selector is not a property expression.</exception>
    //==============================================================================================
    protected static string GetPropertyName<T, TValue>(Expression<Func<T, TValue>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        if (selector.Body is MemberExpression memberExpr)
        {
            return memberExpr.Member.Name;
        }

        throw new ArgumentException("Selector must be a property expression", nameof(selector));
    }

    #endregion
}
