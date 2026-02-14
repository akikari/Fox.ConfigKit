//==================================================================================================
// Unit tests for file system validation rules.
// Tests for DirectoryExistsRule and FileExistsRule using temporary file system resources.
//==================================================================================================
using FluentAssertions;
using Fox.ConfigKit.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ConfigKit.Tests;

//==============================================================================================
/// <summary>
/// Tests for file system validation rules.
/// </summary>
//==============================================================================================
public sealed class FileSystemValidationRulesTests : IDisposable
{
    #region Fields

    private readonly string tempDirectory;
    private readonly string tempFilePath;

    #endregion

    #region Constructor

    public FileSystemValidationRulesTests()
    {
        tempDirectory = Path.Combine(Path.GetTempPath(), $"ConfigKitTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDirectory);

        tempFilePath = Path.Combine(tempDirectory, "test.txt");
        File.WriteAllText(tempFilePath, "test content");
    }

    #endregion

    #region DirectoryExists Tests

    //==========================================================================================
    /// <summary>
    /// Test options class for directory validation scenarios.
    /// </summary>
    //==========================================================================================
    private sealed class DirectoryTestOptions
    {
        public string? DirectoryPath { get; set; }
    }

    [Fact]
    public void DirectoryExists_should_pass_when_directory_exists()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DirectoryTestOptions>(services, "Test");
        builder.DirectoryExists(o => o.DirectoryPath);

        var options = new DirectoryTestOptions { DirectoryPath = tempDirectory };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void DirectoryExists_should_fail_when_directory_does_not_exist()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DirectoryTestOptions>(services, "Test");
        builder.DirectoryExists(o => o.DirectoryPath);

        var nonExistentPath = Path.Combine(tempDirectory, "NonExistent");
        var options = new DirectoryTestOptions { DirectoryPath = nonExistentPath };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("Directory does not exist");
        errors[0].Message.Should().Contain(nonExistentPath);
    }

    [Fact]
    public void DirectoryExists_should_fail_when_path_is_null()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DirectoryTestOptions>(services, "Test");
        builder.DirectoryExists(o => o.DirectoryPath);

        var options = new DirectoryTestOptions { DirectoryPath = null };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("directory path is not specified");
    }

    [Fact]
    public void DirectoryExists_should_fail_when_path_is_empty()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DirectoryTestOptions>(services, "Test");
        builder.DirectoryExists(o => o.DirectoryPath);

        var options = new DirectoryTestOptions { DirectoryPath = string.Empty };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("directory path is not specified");
    }

    [Fact]
    public void DirectoryExists_should_fail_when_path_is_whitespace()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DirectoryTestOptions>(services, "Test");
        builder.DirectoryExists(o => o.DirectoryPath);

        var options = new DirectoryTestOptions { DirectoryPath = "   " };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("directory path is not specified");
    }

    [Fact]
    public void DirectoryExists_should_use_custom_message_when_provided()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DirectoryTestOptions>(services, "Test");
        builder.DirectoryExists(o => o.DirectoryPath, message: "Custom directory error message");

        var nonExistentPath = Path.Combine(tempDirectory, "NonExistent");
        var options = new DirectoryTestOptions { DirectoryPath = nonExistentPath };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Be("Custom directory error message");
    }

    [Fact]
    public void DirectoryExists_should_provide_suggestion_to_create_directory()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<DirectoryTestOptions>(services, "Test");
        builder.DirectoryExists(o => o.DirectoryPath);

        var nonExistentPath = Path.Combine(tempDirectory, "NonExistent");
        var options = new DirectoryTestOptions { DirectoryPath = nonExistentPath };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Suggestions.Should().Contain(s => s.Contains("Create directory at"));
    }

    #endregion

    #region FileExists Tests

    //==========================================================================================
    /// <summary>
    /// Test options class for file validation scenarios.
    /// </summary>
    //==========================================================================================
    private sealed class FileTestOptions
    {
        public string? FilePath { get; set; }
    }

    [Fact]
    public void FileExists_should_pass_when_file_exists()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<FileTestOptions>(services, "Test");
        builder.FileExists(o => o.FilePath);

        var options = new FileTestOptions { FilePath = tempFilePath };
        var errors = builder.Validate(options).ToList();

        errors.Should().BeEmpty();
    }

    [Fact]
    public void FileExists_should_fail_when_file_does_not_exist()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<FileTestOptions>(services, "Test");
        builder.FileExists(o => o.FilePath);

        var nonExistentPath = Path.Combine(tempDirectory, "NonExistent.txt");
        var options = new FileTestOptions { FilePath = nonExistentPath };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("File does not exist");
        errors[0].Message.Should().Contain(nonExistentPath);
    }

    [Fact]
    public void FileExists_should_fail_when_path_is_null()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<FileTestOptions>(services, "Test");
        builder.FileExists(o => o.FilePath);

        var options = new FileTestOptions { FilePath = null };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("file path is not specified");
    }

    [Fact]
    public void FileExists_should_fail_when_path_is_empty()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<FileTestOptions>(services, "Test");
        builder.FileExists(o => o.FilePath);

        var options = new FileTestOptions { FilePath = string.Empty };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("file path is not specified");
    }

    [Fact]
    public void FileExists_should_fail_when_path_is_whitespace()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<FileTestOptions>(services, "Test");
        builder.FileExists(o => o.FilePath);

        var options = new FileTestOptions { FilePath = "   " };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Contain("file path is not specified");
    }

    [Fact]
    public void FileExists_should_use_custom_message_when_provided()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<FileTestOptions>(services, "Test");
        builder.FileExists(o => o.FilePath, message: "Custom file error message");

        var nonExistentPath = Path.Combine(tempDirectory, "NonExistent.txt");
        var options = new FileTestOptions { FilePath = nonExistentPath };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Message.Should().Be("Custom file error message");
    }

    [Fact]
    public void FileExists_should_provide_suggestion_to_create_file()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<FileTestOptions>(services, "Test");
        builder.FileExists(o => o.FilePath);

        var nonExistentPath = Path.Combine(tempDirectory, "NonExistent.txt");
        var options = new FileTestOptions { FilePath = nonExistentPath };
        var errors = builder.Validate(options).ToList();

        errors.Should().HaveCount(1);
        errors[0].Suggestions.Should().Contain(s => s.Contains("Create file at"));
    }

    #endregion

    #region Cleanup

    public void Dispose()
    {
        if (Directory.Exists(tempDirectory))
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    #endregion
}
