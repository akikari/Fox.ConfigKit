//==================================================================================================
// Tests for ConfigValidationBuilder core functionality.
// Verifies builder pattern and rule management.
//==================================================================================================
using FluentAssertions;
using Fox.ConfigKit.Errors;
using Fox.ConfigKit.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ConfigKit.Tests;

//==================================================================================================
/// <summary>
/// Tests for <see cref="ConfigValidationBuilder{T}"/>.
/// </summary>
//==================================================================================================
public sealed class ConfigValidationBuilderTests
{
    #region Test Classes

    private sealed class TestConfig
    {
        public string? Value { get; set; }
    }

    private sealed class TestRule(bool shouldFail = false) : IValidationRule<TestConfig>
    {
        public ConfigValidationError? Validate(TestConfig options, string sectionName)
        {
            if (shouldFail)
            {
                return new ConfigValidationError("Test:Value", "Test error");
            }

            return null;
        }
    }

    #endregion

    #region AddRule Tests

    //==============================================================================================
    /// <summary>
    /// AddRule() should add rule and return builder for chaining.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddRule_should_add_rule_and_return_builder()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        var rule = new TestRule();

        var result = builder.AddRule(rule);

        result.Should().BeSameAs(builder);
    }

    //==============================================================================================
    /// <summary>
    /// AddRule() explicit interface implementation should work.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddRule_explicit_interface_should_work()
    {
        var services = new ServiceCollection();
        IValidationBuilder<TestConfig> builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        var rule = new TestRule();

        var result = builder.AddRule(rule);

        result.Should().BeSameAs(builder);
    }

    //==============================================================================================
    /// <summary>
    /// AddRule() should allow multiple rules.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddRule_should_allow_multiple_rules()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");

        builder.AddRule(new TestRule(true))
               .AddRule(new TestRule(true));

        var errors = builder.Validate(new TestConfig()).ToList();

        errors.Should().HaveCount(2);
    }

    #endregion

    #region Validate Tests

    //==============================================================================================
    /// <summary>
    /// Validate() should return empty when all rules pass.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void Validate_should_return_empty_when_all_rules_pass()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        builder.AddRule(new TestRule(false));

        var errors = builder.Validate(new TestConfig());

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// Validate() should return errors when rules fail.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void Validate_should_return_errors_when_rules_fail()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        builder.AddRule(new TestRule(true));

        var errors = builder.Validate(new TestConfig()).ToList();

        errors.Should().ContainSingle();
        errors[0].Message.Should().Be("Test error");
    }

    //==============================================================================================
    /// <summary>
    /// Validate() should execute all rules.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void Validate_should_execute_all_rules()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        builder.AddRule(new TestRule(true))
               .AddRule(new TestRule(false))
               .AddRule(new TestRule(true));

        var errors = builder.Validate(new TestConfig()).ToList();

        errors.Should().HaveCount(2);
    }

    //==============================================================================================
    /// <summary>
    /// Validate() should work with no rules.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void Validate_should_work_with_no_rules()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");

        var errors = builder.Validate(new TestConfig());

        errors.Should().BeEmpty();
    }

    #endregion

    #region Services Property Tests

    //==============================================================================================
    /// <summary>
    /// Services property should return the service collection.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void Services_property_should_return_service_collection()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");

        builder.Services.Should().BeSameAs(services);
    }

    #endregion
}
