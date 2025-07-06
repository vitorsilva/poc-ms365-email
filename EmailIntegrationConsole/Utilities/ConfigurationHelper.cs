using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace EmailIntegrationConsole.Utilities
{
    /// <summary>
    /// Helper class for loading and accessing application configuration.
    /// Provides methods to retrieve configuration settings from appsettings.json file.
    /// 
    /// The configuration file should include the following sections:
    /// - AzureAd: Contains settings for Azure Active Directory authentication
    ///   - ClientId: The Application (client) ID of your app registration in Azure portal
    ///   - TenantId: The tenant ID or domain name
    ///   - RedirectUri: The redirect URI for the authentication flow
    ///   - Scopes: An array of permission scopes required by the application
    /// 
    /// - GraphApi: Contains settings for Microsoft Graph API integration
    ///   - BaseUrl: The base URL for Microsoft Graph API
    ///   - MaxRetries: Maximum number of retry attempts for failed API calls
    ///   - RetryDelay: Delay in milliseconds between retry attempts
    /// 
    /// See Configuration.md for detailed documentation on configuration options.
    /// </summary>
    public class ConfigurationHelper
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the ConfigurationHelper class.
        /// </summary>
        public ConfigurationHelper()
        {
            _configuration = LoadConfiguration();
        }

        /// <summary>
        /// Loads the application configuration from appsettings.json.
        /// </summary>
        /// <returns>An IConfiguration object containing the application settings.</returns>
        /// <exception cref="ConfigurationException">Thrown when the configuration file cannot be found or is invalid.</exception>
        protected virtual IConfiguration LoadConfiguration()
        {
            string configFile = "appsettings.json";
            
            try
            {
                // Check if configuration file exists
                string configPath = Path.Combine(Directory.GetCurrentDirectory(), configFile);
                if (!File.Exists(configPath))
                {
                    throw new ConfigurationException($"Configuration file not found: {configPath}", configFile);
                }

                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(configFile, optional: false, reloadOnChange: true);

                return builder.Build();
            }
            catch (ConfigurationException)
            {
                // Re-throw ConfigurationException as is
                throw;
            }
            catch (InvalidDataException ex)
            {
                throw new ConfigurationException($"Configuration file '{configFile}' contains invalid JSON: {ex.Message}", configFile, ex);
            }
            catch (Exception ex)
            {
                throw new ConfigurationException($"Error loading configuration file '{configFile}': {ex.Message}", configFile, ex);
            }
        }

        /// <summary>
        /// Gets a configuration value by its key.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value as a string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the key is null or empty.</exception>
        /// <exception cref="ConfigurationException">Thrown when the configuration key is not found or the value is empty.</exception>
        public string GetValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Configuration key cannot be null or empty");
            }

            string value = _configuration[key];
            
            if (value == null)
            {
                throw new ConfigurationException($"Configuration key '{key}' not found", key);
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ConfigurationException($"Configuration key '{key}' has an empty value", key);
            }
            
            return value;
        }
        
        /// <summary>
        /// Gets a configuration value by its key with a default value if the key is not found.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <param name="defaultValue">The default value to return if the key is not found.</param>
        /// <returns>The configuration value as a string, or the default value if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the key is null or empty.</exception>
        public string GetValue(string key, string defaultValue)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Configuration key cannot be null or empty");
            }

            string value = _configuration[key];
            
            return value ?? defaultValue;
        }

        /// <summary>
        /// Gets a typed configuration section.
        /// </summary>
        /// <typeparam name="T">The type to bind the section to.</typeparam>
        /// <param name="sectionName">The name of the configuration section.</param>
        /// <returns>The configuration section bound to the specified type.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the section name is null or empty.</exception>
        /// <exception cref="ConfigurationException">Thrown when the configuration section is not found or is invalid.</exception>
        public T GetSection<T>(string sectionName) where T : class, new()
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                throw new ArgumentNullException(nameof(sectionName), "Section name cannot be null or empty");
            }

            var section = _configuration.GetSection(sectionName);
            
            if (!section.Exists())
            {
                throw new ConfigurationException($"Configuration section '{sectionName}' not found", sectionName);
            }

            try
            {
                var result = new T();
                section.Bind(result);
                
                // For some common types, validate that properties were bound successfully
                if (result is IValidatableConfig validatableConfig)
                {
                    validatableConfig.Validate(sectionName);
                }
                
                return result;
            }
            catch (Exception ex) when (!(ex is ConfigurationException))
            {
                throw new ConfigurationException($"Error binding configuration section '{sectionName}': {ex.Message}", sectionName, ex);
            }
        }
        
        /// <summary>
        /// Interface for configuration classes that can validate their contents.
        /// </summary>
        public interface IValidatableConfig
        {
            /// <summary>
            /// Validates the configuration object.
            /// </summary>
            /// <param name="sectionName">The name of the configuration section being validated.</param>
            void Validate(string sectionName);
        }

        /// <summary>
        /// Gets the Azure AD configuration section.
        /// </summary>
        /// <returns>An object containing the Azure AD configuration.</returns>
        /// <exception cref="ConfigurationException">Thrown when the Azure AD configuration is missing or invalid.</exception>
        public AzureAdConfig GetAzureAdConfig()
        {
            try
            {
                return GetSection<AzureAdConfig>("AzureAd");
            }
            catch (ConfigurationException ex)
            {
                // Provide more specific guidance for Azure AD configuration issues
                throw new ConfigurationException(
                    $"Azure AD configuration error: {ex.Message}. " +
                    "Please ensure your appsettings.json file contains a valid 'AzureAd' section with " +
                    "ClientId, TenantId, RedirectUri, and Scopes.", 
                    ex.ConfigurationPath ?? "AzureAd", 
                    ex);
            }
        }

        /// <summary>
        /// Gets the Graph API configuration section.
        /// </summary>
        /// <returns>An object containing the Graph API configuration.</returns>
        /// <exception cref="ConfigurationException">Thrown when the Graph API configuration is missing or invalid.</exception>
        public GraphApiConfig GetGraphApiConfig()
        {
            try
            {
                return GetSection<GraphApiConfig>("GraphApi");
            }
            catch (ConfigurationException ex)
            {
                // Provide more specific guidance for Graph API configuration issues
                throw new ConfigurationException(
                    $"Graph API configuration error: {ex.Message}. " +
                    "Please ensure your appsettings.json file contains a valid 'GraphApi' section with " +
                    "BaseUrl, MaxRetries, and RetryDelay.", 
                    ex.ConfigurationPath ?? "GraphApi", 
                    ex);
            }
        }
    }

    /// <summary>
    /// Class representing the Azure AD configuration section.
    /// </summary>
    public class AzureAdConfig : ConfigurationHelper.IValidatableConfig
    {
        /// <summary>
        /// Gets or sets the client ID for the Azure AD application.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the tenant ID for the Azure AD application.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the redirect URI for the Azure AD application.
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the scopes for the Azure AD application.
        /// </summary>
        public string[] Scopes { get; set; }
        
        /// <summary>
        /// Validates the Azure AD configuration.
        /// </summary>
        /// <param name="sectionName">The name of the configuration section.</param>
        /// <exception cref="ConfigurationException">Thrown when the configuration is invalid.</exception>
        public void Validate(string sectionName)
        {
            if (string.IsNullOrWhiteSpace(ClientId))
            {
                throw new ConfigurationException($"'{sectionName}:ClientId' is missing or empty", $"{sectionName}:ClientId");
            }
            
            if (string.IsNullOrWhiteSpace(TenantId))
            {
                throw new ConfigurationException($"'{sectionName}:TenantId' is missing or empty", $"{sectionName}:TenantId");
            }
            
            if (string.IsNullOrWhiteSpace(RedirectUri))
            {
                throw new ConfigurationException($"'{sectionName}:RedirectUri' is missing or empty", $"{sectionName}:RedirectUri");
            }

            if (Scopes == null || Scopes.Length == 0)
            {
                throw new ConfigurationException($"'{sectionName}:Scopes' is missing or empty", $"{sectionName}:Scopes");
            }

            foreach (var scope in Scopes)
            {
                if (string.IsNullOrWhiteSpace(scope))
                {
                    throw new ConfigurationException($"'{sectionName}:Scopes' contains an empty scope", $"{sectionName}:Scopes");
                }
            }
        }
    }

    /// <summary>
    /// Class representing the Graph API configuration section.
    /// </summary>
    public class GraphApiConfig : ConfigurationHelper.IValidatableConfig
    {
        /// <summary>
        /// Gets or sets the base URL for the Graph API.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of retries for Graph API calls.
        /// </summary>
        public int MaxRetries { get; set; }

        /// <summary>
        /// Gets or sets the delay between retries in milliseconds.
        /// </summary>
        public int RetryDelay { get; set; }
        
        /// <summary>
        /// Validates the Graph API configuration.
        /// </summary>
        /// <param name="sectionName">The name of the configuration section.</param>
        /// <exception cref="ConfigurationException">Thrown when the configuration is invalid.</exception>
        public void Validate(string sectionName)
        {
            if (string.IsNullOrWhiteSpace(BaseUrl))
            {
                throw new ConfigurationException($"'{sectionName}:BaseUrl' is missing or empty", $"{sectionName}:BaseUrl");
            }
            
            if (MaxRetries < 0)
            {
                throw new ConfigurationException($"'{sectionName}:MaxRetries' must be a non-negative value", $"{sectionName}:MaxRetries");
            }
            
            if (RetryDelay < 0)
            {
                throw new ConfigurationException($"'{sectionName}:RetryDelay' must be a non-negative value", $"{sectionName}:RetryDelay");
            }
        }
    }
}
