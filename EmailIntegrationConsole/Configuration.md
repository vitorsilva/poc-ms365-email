# Configuration Documentation

This document provides information about the application's configuration options and structure.

## Overview

The application uses the `ConfigurationHelper` class to load settings from `appsettings.json`. This class provides methods for accessing configuration values and sections in a type-safe manner.

## Configuration File Structure

The `appsettings.json` file should have the following structure:

```json
{
  "AzureAd": {
    "ClientId": "your-client-id",
    "TenantId": "your-tenant-id",
    "RedirectUri": "http://localhost:8080",
    "Scopes": [
      "https://graph.microsoft.com/Mail.Read",
      "https://graph.microsoft.com/User.Read"
    ]
  },
  "GraphApi": {
    "BaseUrl": "https://graph.microsoft.com/v1.0",
    "MaxRetries": 3,
    "RetryDelay": 1000
  }
}
```

## Configuration Sections

### AzureAd Section

This section contains settings for Azure Active Directory authentication.

| Setting | Description | Required | Default |
|---------|-------------|----------|---------|
| ClientId | The Application (client) ID of your app registration in Azure portal | Yes | None |
| TenantId | The tenant ID or domain name (e.g., "contoso.onmicrosoft.com") | Yes | None |
| RedirectUri | The redirect URI for the authentication flow | Yes | None |
| Scopes | An array of permission scopes required by the application | Yes | None |

### GraphApi Section

This section contains settings for Microsoft Graph API integration.

| Setting | Description | Required | Default |
|---------|-------------|----------|---------|
| BaseUrl | The base URL for Microsoft Graph API | Yes | None |
| MaxRetries | Maximum number of retry attempts for failed API calls | Yes | None |
| RetryDelay | Delay in milliseconds between retry attempts | Yes | None |

## Error Handling

The application includes comprehensive error handling for configuration issues:

- Missing configuration file
- Invalid JSON in the configuration file
- Missing required configuration sections
- Missing required settings
- Invalid setting values

When a configuration error occurs, a `ConfigurationException` is thrown with details about the specific issue.

## Usage Example

```csharp
try
{
    var configHelper = new ConfigurationHelper();
    var azureConfig = configHelper.GetAzureAdConfig();
    var graphConfig = configHelper.GetGraphApiConfig();
    
    // Use the configurations...
}
catch (ConfigurationException ex)
{
    Console.WriteLine($"Configuration error: {ex.Message}");
    Console.WriteLine($"Configuration path: {ex.ConfigurationPath}");
}
```
