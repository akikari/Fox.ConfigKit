//==================================================================================================
// Unit tests for property-to-property comparison validation rules.
// Tests GreaterThanProperty, LessThanProperty, MinimumProperty, and MaximumProperty validators.
//==================================================================================================
using FluentAssertions;
using Fox.ConfigKit.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ConfigKit.Tests;

public class PropertyComparisonValidationExtensionsTests
{
    #region Test Classes

    private sealed class TestConfig
    {
        public int StartValue { get; set; }
        public int EndValue { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan MinDuration { get; set; }
        public TimeSpan MaxDuration { get; set; }
    }

    #endregion

    #region GreaterThanProperty Tests

    //==============================================================================================
    /// <summary>
    /// GreaterThanProperty should succeed when property value is greater than comparison property.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void GreaterThanProperty_should_succeed_when_value_is_greater()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        builder.GreaterThanProperty(c => c.EndValue, c => c.StartValue, "EndValue must be > StartValue");

        var config = new TestConfig { StartValue = 10, EndValue = 20 };
        var errors = builder.Validate(config).ToList();

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// GreaterThanProperty should fail when property value equals comparison property.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void GreaterThanProperty_should_fail_when_values_are_equal()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        builder.GreaterThanProperty(c => c.EndValue, c => c.StartValue, "EndValue must be > StartValue");

        var config = new TestConfig { StartValue = 10, EndValue = 10 };
        var errors = builder.Validate(config).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("EndValue must be > StartValue");
    }

    //==============================================================================================
    /// <summary>
    /// GreaterThanProperty should fail when property value is less than comparison property.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void GreaterThanProperty_should_fail_when_value_is_less()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        builder.GreaterThanProperty(c => c.EndValue, c => c.StartValue);

        var config = new TestConfig { StartValue = 20, EndValue = 10 };
        var errors = builder.Validate(config).ToList();

        errors.Should().HaveCount(1);
    }

    #endregion

    #region LessThanProperty Tests

    //==============================================================================================
    /// <summary>
    /// LessThanProperty should succeed when property value is less than comparison property.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void LessThanProperty_should_succeed_when_value_is_less()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        builder.LessThanProperty(c => c.MinPrice, c => c.MaxPrice, "MinPrice must be < MaxPrice");

        var config = new TestConfig { MinPrice = 10.0m, MaxPrice = 20.0m };
        var errors = builder.Validate(config).ToList();

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// LessThanProperty should fail when property value equals comparison property.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void LessThanProperty_should_fail_when_values_are_equal()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        builder.LessThanProperty(c => c.MinPrice, c => c.MaxPrice, "MinPrice must be < MaxPrice");

        var config = new TestConfig { MinPrice = 10.0m, MaxPrice = 10.0m };
        var errors = builder.Validate(config).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("MinPrice must be < MaxPrice");
    }

    //==============================================================================================
    /// <summary>
    /// LessThanProperty should fail when property value is greater than comparison property.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void LessThanProperty_should_fail_when_value_is_greater()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        builder.LessThanProperty(c => c.MinPrice, c => c.MaxPrice);

        var config = new TestConfig { MinPrice = 20.0m, MaxPrice = 10.0m };
        var errors = builder.Validate(config).ToList();

        errors.Should().HaveCount(1);
    }

    #endregion

    #region MinimumProperty Tests

    //==============================================================================================
    /// <summary>
    /// MinimumProperty should succeed when property value is greater than comparison property.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void MinimumProperty_should_succeed_when_value_is_greater()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        builder.MinimumProperty(c => c.EndDate, c => c.StartDate, "EndDate must be >= StartDate");

        var config = new TestConfig { StartDate = new DateTime(2024, 1, 1), EndDate = new DateTime(2024, 12, 31) };
        var errors = builder.Validate(config).ToList();

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// MinimumProperty should succeed when property value equals comparison property.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void MinimumProperty_should_succeed_when_values_are_equal()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        builder.MinimumProperty(c => c.EndDate, c => c.StartDate, "EndDate must be >= StartDate");

        var config = new TestConfig { StartDate = new DateTime(2024, 1, 1), EndDate = new DateTime(2024, 1, 1) };
        var errors = builder.Validate(config).ToList();

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// MinimumProperty should fail when property value is less than comparison property.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void MinimumProperty_should_fail_when_value_is_less()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        builder.MinimumProperty(c => c.EndDate, c => c.StartDate, "EndDate must be >= StartDate");

        var config = new TestConfig { StartDate = new DateTime(2024, 12, 31), EndDate = new DateTime(2024, 1, 1) };
        var errors = builder.Validate(config).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("EndDate must be >= StartDate");
    }

    #endregion

    #region MaximumProperty Tests

    //==============================================================================================
    /// <summary>
    /// MaximumProperty should succeed when property value is less than comparison property.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void MaximumProperty_should_succeed_when_value_is_less()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        builder.MaximumProperty(c => c.MinDuration, c => c.MaxDuration, "MinDuration must be <= MaxDuration");

        var config = new TestConfig { MinDuration = TimeSpan.FromMinutes(5), MaxDuration = TimeSpan.FromMinutes(30) };
        var errors = builder.Validate(config).ToList();

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// MaximumProperty should succeed when property value equals comparison property.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void MaximumProperty_should_succeed_when_values_are_equal()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        builder.MaximumProperty(c => c.MinDuration, c => c.MaxDuration, "MinDuration must be <= MaxDuration");

        var config = new TestConfig { MinDuration = TimeSpan.FromMinutes(15), MaxDuration = TimeSpan.FromMinutes(15) };
        var errors = builder.Validate(config).ToList();

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// MaximumProperty should fail when property value is greater than comparison property.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void MaximumProperty_should_fail_when_value_is_greater()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        builder.MaximumProperty(c => c.MinDuration, c => c.MaxDuration, "MinDuration must be <= MaxDuration");

        var config = new TestConfig { MinDuration = TimeSpan.FromMinutes(30), MaxDuration = TimeSpan.FromMinutes(5) };
        var errors = builder.Validate(config).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("MinDuration must be <= MaxDuration");
    }

    #endregion

    #region Real-World Scenarios

    //==============================================================================================
    /// <summary>
    /// Property comparison should work for batch size validation scenario.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void Property_comparison_should_validate_batch_processing_configuration()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        builder.Minimum(c => c.StartValue, 0, "RecordsPerRun cannot be negative")
            .InRange(c => c.EndValue, 1, 100000, "BatchSize must be between 1 and 100000")
            .MinimumProperty(c => c.StartValue, c => c.EndValue, "RecordsPerRun must be >= BatchSize (or 0 for no limit)");

        var config = new TestConfig { StartValue = 10000, EndValue = 1000 };
        var errors = builder.Validate(config).ToList();

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// Property comparison should fail when RecordsPerRun is less than BatchSize.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void Property_comparison_should_fail_when_records_per_run_is_less_than_batch_size()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TestConfig>(services, "Test");
        builder.MinimumProperty(c => c.StartValue, c => c.EndValue, "RecordsPerRun must be >= BatchSize");

        var config = new TestConfig { StartValue = 500, EndValue = 1000 };
        var errors = builder.Validate(config).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("RecordsPerRun must be >= BatchSize");
    }

    #endregion
}
