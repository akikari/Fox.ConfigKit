//==================================================================================================
// Validation rule that ensures a network port is available for binding.
// Sealed class implementation for port availability checking.
//==================================================================================================
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using Fox.ConfigKit.Errors;

namespace Fox.ConfigKit.Validation.Rules;

//==================================================================================================
/// <summary>
/// Validates that a network port is available.
/// </summary>
/// <typeparam name="T">The type of the configuration class.</typeparam>
/// <param name="selector">Expression that selects the port property.</param>
/// <param name="customMessage">Optional custom error message.</param>
//==================================================================================================
internal sealed class PortAvailableRule<T>(Expression<Func<T, int>> selector, string? customMessage = null) : ValidationRuleBase, IValidationRule<T> where T : class
{
    #region Fields

    private readonly Func<T, int> getValue = selector.Compile();
    private readonly string propertyName = GetPropertyName(selector);

    #endregion

    #region Public Methods

    //==============================================================================================
    /// <summary>
    /// Validates the rule.
    /// </summary>
    /// <param name="options">The configuration object to validate.</param>
    /// <param name="sectionName">The configuration section name.</param>
    /// <returns>A validation error if the rule fails; otherwise, null.</returns>
    //==============================================================================================
    public ConfigValidationError? Validate(T options, string sectionName)
    {
        var port = getValue(options);

        if (port is < 1 or > 65535)
        {
            var key = $"{sectionName}:{propertyName}";
            var message = $"Port number must be between 1 and 65535 (current: {port})";
            return new ConfigValidationError(key, message, port, ["Use a valid port number"]);
        }

        if (!IsPortAvailable(port))
        {
            var key = $"{sectionName}:{propertyName}";
            var message = customMessage ?? $"Port {port} is already in use";
            return new ConfigValidationError(key, message, port, ["Choose a different port or stop the service using this port"]);
        }

        return null;
    }

    #endregion

    #region Private Methods

    //==============================================================================================
    /// <summary>
    /// Checks if a port is available for binding.
    /// </summary>
    /// <param name="port">The port number to check.</param>
    /// <returns>True if the port is available; otherwise, false.</returns>
    //==============================================================================================
    private static bool IsPortAvailable(int port)
    {
        try
        {
            var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            listener.Stop();
            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }

    #endregion
}
