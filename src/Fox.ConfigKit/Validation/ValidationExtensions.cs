//==================================================================================================
// Provides extension methods for adding basic validation rules to configuration builders.
// Fluent API extensions for common validation scenarios.
//==================================================================================================
using System.Linq.Expressions;
using Fox.ConfigKit.Validation.Rules;

namespace Fox.ConfigKit.Validation;

//==============================================================================================
/// <summary>
/// Extension methods for common validation rules.
/// </summary>
//==============================================================================================
public static class ValidationExtensions
{
    #region Public Methods

    //==============================================================================================
    /// <summary>
    /// Validates that a string property is not null or empty.
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="selector">Expression to select the property to validate.</param>
    /// <param name="message">Custom error message.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> NotEmpty<T>(this ConfigValidationBuilder<T> builder, Expression<Func<T, string?>> selector, string? message = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new NotEmptyRule<T>(selector, message));
    }

    //==============================================================================================
    /// <summary>
    /// Validates that a property is not null.
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <typeparam name="TProperty">The type of the property to validate.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="selector">Expression to select the property to validate.</param>
    /// <param name="message">Custom error message.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> NotNull<T, TProperty>(this ConfigValidationBuilder<T> builder, Expression<Func<T, TProperty?>> selector, string? message = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new NotNullRule<T, TProperty>(selector, message));
    }

    //==============================================================================================
    /// <summary>
    /// Validates that a value is greater than a minimum (exclusive).
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <typeparam name="TValue">The type of the value to compare.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="selector">Expression to select the property to validate.</param>
    /// <param name="minimum">The exclusive minimum value.</param>
    /// <param name="message">Custom error message.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> GreaterThan<T, TValue>(this ConfigValidationBuilder<T> builder, Expression<Func<T, TValue>> selector, TValue minimum, string? message = null) where T : class where TValue : IComparable<TValue>
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new GreaterThanRule<T, TValue>(selector, minimum, message));
    }

    //==============================================================================================
    /// <summary>
    /// Validates that a value is less than a maximum (exclusive).
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <typeparam name="TValue">The type of the value to compare.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="selector">Expression to select the property to validate.</param>
    /// <param name="maximum">The exclusive maximum value.</param>
    /// <param name="message">Custom error message.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> LessThan<T, TValue>(this ConfigValidationBuilder<T> builder, Expression<Func<T, TValue>> selector, TValue maximum, string? message = null) where T : class where TValue : IComparable<TValue>
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new LessThanRule<T, TValue>(selector, maximum, message));
    }

    //==============================================================================================
    /// <summary>
    /// Validates that a value is greater than or equal to a minimum (inclusive).
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <typeparam name="TValue">The type of the value to compare.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="selector">Expression to select the property to validate.</param>
    /// <param name="minimum">The inclusive minimum value.</param>
    /// <param name="message">Custom error message.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> Minimum<T, TValue>(this ConfigValidationBuilder<T> builder, Expression<Func<T, TValue>> selector, TValue minimum, string? message = null) where T : class where TValue : IComparable<TValue>
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new MinimumRule<T, TValue>(selector, minimum, message));
    }

    //==============================================================================================
    /// <summary>
    /// Validates that a value is less than or equal to a maximum (inclusive).
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <typeparam name="TValue">The type of the value to compare.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="selector">Expression to select the property to validate.</param>
    /// <param name="maximum">The inclusive maximum value.</param>
    /// <param name="message">Custom error message.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> Maximum<T, TValue>(this ConfigValidationBuilder<T> builder, Expression<Func<T, TValue>> selector, TValue maximum, string? message = null) where T : class where TValue : IComparable<TValue>
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new MaximumRule<T, TValue>(selector, maximum, message));
    }

    //==============================================================================================
    /// <summary>
    /// Validates that a value is within a specific range (inclusive).
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <typeparam name="TValue">The type of the value to compare.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="selector">Expression to select the property to validate.</param>
    /// <param name="minimum">The inclusive minimum value.</param>
    /// <param name="maximum">The inclusive maximum value.</param>
    /// <param name="message">Custom error message.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> InRange<T, TValue>(this ConfigValidationBuilder<T> builder, Expression<Func<T, TValue>> selector, TValue minimum, TValue maximum, string? message = null) where T : class where TValue : IComparable<TValue>
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new RangeRule<T, TValue>(selector, minimum, maximum, message));
    }

    //==============================================================================================
    /// <summary>
    /// Validates that a string matches a regular expression pattern.
    /// </summary>
    /// <typeparam name="T">The type of options to validate.</typeparam>
    /// <param name="builder">The configuration validation builder.</param>
    /// <param name="selector">Expression to select the property to validate.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="message">Custom error message.</param>
    /// <returns>The builder for method chaining.</returns>
    //==============================================================================================
    public static ConfigValidationBuilder<T> MatchesPattern<T>(this ConfigValidationBuilder<T> builder, Expression<Func<T, string?>> selector, string pattern, string? message = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddRule(new RegexRule<T>(selector, pattern, message));
    }

    #endregion
}
