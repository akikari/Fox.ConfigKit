//==================================================================================================
// Unit tests for generic comparable validation rules.
// Tests for GreaterThan, LessThan, Minimum, Maximum, InRange with various types.
//==================================================================================================
using FluentAssertions;
using Fox.ConfigKit.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ConfigKit.Tests;

//==============================================================================================
/// <summary>
/// Tests for generic validation rules with IComparable types.
/// </summary>
//==============================================================================================
public sealed class GenericValidationTests
{
    //==========================================================================================
    /// <summary>
    /// Test options class for decimal validation scenarios.
    /// </summary>
    //==========================================================================================
    private sealed class DecimalOptions
    {
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }

    //==========================================================================================
    /// <summary>
    /// Test options class for DateTime validation scenarios.
    /// </summary>
    //==========================================================================================
    private sealed class DateTimeOptions
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    //==========================================================================================
    /// <summary>
    /// Test options class for TimeSpan validation scenarios.
    /// </summary>
    //==========================================================================================
    private sealed class TimeSpanOptions
    {
        public TimeSpan Timeout { get; set; }
        public TimeSpan MaxDuration { get; set; }
    }

    //==========================================================================================
    /// <summary>
    /// Test options class for int validation scenarios.
    /// </summary>
    //==========================================================================================
    private sealed class IntOptions
    {
        public int Port { get; set; }
        public int MaxConnections { get; set; }
    }

    //==========================================================================================
    // GreaterThan Tests
    //==========================================================================================

    [Fact]
    public void GreaterThan_should_fail_when_decimal_is_not_greater()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DecimalOptions>(services, "Test");
        builder.GreaterThan(o => o.Price, 0.0m);

        var options = new DecimalOptions { Price = 0.0m };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("must be >");
    }

    [Fact]
    public void GreaterThan_should_pass_when_decimal_is_greater()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DecimalOptions>(services, "Test");
        builder.GreaterThan(o => o.Price, 0.0m);

        var options = new DecimalOptions { Price = 10.99m };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void GreaterThan_should_fail_when_datetime_is_not_greater()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DateTimeOptions>(services, "Test");
        var minDate = new DateTime(2024, 1, 1);
        builder.GreaterThan(o => o.StartDate, minDate);

        var options = new DateTimeOptions { StartDate = minDate };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void GreaterThan_should_pass_when_datetime_is_greater()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DateTimeOptions>(services, "Test");
        var minDate = new DateTime(2024, 1, 1);
        builder.GreaterThan(o => o.StartDate, minDate);

        var options = new DateTimeOptions { StartDate = new DateTime(2024, 6, 1) };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void GreaterThan_should_fail_when_timespan_is_not_greater()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TimeSpanOptions>(services, "Test");
        builder.GreaterThan(o => o.Timeout, TimeSpan.FromSeconds(0));

        var options = new TimeSpanOptions { Timeout = TimeSpan.Zero };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void GreaterThan_should_pass_when_timespan_is_greater()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TimeSpanOptions>(services, "Test");
        builder.GreaterThan(o => o.Timeout, TimeSpan.Zero);

        var options = new TimeSpanOptions { Timeout = TimeSpan.FromSeconds(30) };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    //==========================================================================================
    // LessThan Tests
    //==========================================================================================

    [Fact]
    public void LessThan_should_fail_when_decimal_is_not_less()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DecimalOptions>(services, "Test");
        builder.LessThan(o => o.Price, 100.0m);

        var options = new DecimalOptions { Price = 100.0m };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("must be <");
    }

    [Fact]
    public void LessThan_should_pass_when_decimal_is_less()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DecimalOptions>(services, "Test");
        builder.LessThan(o => o.Price, 100.0m);

        var options = new DecimalOptions { Price = 50.0m };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void LessThan_should_fail_when_datetime_is_not_less()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DateTimeOptions>(services, "Test");
        var maxDate = new DateTime(2024, 12, 31);
        builder.LessThan(o => o.EndDate, maxDate);

        var options = new DateTimeOptions { EndDate = maxDate };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void LessThan_should_pass_when_datetime_is_less()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DateTimeOptions>(services, "Test");
        var maxDate = new DateTime(2024, 12, 31);
        builder.LessThan(o => o.EndDate, maxDate);

        var options = new DateTimeOptions { EndDate = new DateTime(2024, 6, 1) };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void LessThan_should_fail_when_timespan_is_not_less()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TimeSpanOptions>(services, "Test");
        builder.LessThan(o => o.Timeout, TimeSpan.FromMinutes(5));

        var options = new TimeSpanOptions { Timeout = TimeSpan.FromMinutes(5) };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void LessThan_should_pass_when_timespan_is_less()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TimeSpanOptions>(services, "Test");
        builder.LessThan(o => o.Timeout, TimeSpan.FromMinutes(5));

        var options = new TimeSpanOptions { Timeout = TimeSpan.FromSeconds(30) };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    //==========================================================================================
    // Minimum Tests (NEW - Inclusive)
    //==========================================================================================

    [Fact]
    public void Minimum_should_fail_when_decimal_is_below_minimum()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DecimalOptions>(services, "Test");
        builder.Minimum(o => o.Price, 0.01m);

        var options = new DecimalOptions { Price = 0.0m };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("at least");
    }

    [Fact]
    public void Minimum_should_pass_when_decimal_equals_minimum()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DecimalOptions>(services, "Test");
        builder.Minimum(o => o.Price, 0.01m);

        var options = new DecimalOptions { Price = 0.01m };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Minimum_should_pass_when_decimal_is_above_minimum()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DecimalOptions>(services, "Test");
        builder.Minimum(o => o.Price, 0.01m);

        var options = new DecimalOptions { Price = 10.99m };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Minimum_should_fail_when_datetime_is_before_minimum()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DateTimeOptions>(services, "Test");
        var minDate = new DateTime(2024, 1, 1);
        builder.Minimum(o => o.StartDate, minDate);

        var options = new DateTimeOptions { StartDate = new DateTime(2023, 12, 31) };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void Minimum_should_pass_when_datetime_equals_minimum()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DateTimeOptions>(services, "Test");
        var minDate = new DateTime(2024, 1, 1);
        builder.Minimum(o => o.StartDate, minDate);

        var options = new DateTimeOptions { StartDate = minDate };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Minimum_should_fail_when_timespan_is_below_minimum()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TimeSpanOptions>(services, "Test");
        builder.Minimum(o => o.Timeout, TimeSpan.FromSeconds(1));

        var options = new TimeSpanOptions { Timeout = TimeSpan.Zero };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void Minimum_should_pass_when_timespan_equals_minimum()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TimeSpanOptions>(services, "Test");
        var minTimeout = TimeSpan.FromSeconds(1);
        builder.Minimum(o => o.Timeout, minTimeout);

        var options = new TimeSpanOptions { Timeout = minTimeout };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Minimum_should_pass_when_int_equals_minimum()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<IntOptions>(services, "Test");
        builder.Minimum(o => o.Port, 1024);

        var options = new IntOptions { Port = 1024 };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    //==========================================================================================
    // Maximum Tests (NEW - Inclusive)
    //==========================================================================================

    [Fact]
    public void Maximum_should_fail_when_decimal_is_above_maximum()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DecimalOptions>(services, "Test");
        builder.Maximum(o => o.Price, 100.0m);

        var options = new DecimalOptions { Price = 100.01m };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("at most");
    }

    [Fact]
    public void Maximum_should_pass_when_decimal_equals_maximum()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DecimalOptions>(services, "Test");
        builder.Maximum(o => o.Price, 100.0m);

        var options = new DecimalOptions { Price = 100.0m };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Maximum_should_pass_when_decimal_is_below_maximum()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DecimalOptions>(services, "Test");
        builder.Maximum(o => o.Price, 100.0m);

        var options = new DecimalOptions { Price = 50.0m };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Maximum_should_fail_when_datetime_is_after_maximum()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DateTimeOptions>(services, "Test");
        var maxDate = new DateTime(2024, 12, 31);
        builder.Maximum(o => o.EndDate, maxDate);

        var options = new DateTimeOptions { EndDate = new DateTime(2025, 1, 1) };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void Maximum_should_pass_when_datetime_equals_maximum()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DateTimeOptions>(services, "Test");
        var maxDate = new DateTime(2024, 12, 31);
        builder.Maximum(o => o.EndDate, maxDate);

        var options = new DateTimeOptions { EndDate = maxDate };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Maximum_should_fail_when_timespan_is_above_maximum()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TimeSpanOptions>(services, "Test");
        builder.Maximum(o => o.Timeout, TimeSpan.FromMinutes(5));

        var options = new TimeSpanOptions { Timeout = TimeSpan.FromMinutes(6) };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void Maximum_should_pass_when_timespan_equals_maximum()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TimeSpanOptions>(services, "Test");
        var maxTimeout = TimeSpan.FromMinutes(5);
        builder.Maximum(o => o.Timeout, maxTimeout);

        var options = new TimeSpanOptions { Timeout = maxTimeout };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Maximum_should_pass_when_int_equals_maximum()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<IntOptions>(services, "Test");
        builder.Maximum(o => o.Port, 65535);

        var options = new IntOptions { Port = 65535 };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    //==========================================================================================
    // InRange Tests (Generic)
    //==========================================================================================

    [Fact]
    public void InRange_should_fail_when_decimal_is_below_range()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DecimalOptions>(services, "Test");
        builder.InRange(o => o.Price, 0.01m, 100.0m);

        var options = new DecimalOptions { Price = 0.0m };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("between");
    }

    [Fact]
    public void InRange_should_fail_when_decimal_is_above_range()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DecimalOptions>(services, "Test");
        builder.InRange(o => o.Price, 0.01m, 100.0m);

        var options = new DecimalOptions { Price = 100.01m };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void InRange_should_pass_when_decimal_is_at_minimum_boundary()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DecimalOptions>(services, "Test");
        builder.InRange(o => o.Price, 0.01m, 100.0m);

        var options = new DecimalOptions { Price = 0.01m };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void InRange_should_pass_when_decimal_is_at_maximum_boundary()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DecimalOptions>(services, "Test");
        builder.InRange(o => o.Price, 0.01m, 100.0m);

        var options = new DecimalOptions { Price = 100.0m };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void InRange_should_pass_when_decimal_is_within_range()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DecimalOptions>(services, "Test");
        builder.InRange(o => o.Price, 0.01m, 100.0m);

        var options = new DecimalOptions { Price = 50.0m };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void InRange_should_fail_when_datetime_is_before_range()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DateTimeOptions>(services, "Test");
        var minDate = new DateTime(2024, 1, 1);
        var maxDate = new DateTime(2024, 12, 31);
        builder.InRange(o => o.StartDate, minDate, maxDate);

        var options = new DateTimeOptions { StartDate = new DateTime(2023, 12, 31) };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void InRange_should_fail_when_datetime_is_after_range()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DateTimeOptions>(services, "Test");
        var minDate = new DateTime(2024, 1, 1);
        var maxDate = new DateTime(2024, 12, 31);
        builder.InRange(o => o.StartDate, minDate, maxDate);

        var options = new DateTimeOptions { StartDate = new DateTime(2025, 1, 1) };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void InRange_should_pass_when_datetime_is_at_range_boundaries()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DateTimeOptions>(services, "Test");
        var minDate = new DateTime(2024, 1, 1);
        var maxDate = new DateTime(2024, 12, 31);
        builder.InRange(o => o.StartDate, minDate, maxDate);

        var optionsMin = new DateTimeOptions { StartDate = minDate };
        var errorsMin = builder.Validate(optionsMin).ToList();
        errorsMin.Should().BeEmpty();

        var optionsMax = new DateTimeOptions { StartDate = maxDate };
        var errorsMax = builder.Validate(optionsMax).ToList();
        errorsMax.Should().BeEmpty();
    }

    [Fact]
    public void InRange_should_pass_when_datetime_is_within_range()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DateTimeOptions>(services, "Test");
        var minDate = new DateTime(2024, 1, 1);
        var maxDate = new DateTime(2024, 12, 31);
        builder.InRange(o => o.StartDate, minDate, maxDate);

        var options = new DateTimeOptions { StartDate = new DateTime(2024, 6, 15) };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void InRange_should_fail_when_timespan_is_below_range()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TimeSpanOptions>(services, "Test");
        builder.InRange(o => o.Timeout, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(5));

        var options = new TimeSpanOptions { Timeout = TimeSpan.Zero };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void InRange_should_fail_when_timespan_is_above_range()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TimeSpanOptions>(services, "Test");
        builder.InRange(o => o.Timeout, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(5));

        var options = new TimeSpanOptions { Timeout = TimeSpan.FromMinutes(6) };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
    }

    [Fact]
    public void InRange_should_pass_when_timespan_is_at_range_boundaries()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TimeSpanOptions>(services, "Test");
        var minTimeout = TimeSpan.FromSeconds(1);
        var maxTimeout = TimeSpan.FromMinutes(5);
        builder.InRange(o => o.Timeout, minTimeout, maxTimeout);

        var optionsMin = new TimeSpanOptions { Timeout = minTimeout };
        var errorsMin = builder.Validate(optionsMin).ToList();
        errorsMin.Should().BeEmpty();

        var optionsMax = new TimeSpanOptions { Timeout = maxTimeout };
        var errorsMax = builder.Validate(optionsMax).ToList();
        errorsMax.Should().BeEmpty();
    }

    [Fact]
    public void InRange_should_pass_when_timespan_is_within_range()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TimeSpanOptions>(services, "Test");
        builder.InRange(o => o.Timeout, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(5));

        var options = new TimeSpanOptions { Timeout = TimeSpan.FromSeconds(30) };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    //==========================================================================================
    // Backward Compatibility Tests (int still works)
    //==========================================================================================

    [Fact]
    public void GreaterThan_should_work_with_int_for_backward_compatibility()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<IntOptions>(services, "Test");
        builder.GreaterThan(o => o.Port, 1024);

        var options = new IntOptions { Port = 8080 };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void InRange_should_work_with_int_for_backward_compatibility()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<IntOptions>(services, "Test");
        builder.InRange(o => o.Port, 1024, 65535);

        var options = new IntOptions { Port = 8080 };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    //==========================================================================================
    // Custom Message Tests
    //==========================================================================================

    [Fact]
    public void Minimum_should_use_custom_message()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DecimalOptions>(services, "Test");
        builder.Minimum(o => o.Price, 0.01m, "Price cannot be zero or negative");

        var options = new DecimalOptions { Price = 0.0m };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("Price cannot be zero or negative");
    }

    [Fact]
    public void Maximum_should_use_custom_message()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<TimeSpanOptions>(services, "Test");
        builder.Maximum(o => o.Timeout, TimeSpan.FromMinutes(5), "Timeout too long");

        var options = new TimeSpanOptions { Timeout = TimeSpan.FromMinutes(10) };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("Timeout too long");
    }
}
