# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

_No unreleased changes yet._

## [1.2.1] - 2026-04-03

### Fixed

#### Fox.ConfigKit
- **Package Versioning**: Aligned version to 1.2.1 to match ResultKit

#### Fox.ConfigKit.ResultKit
- **NuGet Dependency**: Fixed missing `Fox.ConfigKit` package dependency in NuGet package
  - Use `ProjectReference` with `PrivateAssets="all"` for builds (both Debug and Release)
  - Add explicit `PackageReference` with `PrivateAssets="none"` for NuGet package metadata
  - Ensures `Fox.ConfigKit 1.2.1` is automatically installed when using `Fox.ConfigKit.ResultKit 1.2.1`

## [1.2.0] - 2026-04-03

### Added

#### Fox.ConfigKit
- **Collection Validation**: New `ValidateEach<T, TItem>()` extension method for validating each item in a collection
  - Supports simple property access: `c => c.Endpoints`
  - Supports LINQ filtering: `c => c.Endpoints.Where(e => e.Enabled)`
  - Optional `minCount` parameter for ensuring minimum collection size
  - Optional `emptyMessage` for custom empty collection error messages
  - Nested validation rules: Apply full validation builder to each collection item
- New `CollectionValidationRule<T, TItem>` internal rule with expression tree parsing
  - Handles `MemberExpression` for simple property access
  - Handles `MethodCallExpression` for LINQ methods (Where, Select, etc.)
  - Custom `ExtractCollectionName()` method for proper error messages
- 30 comprehensive unit tests covering all collection validation scenarios

#### Fox.ConfigKit.ResultKit
- Collection validation now available through `ConfigValidator.Validate<T>()` API
- Full integration with `ToValidationResult()` for functional error handling

#### Documentation
- Updated README.md with collection validation examples and LINQ filtering
- Added comprehensive `ServerConfig` example in sample project with endpoint validation
- New API endpoint: `GET /api/configuration/servers`
- Updated samples README with Example 8: Collection Validation
- Added collection validation to Key Validation Features table

### Changed
- Version bumped from 1.1.0 to 1.2.0 for both Fox.ConfigKit and Fox.ConfigKit.ResultKit packages

## [1.1.0] - 2026-03-28

### Added

#### Fox.ConfigKit
- **Property-to-Property Comparison Validation**: New validation methods for comparing two properties within the same configuration object
  - `GreaterThanProperty<T, TValue>(selector, compareToSelector, message)` - Validates that one property is greater than another (exclusive)
  - `LessThanProperty<T, TValue>(selector, compareToSelector, message)` - Validates that one property is less than another (exclusive)
  - `MinimumProperty<T, TValue>(selector, compareToSelector, message)` - Validates that one property is at least another (inclusive)
  - `MaximumProperty<T, TValue>(selector, compareToSelector, message)` - Validates that one property is at most another (inclusive)
- New validation rules: `GreaterThanPropertyRule`, `LessThanPropertyRule`, `MinimumPropertyRule`, `MaximumPropertyRule`
- Support for all `IComparable<T>` types including `int`, `decimal`, `DateTime`, `TimeSpan`, etc.
- 14 new unit tests covering all property comparison scenarios (42 tests total for property comparison)

#### Fox.ConfigKit.ResultKit
- Property-to-property comparison validation now available through `ConfigValidator.Validate<T>()` API
- Full integration with `ToValidationResult()` for functional error handling

#### Documentation
- Updated README.md with property-to-property comparison examples
- Updated package README files (src/Fox.ConfigKit/README.md and src/Fox.ConfigKit.ResultKit/README.md)
- Added comprehensive examples in sample project (Fox.ConfigKit.Samples.WebApi)
- New `MigrationConfig` sample demonstrating batch processing validation (RecordsPerRun >= BatchSize)
- Updated `CampaignConfig` sample with date range validation (EndDate > StartDate)
- New API endpoint: `GET /api/configuration/migration`
- Added Scenario 6 to testing scenarios (Property Comparison Violation)

### Changed
- Version bumped from 1.0.4/1.0.5 to 1.1.0 for both Fox.ConfigKit and Fox.ConfigKit.ResultKit packages
- Updated Key Validation Features documentation table to include property comparison methods

## [1.0.5] - 2026-03-25

### Changed

#### Fox.ConfigKit.ResultKit
- Updated Fox.ResultKit dependency from 1.2.0 to 1.3.0
- Now supports `Result.Match()` method for functional error handling
- Improved Result pattern integration with latest Fox.ResultKit features

## [1.0.4] - 2026-02-10

### Fixed

#### Documentation
- Fixed Author name in package READMEs to include proper accented characters: **Károly Akácz**
- Project-specific READMEs (src/Fox.ConfigKit/README.md and src/Fox.ConfigKit.ResultKit/README.md) now display Author name correctly

### Changed
- Version bumped to 1.0.4 for both Fox.ConfigKit and Fox.ConfigKit.ResultKit packages
- PackageReleaseNotes updated to describe Author name fix

## [1.0.3] - 2026-02-10

### Fixed

#### Documentation
- Fixed NuGet package README rendering issues on NuGet.org
- Implemented dual README strategy:
  - Root README.md for GitHub display (with emoji + Author section)
  - Project-specific READMEs for NuGet packages (ASCII-only for compatibility)
- Removed emoji from H1 headers in package READMEs to fix UTF-8 encoding issues on NuGet.org
- Fixed XML syntax error in Fox.ConfigKit.ResultKit.csproj (duplicate quotation mark)
- Updated version number in README.md from 1.0.0 to 1.0.3

### Changed
- PackageReleaseNotes updated to describe dual README fix

## [1.0.2] - 2026-02-10

### Fixed
- Attempted fix for NuGet package README (unlisted - wrong file modified)

## [1.0.1] - 2026-02-09

### Fixed
- Attempted fix for NuGet package README (unlisted - wrong file modified)

## [1.0.0] - 2026-02-07

### Added

#### Fox.ConfigKit (Core Library)
- `IOptions<T>` validation with fluent API
- Fail-fast startup validation with `.ValidateOnStartup()`
- Comprehensive validation rules:
  - **Basic**: `NotNull`, `NotEmpty`, `NotNullOrWhiteSpace`
  - **Comparison**: `GreaterThan`, `LessThan`, `Minimum`, `Maximum`, `InRange`
  - **Collection**: `NotNullOrEmpty`, `MinimumCount`, `MaximumCount`
  - **String**: `MinLength`, `MaxLength`, `Matches` (regex)
  - **DateTime/TimeSpan**: `After`, `Before`, `InRange`
  - **Custom**: `Must` (custom predicates), `CustomRule`
  - **File System**: `DirectoryExists`, `FileExists`
  - **Network**: `UrlReachable`, `PortAvailable`
- Secret detection in validation messages to prevent logging sensitive data
- Regex-based validation for URLs, emails, IP addresses, phone numbers
- DI integration with `IServiceCollection` extensions
- XML documentation for all public APIs

#### Fox.ConfigKit.ResultKit (Integration Package)
- ResultKit integration for Railway Oriented Programming
- `ValidateAsResult()` extension method returns `Result<T>` instead of throwing exceptions
- Seamless integration between Fox.ConfigKit and Fox.ResultKit
- Functional error handling for configuration validation

#### Documentation
- Comprehensive README.md with examples and usage patterns
- Design principles documentation
- Contributing guidelines
- MIT License

#### Samples
- Sample WebAPI application demonstrating configuration validation
- Examples of all validation rule types
- Secret detection examples
- ResultKit integration examples

#### Tests
- 459 comprehensive unit tests (100% passing)
- Mock-based testing for external dependencies
- XML exception documentation tests
- Coverage for all validation rules including:
  - SecretDetector tests (39 test cases)
  - FileSystem validation tests (14 tests with temp resources)
  - Network validation tests (15 tests with real network operations)
  - Generic validation tests (45 tests for int, decimal, DateTime, TimeSpan)
  - Core validation logic tests (13 tests)
  - ResultKit integration tests

#### Build & CI/CD
- Multi-targeting: .NET 8.0, .NET 9.0, .NET 10.0
- GitHub Actions workflows:
  - `build-and-test.yml` - Build and test validation
  - `publish-nuget.yml` - Automated NuGet publishing on version tags
- NuGet package metadata configuration
- Package icon and license integration

### Initial Release
- Production-ready code quality (9.5/10 assessment)
- All nullable reference types enabled
- Follows Microsoft coding conventions
- Comprehensive XML documentation
- Full test coverage
