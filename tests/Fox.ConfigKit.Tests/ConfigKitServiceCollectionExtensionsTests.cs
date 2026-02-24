//==================================================================================================
// Tests for ConfigKit service collection extension methods.
// Verifies dependency injection and options validation integration.
//==================================================================================================
using FluentAssertions;
using Fox.ConfigKit.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fox.ConfigKit.Tests;

//==================================================================================================
/// <summary>
/// Tests for <see cref="ConfigKitServiceCollectionExtensions"/>.
/// </summary>
//==================================================================================================
public sealed class ConfigKitServiceCollectionExtensionsTests
{
    #region Test Classes

    private sealed class TestConfig
    {
        public string? Name { get; set; }
        public int Port { get; set; }
    }

    #endregion

    #region AddConfigKit Tests

    //==============================================================================================
    /// <summary>
    /// AddConfigKit() should return the service collection.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddConfigKit_should_return_service_collection()
    {
        var services = new ServiceCollection();

        var result = services.AddConfigKit();

        result.Should().BeSameAs(services);
    }

    //==============================================================================================
    /// <summary>
    /// AddConfigKit() should throw ArgumentNullException when services is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddConfigKit_should_throw_when_services_is_null()
    {
        IServiceCollection services = null!;

        var act = () => services.AddConfigKit();

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("services");
    }

    //==============================================================================================
    /// <summary>
    /// AddConfigKit{T}() should return ConfigValidationBuilder.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddConfigKit_generic_should_return_validation_builder()
    {
        var services = new ServiceCollection();

        var builder = services.AddConfigKit<TestConfig>("Test");

        builder.Should().NotBeNull();
        builder.Should().BeOfType<ConfigValidationBuilder<TestConfig>>();
    }

    //==============================================================================================
    /// <summary>
    /// AddConfigKit{T}() should throw ArgumentNullException when services is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddConfigKit_generic_should_throw_when_services_is_null()
    {
        IServiceCollection services = null!;

        var act = () => services.AddConfigKit<TestConfig>("Test");

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("services");
    }

    //==============================================================================================
    /// <summary>
    /// AddConfigKit{T}() should throw ArgumentNullException when sectionName is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddConfigKit_generic_should_throw_when_section_name_is_null()
    {
        var services = new ServiceCollection();

        var act = () => services.AddConfigKit<TestConfig>(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("sectionName");
    }

    //==============================================================================================
    /// <summary>
    /// AddConfigKit{T}() should register options services.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddConfigKit_generic_should_register_options()
    {
        var services = new ServiceCollection();

        services.AddConfigKit<TestConfig>("Test");

        services.Should().Contain(sd => sd.ServiceType == typeof(IConfigureOptions<TestConfig>));
    }

    #endregion

    #region ValidateOnStartup Tests

    //==============================================================================================
    /// <summary>
    /// ValidateOnStartup() should return the service collection.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateOnStartup_should_return_service_collection()
    {
        var services = new ServiceCollection();
        var builder = services.AddConfigKit<TestConfig>("Test");

        var result = builder.ValidateOnStartup();

        result.Should().BeSameAs(services);
    }

    //==============================================================================================
    /// <summary>
    /// ValidateOnStartup() should throw ArgumentNullException when builder is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateOnStartup_should_throw_when_builder_is_null()
    {
        ConfigValidationBuilder<TestConfig> builder = null!;

        var act = () => builder.ValidateOnStartup();

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("builder");
    }

    //==============================================================================================
    /// <summary>
    /// ValidateOnStartup() should register IValidateOptions.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateOnStartup_should_register_validate_options()
    {
        var services = new ServiceCollection();

        services.AddConfigKit<TestConfig>("Test")
            .NotEmpty(o => o.Name, "Name is required")
            .ValidateOnStartup();

        services.Should().Contain(sd => sd.ServiceType == typeof(IValidateOptions<TestConfig>));
    }

    #endregion
}
