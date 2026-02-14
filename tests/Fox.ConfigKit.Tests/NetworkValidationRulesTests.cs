//==================================================================================================
// Unit tests for network validation rules.
// Integration tests for UrlReachableRule and PortAvailableRule using real network operations.
//==================================================================================================
using System.Net;
using System.Net.Sockets;
using FluentAssertions;
using Fox.ConfigKit.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ConfigKit.Tests;

//==============================================================================================
/// <summary>
/// Tests for network validation rules.
/// </summary>
//==============================================================================================
public sealed class NetworkValidationRulesTests : IDisposable
{
    #region Fields

    private TcpListener? testListener;

    #endregion

    #region UrlReachable Tests

    //==========================================================================================
    /// <summary>
    /// Test options class for URL validation scenarios.
    /// </summary>
    //==========================================================================================
    private sealed class UrlTestOptions
    {
        public string? Url { get; set; }
    }

    [Fact]
    public void UrlReachable_should_pass_when_url_is_reachable()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<UrlTestOptions>(services, "Test");
        builder.UrlReachable(o => o.Url, timeout: TimeSpan.FromSeconds(10));

        var options = new UrlTestOptions { Url = "https://www.example.com" };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void UrlReachable_should_fail_when_url_is_null()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<UrlTestOptions>(services, "Test");
        builder.UrlReachable(o => o.Url);

        var options = new UrlTestOptions { Url = null };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("URL is not specified");
    }

    [Fact]
    public void UrlReachable_should_fail_when_url_is_empty()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<UrlTestOptions>(services, "Test");
        builder.UrlReachable(o => o.Url);

        var options = new UrlTestOptions { Url = string.Empty };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("URL is not specified");
    }

    [Fact]
    public void UrlReachable_should_fail_when_url_is_whitespace()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<UrlTestOptions>(services, "Test");
        builder.UrlReachable(o => o.Url);

        var options = new UrlTestOptions { Url = "   " };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("URL is not specified");
    }

    [Fact]
    public void UrlReachable_should_fail_when_url_format_is_invalid()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<UrlTestOptions>(services, "Test");
        builder.UrlReachable(o => o.Url);

        var options = new UrlTestOptions { Url = "not-a-valid-url" };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("Invalid URL format");
    }

    [Fact]
    public void UrlReachable_should_fail_when_url_is_unreachable()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<UrlTestOptions>(services, "Test");
        builder.UrlReachable(o => o.Url, timeout: TimeSpan.FromSeconds(5));

        var options = new UrlTestOptions { Url = "https://this-domain-definitely-does-not-exist-12345.com" };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("Failed to reach URL");
    }

    [Fact]
    public void UrlReachable_should_use_custom_message_when_provided()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<UrlTestOptions>(services, "Test");
        builder.UrlReachable(o => o.Url, timeout: TimeSpan.FromSeconds(5), message: "Custom URL error message");

        var options = new UrlTestOptions { Url = "https://this-domain-definitely-does-not-exist-12345.com" };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Be("Custom URL error message");
    }

    [Fact]
    public void UrlReachable_should_provide_suggestion_for_unreachable_url()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<UrlTestOptions>(services, "Test");
        builder.UrlReachable(o => o.Url, timeout: TimeSpan.FromSeconds(5));

        var options = new UrlTestOptions { Url = "https://this-domain-definitely-does-not-exist-12345.com" };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Suggestions.Should().Contain(s => s.Contains("Check URL availability"));
    }

    #endregion

    #region PortAvailable Tests

    //==========================================================================================
    /// <summary>
    /// Test options class for port validation scenarios.
    /// </summary>
    //==========================================================================================
    private sealed class PortTestOptions
    {
        public int Port { get; set; }
    }

    [Fact]
    public void PortAvailable_should_pass_when_port_is_available()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<PortTestOptions>(services, "Test");
        builder.PortAvailable(o => o.Port);

        var availablePort = GetAvailablePort();
        var options = new PortTestOptions { Port = availablePort };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void PortAvailable_should_fail_when_port_is_in_use()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<PortTestOptions>(services, "Test");
        builder.PortAvailable(o => o.Port);

        var port = GetAvailablePort();
        testListener = new TcpListener(IPAddress.Loopback, port);
        testListener.Start();

        var options = new PortTestOptions { Port = port };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain($"Port {port} is already in use");
    }

    [Fact]
    public void PortAvailable_should_fail_when_port_is_below_valid_range()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<PortTestOptions>(services, "Test");
        builder.PortAvailable(o => o.Port);

        var options = new PortTestOptions { Port = 0 };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("Port number must be between 1 and 65535");
    }

    [Fact]
    public void PortAvailable_should_fail_when_port_is_negative()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<PortTestOptions>(services, "Test");
        builder.PortAvailable(o => o.Port);

        var options = new PortTestOptions { Port = -1 };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("Port number must be between 1 and 65535");
    }

    [Fact]
    public void PortAvailable_should_fail_when_port_is_above_valid_range()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<PortTestOptions>(services, "Test");
        builder.PortAvailable(o => o.Port);

        var options = new PortTestOptions { Port = 65536 };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("Port number must be between 1 and 65535");
    }

    [Fact]
    public void PortAvailable_should_use_custom_message_when_provided()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<PortTestOptions>(services, "Test");
        builder.PortAvailable(o => o.Port, message: "Custom port error message");

        var port = GetAvailablePort();
        testListener = new TcpListener(IPAddress.Loopback, port);
        testListener.Start();

        var options = new PortTestOptions { Port = port };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Be("Custom port error message");
    }

    [Fact]
    public void PortAvailable_should_provide_suggestion_for_occupied_port()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<PortTestOptions>(services, "Test");
        builder.PortAvailable(o => o.Port);

        var port = GetAvailablePort();
        testListener = new TcpListener(IPAddress.Loopback, port);
        testListener.Start();

        var options = new PortTestOptions { Port = port };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Suggestions.Should().Contain(s => s.Contains("Choose a different port"));
    }

    #endregion

    #region Helper Methods

    //==========================================================================================
    /// <summary>
    /// Gets an available port number.
    /// </summary>
    /// <returns>An available port number.</returns>
    //==========================================================================================
    private static int GetAvailablePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    #endregion

    #region Cleanup

    public void Dispose()
    {
        testListener?.Stop();
    }

    #endregion
}
