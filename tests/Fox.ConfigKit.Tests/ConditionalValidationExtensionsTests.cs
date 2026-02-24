//==================================================================================================
// Tests for conditional validation (When/Unless) extension methods.
// Verifies cross-property validation scenarios.
//==================================================================================================
using FluentAssertions;
using Fox.ConfigKit.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ConfigKit.Tests;

//==================================================================================================
/// <summary>
/// Tests for <see cref="ConditionalValidationExtensions"/>.
/// </summary>
//==================================================================================================
public sealed class ConditionalValidationExtensionsTests
{
    #region Test Classes

    private sealed class HttpsConfig
    {
        public bool UseHttps { get; set; }
        public string? CertificatePath { get; set; }
        public int Port { get; set; }
    }

    private sealed class DatabaseConfig
    {
        public bool UseConnectionPool { get; set; }
        public int MaxPoolSize { get; set; }
        public int MinPoolSize { get; set; }
    }

    #endregion

    #region When Tests

    //==============================================================================================
    /// <summary>
    /// When() should apply validation rules only when condition is true.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void When_should_apply_rules_when_condition_is_true()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<HttpsConfig>(services, "Test");

        builder.When(o => o.UseHttps, https => https
            .NotNull(o => o.CertificatePath, "Certificate path required for HTTPS"));

        var errors = builder.Validate(new HttpsConfig { UseHttps = true, CertificatePath = null });

        errors.Should().ContainSingle()
            .Which.Message.Should().Contain("Certificate path required for HTTPS");
    }

    //==============================================================================================
    /// <summary>
    /// When() should skip validation rules when condition is false.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void When_should_skip_rules_when_condition_is_false()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<HttpsConfig>(services, "Test");

        builder.When(o => o.UseHttps, https => https
            .NotNull(o => o.CertificatePath, "Certificate path required for HTTPS"));

        var errors = builder.Validate(new HttpsConfig { UseHttps = false, CertificatePath = null });

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// When() should support multiple rules in conditional block.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void When_should_support_multiple_rules()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<HttpsConfig>(services, "Test");

        builder.When(o => o.UseHttps, https => https
            .NotNull(o => o.CertificatePath, "Certificate required")
            .GreaterThan(o => o.Port, 0, "Port must be positive"));

        var errors = builder.Validate(new HttpsConfig { UseHttps = true, CertificatePath = null, Port = -1 });

        errors.Should().HaveCount(2);
        errors.Select(e => e.Message).Should().Contain("Certificate required");
        errors.Select(e => e.Message).Should().Contain("Port must be positive");
    }

    //==============================================================================================
    /// <summary>
    /// When() should support nested conditions.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void When_should_support_nested_conditions()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DatabaseConfig>(services, "Test");

        builder.When(o => o.UseConnectionPool, pool => pool
            .GreaterThan(o => o.MaxPoolSize, 0, "Max pool size must be positive")
            .When(o => o.MaxPoolSize > 0, inner => inner
                .Maximum(o => o.MinPoolSize, 10, "Min must be <= 10")));

        var errors = builder.Validate(new DatabaseConfig
        {
            UseConnectionPool = true,
            MaxPoolSize = 10,
            MinPoolSize = 20
        });

        errors.Should().ContainSingle()
            .Which.Message.Should().Contain("Min must be <= 10");
    }

    //==============================================================================================
    /// <summary>
    /// When() should throw ArgumentNullException when builder is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void When_should_throw_when_builder_is_null()
    {
        ConfigValidationBuilder<HttpsConfig> builder = null!;

        var act = () => builder.When(o => o.UseHttps, https => https.NotNull(o => o.CertificatePath));

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("builder");
    }

    //==============================================================================================
    /// <summary>
    /// When() should throw ArgumentNullException when condition is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void When_should_throw_when_condition_is_null()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<HttpsConfig>(services, "Test");

        var act = () => builder.When(null!, https => https.NotNull(o => o.CertificatePath));

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("condition");
    }

    //==============================================================================================
    /// <summary>
    /// When() should throw ArgumentNullException when configure action is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void When_should_throw_when_configure_is_null()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<HttpsConfig>(services, "Test");

        var act = () => builder.When(o => o.UseHttps, null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("configure");
    }

    //==============================================================================================
    /// <summary>
    /// When() should return the same builder instance for method chaining.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void When_should_return_same_builder_for_chaining()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<HttpsConfig>(services, "Test");

        var result = builder.When(o => o.UseHttps, https => https.NotNull(o => o.CertificatePath));

        result.Should().BeSameAs(builder);
    }

    //==============================================================================================
    /// <summary>
    /// When() should work with complex expressions.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void When_should_work_with_complex_expressions()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<HttpsConfig>(services, "Test");

        builder.When(o => o.UseHttps && o.Port > 0, https => https
            .NotNull(o => o.CertificatePath, "Certificate required for HTTPS"));

        var errorsTrue = builder.Validate(new HttpsConfig { UseHttps = true, Port = 443, CertificatePath = null });
        var errorsFalse1 = builder.Validate(new HttpsConfig { UseHttps = false, Port = 443, CertificatePath = null });
        var errorsFalse2 = builder.Validate(new HttpsConfig { UseHttps = true, Port = 0, CertificatePath = null });

        errorsTrue.Should().ContainSingle();
        errorsFalse1.Should().BeEmpty();
        errorsFalse2.Should().BeEmpty();
    }

    #endregion
}
