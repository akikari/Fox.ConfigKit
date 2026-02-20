# Contributing to Fox.ConfigKit

Thank you for your interest in contributing to Fox.ConfigKit! This document provides guidelines and instructions for contributing to the project.

## Code of Conduct

By participating in this project, you agree to maintain a respectful and inclusive environment for all contributors.

## How to Contribute

### Reporting Issues

If you find a bug or have a feature request:

1. Check if the issue already exists in the [GitHub Issues](https://github.com/akikari/Fox.ConfigKit/issues)
2. If not, create a new issue with:
   - Clear, descriptive title
   - Detailed description of the problem or feature
   - Steps to reproduce (for bugs)
   - Expected vs actual behavior
   - Code samples if applicable
   - Environment details (.NET version, OS, etc.)

### Submitting Changes

1. **Fork the repository** and create a new branch from `main`
2. **Make your changes** following the coding guidelines below
3. **Write or update tests** for your changes
4. **Update documentation** if needed (README, XML comments)
5. **Ensure all tests pass** (`dotnet test`)
6. **Ensure build succeeds** (`dotnet build`)
7. **Submit a pull request** with:
   - Clear description of changes
   - Reference to related issues
   - Summary of testing performed

## Coding Guidelines

Fox.ConfigKit follows strict coding standards. Please review the [Copilot Instructions](.github/copilot-instructions.md) for detailed guidelines.

### Key Standards

#### General
- **Language**: All code, comments, and documentation must be in English
- **Line Endings**: CRLF
- **Indentation**: 4 spaces (no tabs)
- **Namespaces**: File-scoped (`namespace MyNamespace;`)
- **Nullable**: Enabled
- **Language Version**: latest

#### Naming Conventions
- **Private Fields**: camelCase without underscore prefix (e.g., `value`, not `_value`)
- **Public Members**: PascalCase
- **Local Variables**: camelCase

#### Code Style
- Use expression-bodied members for simple properties and methods
- Use auto-properties where possible
- Prefer `var` only when type is obvious
- Maximum line length: 100 characters
- Add blank line after closing brace UNLESS next line is also `}`

#### Documentation
- **XML Comments**: Required for all public APIs
- **Language**: English
- **Decorators**: 98 characters width using `//======` (no space after prefix)
- **File Headers**: 3-line header (purpose + technical description + decorators)

Example:
```csharp
//==================================================================================================
// Validates that a string property is not null or empty.
// Provides fail-fast configuration validation at application startup.
//==================================================================================================

namespace Fox.ConfigKit.Validation;

//==================================================================================================
/// <summary>
/// Validates that a string property is not null or empty.
/// </summary>
/// <typeparam name="T">The configuration type to validate.</typeparam>
//==================================================================================================
internal sealed class NotEmptyRule<T> : ValidationRuleBase, IValidationRule<T> where T : class
{
    private readonly Func<T, string?> getValue;
    private readonly string propertyName;
    private readonly string? message;

    //==============================================================================================
    /// <summary>
    /// Initializes a new instance of the <see cref="NotEmptyRule{T}"/> class.
    /// </summary>
    /// <param name="selector">Expression to select the property to validate.</param>
    /// <param name="message">Optional custom error message.</param>
    //==============================================================================================
    public NotEmptyRule(Expression<Func<T, string?>> selector, string? message = null)
    {
        this.getValue = selector.Compile();
        this.propertyName = GetPropertyName(selector);
        this.message = message;
    }

    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public ConfigValidationError? Validate(T options, string sectionName)
    {
        var value = getValue(options);

        if (string.IsNullOrEmpty(value))
        {
            var key = $"{sectionName}:{propertyName}";
            var errorMessage = message ?? $"{propertyName} must not be empty";
            return new ConfigValidationError(key, errorMessage, value, ["Provide a non-empty value"]);
        }

        return null;
    }
}
```

## Testing Requirements

- **Framework**: xUnit
- **Assertions**: FluentAssertions
- **Test Naming**: `MethodName_should_expected_behavior`
- **Coverage**: Aim for 100% coverage of new code
- **Test Structure**:
  - Arrange: Setup test data
  - Act: Execute the method under test
  - Assert: Verify expected behavior

Example:
```csharp
[Fact]
public void NotEmpty_should_fail_when_value_is_empty()
{
    // Arrange
    var config = new TestConfig { Name = string.Empty };
    var rule = new NotEmptyRule<TestConfig>(c => c.Name, "Name is required");

    // Act
    var error = rule.Validate(config, "Test");

    // Assert
    error.Should().NotBeNull();
    error!.Key.Should().Be("Test:Name");
    error.Message.Should().Be("Name is required");
}

[Fact]
public void AddConfigKit_should_validate_on_startup()
{
    // Arrange
    var services = new ServiceCollection();
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Test:Name"] = "ValidName",
            ["Test:Port"] = "8080"
        })
        .Build();

    services.AddSingleton<IConfiguration>(configuration);
    services.Configure<TestConfig>(configuration.GetSection("Test"));

    services.AddConfigKit<TestConfig>("Test")
        .NotEmpty(c => c.Name, "Name is required")
        .InRange(c => c.Port, 1, 65535, "Port must be between 1 and 65535")
        .ValidateOnStartup();

    // Act
    var serviceProvider = services.BuildServiceProvider();
    var config = serviceProvider.GetRequiredService<IOptions<TestConfig>>();

    // Assert
    config.Value.Name.Should().Be("ValidName");
    config.Value.Port.Should().Be(8080);
}
```

## Architecture Principles

Fox.ConfigKit follows Clean Architecture and SOLID principles:

- **Single Responsibility**: Each validation rule has one clear purpose
- **Open/Closed**: Open for extension (via custom rules), closed for modification
- **Liskov Substitution**: All validation rule implementations are substitutable
- **Interface Segregation**: Small, focused interfaces (IValidationRule<T>)
- **Dependency Inversion**: Depend on abstractions, not concretions

### Design Guidelines

- **Fail-Fast**: Catch configuration errors at startup, not at runtime
- **Explicit Validation**: Make validation rules clear and discoverable
- **Type-Safe**: Leverage C# type system for compile-time safety
- **Zero Dependencies**: Only core Microsoft.Extensions packages
- **Fluent API**: Intuitive, chainable validation rules
- **Developer-Friendly**: Clear error messages, excellent IntelliSense

## Project Structure

```
Fox.ConfigKit/
├── src/
│   ├── Fox.ConfigKit/                    # Core package
│   │   ├── ConfigValidationBuilder.cs    # Fluent builder API
│   │   ├── Validation/                   # Validation rules
│   │   │   ├── *ValidationExtensions.cs  # Extension methods for rules
│   │   │   └── Rules/                    # Rule implementations
│   │   └── Internal/                     # Internal utilities (SecretDetector, etc.)
│   └── Fox.ConfigKit.ResultKit/          # ResultKit integration
│       ├── ConfigValidator.cs            # Standalone validation
│       ├── Extensions/                   # ToResult/ToErrorsResult extensions
│       └── Internal/                     # Internal adapters
├── tests/
│   ├── Fox.ConfigKit.Tests/              # Core tests (459 tests)
│   └── Fox.ConfigKit.ResultKit.Tests/    # ResultKit integration tests
└── samples/
    └── Fox.ConfigKit.Samples.WebApi/     # ASP.NET Core sample application
```

## Pull Request Process

1. **Update tests**: Ensure your changes are covered by tests
2. **Update documentation**: Keep README and XML comments up to date
3. **Follow coding standards**: Use provided `.editorconfig` and copilot instructions
4. **Keep commits clean**: 
   - Use clear, descriptive commit messages
   - Squash commits if needed before merging
5. **Update CHANGELOG.md**: Add entry under `[Unreleased]` section
6. **Ensure CI passes**: All tests must pass and build must succeed

### Commit Message Format

Use clear, imperative commit messages in English:

```
Add InRange validation rule for TimeSpan values

- Implement TimeSpan-specific InRange validation
- Add unit tests for TimeSpan boundary conditions
- Update documentation with TimeSpan examples
```

## Feature Requests

When proposing new features, please consider:

1. **Scope**: Does this fit the focused nature of Fox.ConfigKit?
2. **Complexity**: Does this add unnecessary complexity?
3. **Dependencies**: Does this require new external dependencies?
4. **Breaking Changes**: Will this break existing code?
5. **Use Cases**: What real-world scenarios does this address?

Fox.ConfigKit aims to be lightweight and focused. Features should align with configuration validation principles and IOptions pattern.

## Development Setup

### Prerequisites
- .NET 8 SDK or later
- Visual Studio 2022+ or Rider (recommended)
- Git

### Getting Started

1. Clone the repository:
```bash
git clone https://github.com/akikari/Fox.ConfigKit.git
cd Fox.ConfigKit
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the solution:
```bash
dotnet build
```

4. Run tests:
```bash
dotnet test
```

5. Run the sample application:
```bash
dotnet run --project samples/Fox.ConfigKit.Samples.WebApi/Fox.ConfigKit.Samples.WebApi.csproj
```

## Questions?

If you have questions about contributing, feel free to:
- Open a [GitHub Discussion](https://github.com/akikari/Fox.ConfigKit/discussions)
- Create an issue labeled `question`
- Reach out to the maintainers

## License

By contributing to Fox.ConfigKit, you agree that your contributions will be licensed under the MIT License.

Thank you for contributing to Fox.ConfigKit! 🎉
