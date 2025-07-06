# Utility Classes

This folder contains utility classes used across the application.

## ConfigurationHelper

The `ConfigurationHelper` class is responsible for loading and managing application configuration from `appsettings.json`.

### Key Features

- Loading configuration from JSON file
- Type-safe access to configuration values and sections
- Strong error handling for missing or invalid configuration
- Validation of required settings
- Support for custom configuration sections

### Example Usage

```csharp
// Create an instance
var configHelper = new ConfigurationHelper();

// Get specific configuration sections
var azureConfig = configHelper.GetAzureAdConfig();
var graphConfig = configHelper.GetGraphApiConfig();

// Access individual values
string clientId = azureConfig.ClientId;
string baseUrl = graphConfig.BaseUrl;
```

See `Configuration.md` in the project root for detailed documentation on configuration options and structure.

## ConfigurationException

The `ConfigurationException` class is a custom exception used for configuration-related errors.

### Key Features

- Specific error messages for different configuration issues
- Tracking of the configuration path that caused the error
- Support for inner exceptions

### Example Usage

```csharp
try
{
    // Code that might throw ConfigurationException
    var configHelper = new ConfigurationHelper();
    var config = configHelper.GetAzureAdConfig();
}
catch (ConfigurationException ex)
{
    Console.WriteLine($"Configuration error: {ex.Message}");
    Console.WriteLine($"Configuration path: {ex.ConfigurationPath}");
}
```
