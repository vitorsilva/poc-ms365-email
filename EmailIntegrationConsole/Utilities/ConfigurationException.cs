using System;

namespace EmailIntegrationConsole.Utilities
{
    /// <summary>
    /// Exception thrown when there are issues with the application configuration.
    /// </summary>
    public class ConfigurationException : Exception
    {
        /// <summary>
        /// Gets the name of the configuration section or key that caused the exception.
        /// </summary>
        public string ConfigurationPath { get; }

        /// <summary>
        /// Initializes a new instance of the ConfigurationException class.
        /// </summary>
        public ConfigurationException() : base("An error occurred with the application configuration.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the ConfigurationException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConfigurationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ConfigurationException class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ConfigurationException class with a specified error message
        /// and the name of the configuration section or key that caused the exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="configurationPath">The name of the configuration section or key that caused the exception.</param>
        public ConfigurationException(string message, string configurationPath) : base(message)
        {
            ConfigurationPath = configurationPath;
        }

        /// <summary>
        /// Initializes a new instance of the ConfigurationException class with a specified error message,
        /// the name of the configuration section or key that caused the exception, and a reference to the inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="configurationPath">The name of the configuration section or key that caused the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ConfigurationException(string message, string configurationPath, Exception innerException) : base(message, innerException)
        {
            ConfigurationPath = configurationPath;
        }
    }
}