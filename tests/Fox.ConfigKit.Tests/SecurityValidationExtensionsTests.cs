//==================================================================================================
// Tests for security validation extension methods.
// Verifies secret detection, format validation, and default value warnings.
//==================================================================================================
using FluentAssertions;
using Fox.ConfigKit.Security;
using Fox.ConfigKit.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ConfigKit.Tests;

//==================================================================================================
/// <summary>
/// Tests for <see cref="SecurityValidationExtensions"/>.
/// </summary>
//==================================================================================================
public sealed class SecurityValidationExtensionsTests
{
    #region Test Classes

    private sealed class ApiConfig
    {
        public string? ApiKey { get; set; }
        public string? ConnectionString { get; set; }
        public string? Password { get; set; }
    }

    #endregion

    #region NoPlainTextSecrets Tests

    //==============================================================================================
    /// <summary>
    /// NoPlainTextSecrets() should detect likely plain-text API key.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void NoPlainTextSecrets_should_detect_plain_text_api_key()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.NoPlainTextSecrets(o => o.ApiKey, "Plain-text API key detected");

        var errors = builder.Validate(new ApiConfig { ApiKey = "AIzaSyD4vZ8rK3lN2pQ7mX9oY1tW6fE5hU4jI8k" });

        errors.Should().ContainSingle()
            .Which.Message.Should().Contain("Plain-text API key detected");
    }

    //==============================================================================================
    /// <summary>
    /// NoPlainTextSecrets() should pass when value is Azure Key Vault reference.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void NoPlainTextSecrets_should_pass_for_azure_keyvault_reference()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.NoPlainTextSecrets(o => o.ApiKey);

        var errors = builder.Validate(new ApiConfig { ApiKey = "@Microsoft.KeyVault(SecretUri=https://...)" });

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// NoPlainTextSecrets() should pass when value is null or whitespace.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void NoPlainTextSecrets_should_pass_for_null_or_whitespace()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.NoPlainTextSecrets(o => o.ApiKey);

        var errorsNull = builder.Validate(new ApiConfig { ApiKey = null });
        var errorsEmpty = builder.Validate(new ApiConfig { ApiKey = "" });
        var errorsWhitespace = builder.Validate(new ApiConfig { ApiKey = "   " });

        errorsNull.Should().BeEmpty();
        errorsEmpty.Should().BeEmpty();
        errorsWhitespace.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// NoPlainTextSecrets() should throw ArgumentNullException when builder is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void NoPlainTextSecrets_should_throw_when_builder_is_null()
    {
        ConfigValidationBuilder<ApiConfig> builder = null!;

        var act = () => builder.NoPlainTextSecrets(o => o.ApiKey);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("builder");
    }

    #endregion

    #region ValidateSecretFormat Tests

    //==============================================================================================
    /// <summary>
    /// ValidateSecretFormat() should pass when Azure Key Vault format is correct.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateSecretFormat_should_pass_for_azure_keyvault_format()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.ValidateSecretFormat(o => o.ApiKey, SecretFormat.AzureKeyVault);

        var errors = builder.Validate(new ApiConfig { ApiKey = "@Microsoft.KeyVault(SecretUri=https://myvault.vault.azure.net/secrets/mykey)" });

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// ValidateSecretFormat() should fail when Azure Key Vault format is incorrect.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateSecretFormat_should_fail_for_incorrect_azure_keyvault_format()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.ValidateSecretFormat(o => o.ApiKey, SecretFormat.AzureKeyVault);

        var errors = builder.Validate(new ApiConfig { ApiKey = "plaintext_key" });

        errors.Should().ContainSingle()
            .Which.Message.Should().Contain("does not follow AzureKeyVault format");
    }

    //==============================================================================================
    /// <summary>
    /// ValidateSecretFormat() should pass for AWS Secrets Manager format.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateSecretFormat_should_pass_for_aws_secrets_manager_format()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.ValidateSecretFormat(o => o.ApiKey, SecretFormat.AwsSecretsManager);

        var errors = builder.Validate(new ApiConfig { ApiKey = "arn:aws:secretsmanager:us-west-2:123456789012:secret:my-secret" });

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// ValidateSecretFormat() should pass for environment variable format.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateSecretFormat_should_pass_for_environment_variable_format()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.ValidateSecretFormat(o => o.ApiKey, SecretFormat.EnvironmentVariable);

        var errors = builder.Validate(new ApiConfig { ApiKey = "${MY_API_KEY}" });

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// ValidateSecretFormat() should pass for null or whitespace values.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateSecretFormat_should_pass_for_null_or_whitespace()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.ValidateSecretFormat(o => o.ApiKey, SecretFormat.AzureKeyVault);

        var errorsNull = builder.Validate(new ApiConfig { ApiKey = null });
        var errorsEmpty = builder.Validate(new ApiConfig { ApiKey = "" });

        errorsNull.Should().BeEmpty();
        errorsEmpty.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// ValidateSecretFormat() should throw ArgumentNullException when builder is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateSecretFormat_should_throw_when_builder_is_null()
    {
        ConfigValidationBuilder<ApiConfig> builder = null!;

        var act = () => builder.ValidateSecretFormat(o => o.ApiKey, SecretFormat.AzureKeyVault);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("builder");
    }

    //==============================================================================================
    /// <summary>
    /// ValidateSecretFormat() should pass for UserSecrets format with non-secret value.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateSecretFormat_should_pass_for_user_secrets_format()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.ValidateSecretFormat(o => o.ApiKey, SecretFormat.UserSecrets);

        var errors = builder.Validate(new ApiConfig { ApiKey = "configured-via-user-secrets" });

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// ValidateSecretFormat() should fail for UserSecrets format with likely secret.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateSecretFormat_should_fail_for_user_secrets_with_likely_secret()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.ValidateSecretFormat(o => o.ApiKey, SecretFormat.UserSecrets);

        var errors = builder.Validate(new ApiConfig { ApiKey = "AIzaSyD4vZ8rK3lN2pQ7mX9oY1tW6fE5hU4jI8k" });

        errors.Should().ContainSingle();
    }

    //==============================================================================================
    /// <summary>
    /// ValidateSecretFormat() should fail when environment variable format is incorrect.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateSecretFormat_should_fail_for_incorrect_environment_variable_format()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.ValidateSecretFormat(o => o.ApiKey, SecretFormat.EnvironmentVariable);

        var errors = builder.Validate(new ApiConfig { ApiKey = "MY_VAR" });

        errors.Should().ContainSingle()
            .Which.Message.Should().Contain("does not follow EnvironmentVariable format");
    }

    //==============================================================================================
    /// <summary>
    /// ValidateSecretFormat() should fail when AWS format is incorrect.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ValidateSecretFormat_should_fail_for_incorrect_aws_format()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.ValidateSecretFormat(o => o.ApiKey, SecretFormat.AwsSecretsManager);

        var errors = builder.Validate(new ApiConfig { ApiKey = "not-an-arn" });

        errors.Should().ContainSingle()
            .Which.Message.Should().Contain("does not follow AwsSecretsManager format");
    }

    #endregion

    #region WarnIfDefaultValue Tests

    //==============================================================================================
    /// <summary>
    /// WarnIfDefaultValue() should detect default password value with Critical level.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WarnIfDefaultValue_should_detect_default_password_with_critical_level()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.WarnIfDefaultValue(o => o.Password, "password", SecurityLevel.Critical);

        var errors = builder.Validate(new ApiConfig { Password = "password" });

        errors.Should().ContainSingle()
            .Which.Message.Should().Contain("CRITICAL");
    }

    //==============================================================================================
    /// <summary>
    /// WarnIfDefaultValue() should detect default value with Warning level.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WarnIfDefaultValue_should_detect_default_with_warning_level()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.WarnIfDefaultValue(o => o.Password, "default", SecurityLevel.Warning);

        var errors = builder.Validate(new ApiConfig { Password = "default" });

        errors.Should().ContainSingle()
            .Which.Message.Should().Contain("WARNING");
    }

    //==============================================================================================
    /// <summary>
    /// WarnIfDefaultValue() should detect default value with Info level.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WarnIfDefaultValue_should_detect_default_with_info_level()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.WarnIfDefaultValue(o => o.Password, "changeme", SecurityLevel.Info);

        var errors = builder.Validate(new ApiConfig { Password = "changeme" });

        errors.Should().ContainSingle()
            .Which.Message.Should().Contain("INFO");
    }

    //==============================================================================================
    /// <summary>
    /// WarnIfDefaultValue() should be case-insensitive.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WarnIfDefaultValue_should_be_case_insensitive()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.WarnIfDefaultValue(o => o.Password, "password");

        var errorsLower = builder.Validate(new ApiConfig { Password = "password" });
        var errorsUpper = builder.Validate(new ApiConfig { Password = "PASSWORD" });
        var errorsMixed = builder.Validate(new ApiConfig { Password = "PaSsWoRd" });

        errorsLower.Should().ContainSingle();
        errorsUpper.Should().ContainSingle();
        errorsMixed.Should().ContainSingle();
    }

    //==============================================================================================
    /// <summary>
    /// WarnIfDefaultValue() should pass when value is not default.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WarnIfDefaultValue_should_pass_when_value_is_not_default()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.WarnIfDefaultValue(o => o.Password, "password", SecurityLevel.Warning);

        var errors = builder.Validate(new ApiConfig { Password = "SecureP@ssw0rd!" });

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// WarnIfDefaultValue() should pass when value is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WarnIfDefaultValue_should_pass_when_value_is_null()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.WarnIfDefaultValue(o => o.Password, "password");

        var errors = builder.Validate(new ApiConfig { Password = null });

        errors.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// WarnIfDefaultValue() should throw ArgumentNullException when builder is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WarnIfDefaultValue_should_throw_when_builder_is_null()
    {
        ConfigValidationBuilder<ApiConfig> builder = null!;

        var act = () => builder.WarnIfDefaultValue(o => o.Password, "default");

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("builder");
    }

    //==============================================================================================
    /// <summary>
    /// WarnIfDefaultValue() should use custom message when provided.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void WarnIfDefaultValue_should_use_custom_message()
    {
        var services = new ServiceCollection();
        var builder = new ConfigValidationBuilder<ApiConfig>(services, "Test");

        builder.WarnIfDefaultValue(o => o.Password, "admin", SecurityLevel.Critical, "Never use admin password");

        var errors = builder.Validate(new ApiConfig { Password = "admin" });

        errors.Should().ContainSingle()
            .Which.Message.Should().Be("Never use admin password");
    }

    #endregion
}
