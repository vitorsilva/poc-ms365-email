// Office 365 Email Integration Console Application
using EmailIntegrationConsole.Models;
using EmailIntegrationConsole.Utilities;
using System;

Console.WriteLine("=================================");
Console.WriteLine("OFFICE 365 EMAIL INTEGRATION");
Console.WriteLine("=================================");
Console.WriteLine("Status: [Not Authenticated]");
Console.WriteLine();

// Test configuration loading
Console.WriteLine("Testing Configuration Loading:");
Console.WriteLine("-----------------------------");
try
{
    var configHelper = new ConfigurationHelper();
    
    // Test AzureAd configuration
    try
    {
        var azureConfig = configHelper.GetAzureAdConfig();
        Console.WriteLine($"Azure AD Client ID: {azureConfig.ClientId}");
        Console.WriteLine($"Azure AD Tenant ID: {azureConfig.TenantId}");
        Console.WriteLine($"Azure AD Redirect URI: {azureConfig.RedirectUri}");
        Console.WriteLine($"Azure AD Scopes: {string.Join(", ", azureConfig.Scopes)}");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Azure AD configuration is valid.");
        Console.ResetColor();
    }
    catch (ConfigurationException ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Azure AD configuration error: {ex.Message}");
        Console.WriteLine($"Configuration path: {ex.ConfigurationPath}");
        Console.ResetColor();
    }
    
    Console.WriteLine();
    
    // Test GraphApi configuration
    try
    {
        var graphConfig = configHelper.GetGraphApiConfig();
        Console.WriteLine($"Graph API Base URL: {graphConfig.BaseUrl}");
        Console.WriteLine($"Graph API Max Retries: {graphConfig.MaxRetries}");
        Console.WriteLine($"Graph API Retry Delay: {graphConfig.RetryDelay} ms");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Graph API configuration is valid.");
        Console.ResetColor();
    }
    catch (ConfigurationException ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Graph API configuration error: {ex.Message}");
        Console.WriteLine($"Configuration path: {ex.ConfigurationPath}");
        Console.ResetColor();
    }
    
    // Test non-existent configuration (to demonstrate error handling)
    try
    {
        Console.WriteLine("\nTesting error handling for missing configuration:");
        var nonExistentValue = configHelper.GetValue("NonExistentSection:NonExistentKey");
        Console.WriteLine($"This should not be displayed: {nonExistentValue}");
    }
    catch (ConfigurationException ex)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Expected error (demonstrating error handling): {ex.Message}");
        Console.ResetColor();
    }
    
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\nConfiguration loading test completed successfully!");
    Console.ResetColor();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nUnexpected error: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
    }
    Console.WriteLine($"Exception type: {ex.GetType().Name}");
    Console.ResetColor();
}

Console.WriteLine("\nProject setup completed successfully!");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
