//==================================================================================================
// Unit tests for plain-text secret detection.
// Comprehensive tests for IsLikelySecret and IsSecureReference methods.
//==================================================================================================
using FluentAssertions;
using Fox.ConfigKit.Security;

namespace Fox.ConfigKit.Tests;

//==============================================================================================
/// <summary>
/// Tests for secret detection in configuration values.
/// </summary>
//==============================================================================================
public sealed class SecretDetectorTests
{
    #region IsLikelySecret Tests

    [Fact]
    public void IsLikelySecret_should_return_false_when_value_is_null()
    {
        var result = SecretDetector.IsLikelySecret(null, "ApiKey");

        result.Should().BeFalse();
    }

    [Fact]
    public void IsLikelySecret_should_return_false_when_value_is_empty()
    {
        var result = SecretDetector.IsLikelySecret(string.Empty, "ApiKey");

        result.Should().BeFalse();
    }

    [Fact]
    public void IsLikelySecret_should_return_false_when_value_is_whitespace()
    {
        var result = SecretDetector.IsLikelySecret("   ", "ApiKey");

        result.Should().BeFalse();
    }

    [Fact]
    public void IsLikelySecret_should_throw_when_propertyName_is_null()
    {
        var act = () => SecretDetector.IsLikelySecret("value", null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsLikelySecret_should_detect_openai_api_key()
    {
        var result = SecretDetector.IsLikelySecret("sk-abcdefghijklmnopqrstuvwxyz1234567890", "ApiKey");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsLikelySecret_should_detect_google_api_key()
    {
        var result = SecretDetector.IsLikelySecret("AIzaSyABC123XYZ789abcdefghijklmnopqrs", "ApiKey");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsLikelySecret_should_detect_aws_access_key()
    {
        var result = SecretDetector.IsLikelySecret("AKIAIOSFODNN7EXAMPLE", "ApiKey");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsLikelySecret_should_detect_bearer_token()
    {
        var result = SecretDetector.IsLikelySecret("Bearer abc123.def456.ghi789", "Token");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsLikelySecret_should_detect_bearer_token_case_insensitive()
    {
        var result = SecretDetector.IsLikelySecret("bearer abc123.def456.ghi789", "token");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsLikelySecret_should_detect_generic_base64_secret()
    {
        var result = SecretDetector.IsLikelySecret("abcdefghijklmnopqrstuvwxyz123456", "Secret");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsLikelySecret_should_detect_hex64_secret()
    {
        var result = SecretDetector.IsLikelySecret("a1b2c3d4e5f67890a1b2c3d4e5f67890a1b2c3d4e5f67890a1b2c3d4e5f67890", "PrivateKey");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsLikelySecret_should_return_false_for_non_secret_property()
    {
        var result = SecretDetector.IsLikelySecret("sk-abcdefghijklmnopqrstuvwxyz1234567890", "Name");

        result.Should().BeFalse();
    }

    [Fact]
    public void IsLikelySecret_should_return_false_for_secure_reference_azure_keyvault()
    {
        var result = SecretDetector.IsLikelySecret("@Microsoft.KeyVault(SecretUri=https://myvault.vault.azure.net/secrets/mysecret)", "Password");

        result.Should().BeFalse();
    }

    [Fact]
    public void IsLikelySecret_should_return_false_for_secure_reference_aws_secretsmanager()
    {
        var result = SecretDetector.IsLikelySecret("arn:aws:secretsmanager:us-east-1:123456789012:secret:MySecret", "Secret");

        result.Should().BeFalse();
    }

    [Fact]
    public void IsLikelySecret_should_return_false_for_environment_variable_reference()
    {
        var result = SecretDetector.IsLikelySecret("${ENV_VAR_NAME}", "ApiKey");

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("password")]
    [InlineData("passwd")]
    [InlineData("pwd")]
    [InlineData("secret")]
    [InlineData("token")]
    [InlineData("apikey")]
    [InlineData("api_key")]
    [InlineData("private_key")]
    [InlineData("privatekey")]
    [InlineData("client_secret")]
    [InlineData("clientsecret")]
    public void IsLikelySecret_should_detect_secret_keywords_case_insensitive(string keyword)
    {
        var result = SecretDetector.IsLikelySecret("abcdefghijklmnopqrstuvwxyz123456", keyword);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("Password")]
    [InlineData("API_KEY")]
    [InlineData("ClientSecret")]
    public void IsLikelySecret_should_detect_secret_keywords_mixed_case(string keyword)
    {
        var result = SecretDetector.IsLikelySecret("abcdefghijklmnopqrstuvwxyz123456", keyword);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsLikelySecret_should_return_false_for_short_value_without_pattern()
    {
        var result = SecretDetector.IsLikelySecret("short", "Password");

        result.Should().BeFalse();
    }

    [Fact]
    public void IsLikelySecret_should_return_false_for_non_matching_pattern()
    {
        var result = SecretDetector.IsLikelySecret("this-is-not-a-secret-pattern", "ApiKey");

        result.Should().BeFalse();
    }

    #endregion

    #region IsSecureReference Tests

    [Fact]
    public void IsSecureReference_should_throw_when_value_is_null()
    {
        var act = () => SecretDetector.IsSecureReference(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsSecureReference_should_return_true_for_azure_keyvault_reference()
    {
        var result = SecretDetector.IsSecureReference("@Microsoft.KeyVault(SecretUri=https://myvault.vault.azure.net/secrets/mysecret)");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsSecureReference_should_return_true_for_azure_keyvault_reference_case_insensitive()
    {
        var result = SecretDetector.IsSecureReference("@microsoft.keyvault(SecretUri=https://myvault.vault.azure.net/secrets/mysecret)");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsSecureReference_should_return_true_for_aws_secretsmanager_reference()
    {
        var result = SecretDetector.IsSecureReference("arn:aws:secretsmanager:us-east-1:123456789012:secret:MySecret");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsSecureReference_should_return_true_for_aws_secretsmanager_reference_case_insensitive()
    {
        var result = SecretDetector.IsSecureReference("ARN:AWS:SECRETSMANAGER:us-east-1:123456789012:secret:MySecret");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsSecureReference_should_return_true_for_environment_variable_reference()
    {
        var result = SecretDetector.IsSecureReference("${ENV_VAR_NAME}");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsSecureReference_should_return_false_for_plain_text()
    {
        var result = SecretDetector.IsSecureReference("this-is-plain-text");

        result.Should().BeFalse();
    }

    [Fact]
    public void IsSecureReference_should_return_false_for_empty_string()
    {
        var result = SecretDetector.IsSecureReference(string.Empty);

        result.Should().BeFalse();
    }

    #endregion
}
