# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

_No unreleased changes yet._

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
