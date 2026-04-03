//==================================================================================================
// Unit tests for collection validation extensions.
// Tests ValidateEach extension method for validating collection items.
//==================================================================================================
using FluentAssertions;
using Fox.ConfigKit.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ConfigKit.Tests;

public class CollectionValidationExtensionsTests
{
    #region Test Classes

    private sealed class ServerConfig
    {
        public List<EndpointConfig> Endpoints { get; set; } = [];
    }

    private sealed class EndpointConfig
    {
        public string Url { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool Enabled { get; set; }
    }

    private sealed class LogsConfig
    {
        public List<LogConfig> Logs { get; set; } = [];
    }

    private sealed class LogConfig
    {
        public string Name { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public int BatchSize { get; set; }
        public bool Enabled { get; set; }
    }

    #endregion

    #region ValidateEach Tests

    //==============================================================================================
    /// <summary>
    /// ValidateEach should succeed when all items are valid.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateEach_should_succeed_when_all_items_are_valid()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ServerConfig>(services, "Server");

        builder.ValidateEach(c => c.Endpoints,
            itemBuilder => itemBuilder
                .NotEmpty(e => e.Url, "URL is required")
                .InRange(e => e.Port, 1, 65535, "Port must be between 1 and 65535"));

        var config = new ServerConfig
        {
            Endpoints =
            [
                new EndpointConfig { Url = "https://api1.example.com", Port = 443 },
                new EndpointConfig { Url = "https://api2.example.com", Port = 8080 }
            ]
        };

        var errors = builder.Validate(config).ToList();

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// ValidateEach should fail when an item is invalid.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateEach_should_fail_when_item_is_invalid()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ServerConfig>(services, "Server");

        builder.ValidateEach(c => c.Endpoints,
            itemBuilder => itemBuilder
                .NotEmpty(e => e.Url, "URL is required")
                .InRange(e => e.Port, 1, 65535, "Port must be between 1 and 65535"));

        var config = new ServerConfig
        {
            Endpoints =
            [
                new EndpointConfig { Url = "https://api1.example.com", Port = 443 },
                new EndpointConfig { Url = "", Port = 99999 }
            ]
        };

        var errors = builder.Validate(config).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("Endpoints contains invalid items");
    }

    //==============================================================================================
    /// <summary>
    /// ValidateEach should succeed when collection is empty and minCount is 0.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateEach_should_succeed_when_collection_is_empty_and_minCount_is_zero()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ServerConfig>(services, "Server");

        builder.ValidateEach(c => c.Endpoints,
            itemBuilder => itemBuilder.NotEmpty(e => e.Url, "URL is required"),
            minCount: 0);

        var config = new ServerConfig { Endpoints = [] };

        var errors = builder.Validate(config).ToList();

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// ValidateEach should fail when collection has fewer items than minCount.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateEach_should_fail_when_collection_has_fewer_items_than_minCount()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ServerConfig>(services, "Server");

        builder.ValidateEach(c => c.Endpoints,
            itemBuilder => itemBuilder.NotEmpty(e => e.Url, "URL is required"),
            minCount: 2,
            emptyMessage: "At least 2 endpoints are required");

        var config = new ServerConfig
        {
            Endpoints = [new EndpointConfig { Url = "https://api.example.com", Port = 443 }]
        };

        var errors = builder.Validate(config).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("At least 2 endpoints are required");
    }

    //==============================================================================================
    /// <summary>
    /// ValidateEach should succeed when collection has exactly minCount items.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateEach_should_succeed_when_collection_has_exactly_minCount_items()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ServerConfig>(services, "Server");

        builder.ValidateEach(c => c.Endpoints,
            itemBuilder => itemBuilder
                .NotEmpty(e => e.Url, "URL is required")
                .InRange(e => e.Port, 1, 65535, "Port must be valid"),
            minCount: 2);

        var config = new ServerConfig
        {
            Endpoints =
            [
                new EndpointConfig { Url = "https://api1.example.com", Port = 443 },
                new EndpointConfig { Url = "https://api2.example.com", Port = 8080 }
            ]
        };

        var errors = builder.Validate(config).ToList();

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// ValidateEach should support filtering with LINQ Where clause.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateEach_should_support_filtering_with_linq_where()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<LogsConfig>(services, "Logs");

        builder.ValidateEach(c => c.Logs.Where(l => l.Enabled),
            itemBuilder => itemBuilder
                .NotEmpty(l => l.Name, "Log name is required")
                .NotEmpty(l => l.TableName, "Table name is required")
                .InRange(l => l.BatchSize, 1, 10000, "Batch size must be between 1 and 10000"),
            minCount: 1,
            emptyMessage: "At least one enabled log is required");

        var config = new LogsConfig
        {
            Logs =
            [
                new LogConfig { Name = "Log1", TableName = "Table1", BatchSize = 100, Enabled = true },
                new LogConfig { Name = "", TableName = "", BatchSize = 0, Enabled = false }
            ]
        };

        var errors = builder.Validate(config).ToList();

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// ValidateEach should fail when filtered collection is empty and minCount is greater than 0.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateEach_should_fail_when_filtered_collection_is_empty_and_minCount_is_required()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<LogsConfig>(services, "Logs");

        builder.ValidateEach(c => c.Logs.Where(l => l.Enabled),
            itemBuilder => itemBuilder.NotEmpty(l => l.Name, "Log name is required"),
            minCount: 1,
            emptyMessage: "At least one enabled log is required");

        var config = new LogsConfig
        {
            Logs =
            [
                new LogConfig { Name = "Log1", TableName = "Table1", BatchSize = 100, Enabled = false },
                new LogConfig { Name = "Log2", TableName = "Table2", BatchSize = 200, Enabled = false }
            ]
        };

        var errors = builder.Validate(config).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("At least one enabled log is required");
    }

    //==============================================================================================
    /// <summary>
    /// ValidateEach should collect multiple validation errors from multiple items.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateEach_should_collect_multiple_validation_errors_from_multiple_items()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ServerConfig>(services, "Server");

        builder.ValidateEach(c => c.Endpoints,
            itemBuilder => itemBuilder
                .NotEmpty(e => e.Url, "URL is required")
                .InRange(e => e.Port, 1, 65535, "Port must be valid"));

        var config = new ServerConfig
        {
            Endpoints =
            [
                new EndpointConfig { Url = "", Port = 0 },
                new EndpointConfig { Url = "", Port = 99999 }
            ]
        };

        var errors = builder.Validate(config).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("Endpoints contains invalid items");
        errors[0].Message.Should().Contain("URL is required");
        errors[0].Message.Should().Contain("Port must be valid");
    }

    //==============================================================================================
    /// <summary>
    /// ValidateEach should work with null collection (treated as empty).
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateEach_should_work_with_null_collection()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ServerConfig>(services, "Server");

        builder.ValidateEach(c => c.Endpoints,
            itemBuilder => itemBuilder.NotEmpty(e => e.Url, "URL is required"),
            minCount: 0);

        var config = new ServerConfig { Endpoints = null! };

        var errors = builder.Validate(config).ToList();

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// ValidateEach should fail with null collection when minCount is greater than 0.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateEach_should_fail_with_null_collection_when_minCount_required()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ServerConfig>(services, "Server");

        builder.ValidateEach(c => c.Endpoints,
            itemBuilder => itemBuilder.NotEmpty(e => e.Url, "URL is required"),
            minCount: 1,
            emptyMessage: "At least one endpoint is required");

        var config = new ServerConfig { Endpoints = null! };

        var errors = builder.Validate(config).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("At least one endpoint is required");
    }

    #endregion
}
