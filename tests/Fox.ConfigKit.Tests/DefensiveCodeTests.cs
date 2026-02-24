//==================================================================================================
// Tests for defensive code and edge cases.
// Verifies null checks, argument validation, and error handling.
//==================================================================================================
using System.Linq.Expressions;
using FluentAssertions;
using Fox.ConfigKit.Validation;

namespace Fox.ConfigKit.Tests;

//==================================================================================================
/// <summary>
/// Tests for defensive code in validation rules.
/// </summary>
//==================================================================================================
public sealed class DefensiveCodeTests
{
    #region Test Helper Class

    private sealed class TestRule : ValidationRuleBase
    {
        public static string GetName<T, TValue>(Expression<Func<T, TValue>> selector) => GetPropertyName(selector);
    }

    private sealed class TestClass
    {
        public string? Name { get; set; }
        public int Value { get; set; }
    }

    #endregion

    #region ValidationRuleBase Tests

    //==============================================================================================
    /// <summary>
    /// GetPropertyName() should extract property name correctly.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void GetPropertyName_should_extract_property_name()
    {
        var rule = new TestRule();

        var name = TestRule.GetName<TestClass, string?>(o => o.Name);

        name.Should().Be("Name");
    }

    //==============================================================================================
    /// <summary>
    /// GetPropertyName() should throw ArgumentNullException when selector is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void GetPropertyName_should_throw_when_selector_is_null()
    {
        var rule = new TestRule();

        var act = () => TestRule.GetName<TestClass, string?>(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("selector");
    }

    //==============================================================================================
    /// <summary>
    /// GetPropertyName() should throw ArgumentException when selector is not property expression.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void GetPropertyName_should_throw_when_selector_is_not_property()
    {
        var rule = new TestRule();

        var act = () => TestRule.GetName<TestClass, int>(o => 42);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*property expression*")
            .WithParameterName("selector");
    }

    //==============================================================================================
    /// <summary>
    /// GetPropertyName() should work with different property types.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void GetPropertyName_should_work_with_different_types()
    {
        var rule = new TestRule();

        var nameString = TestRule.GetName<TestClass, string?>(o => o.Name);
        var nameInt = TestRule.GetName<TestClass, int>(o => o.Value);

        nameString.Should().Be("Name");
        nameInt.Should().Be("Value");
    }

    #endregion
}
