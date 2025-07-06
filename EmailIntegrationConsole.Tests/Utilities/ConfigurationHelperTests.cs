using System;
using System.IO;
using EmailIntegrationConsole.Utilities;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace EmailIntegrationConsole.Tests.Utilities
{
    public class ConfigurationHelperTests
    {
        private readonly string _testSettingsPath;
        private readonly ConfigurationHelperForTest _configHelper;

        public ConfigurationHelperTests()
        {
            // Set up the test environment
            _testSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.test.json");
            _configHelper = new ConfigurationHelperForTest(_testSettingsPath);
        }

        [Fact]
        public void GetValue_ValidKey_ReturnsCorrectValue()
        {
            // Arrange
            const string key = "TestKey";
            const string expectedValue = "TestValue";

            // Act
            string actualValue = _configHelper.GetValue(key);

            // Assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void GetValue_NullKey_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _configHelper.GetValue(null));
            Assert.Equal("key", exception.ParamName);
        }

        [Fact]
        public void GetValue_EmptyKey_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _configHelper.GetValue(""));
            Assert.Equal("key", exception.ParamName);
        }

        [Fact]
        public void GetValue_NonExistentKey_ThrowsKeyNotFoundException()
        {
            // Arrange
            const string nonExistentKey = "NonExistentKey";

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _configHelper.GetValue(nonExistentKey));
        }

        [Fact]
        public void GetSection_ValidSectionName_ReturnsCorrectObject()
        {
            // Arrange & Act
            var graphApiConfig = _configHelper.GetSection<GraphApiConfig>("GraphApi");

            // Assert
            Assert.NotNull(graphApiConfig);
            Assert.Equal("https://graph.microsoft.com/test", graphApiConfig.BaseUrl);
            Assert.Equal(5, graphApiConfig.MaxRetries);
            Assert.Equal(2000, graphApiConfig.RetryDelay);
        }

        [Fact]
        public void GetSection_NullSectionName_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _configHelper.GetSection<GraphApiConfig>(null));
            Assert.Equal("sectionName", exception.ParamName);
        }

        [Fact]
        public void GetSection_EmptySectionName_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _configHelper.GetSection<GraphApiConfig>(""));
            Assert.Equal("sectionName", exception.ParamName);
        }

        [Fact]
        public void GetSection_NonExistentSectionName_ThrowsKeyNotFoundException()
        {
            // Arrange
            const string nonExistentSection = "NonExistentSection";

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _configHelper.GetSection<GraphApiConfig>(nonExistentSection));
        }

        [Fact]
        public void GetAzureAdConfig_ReturnsCorrectObject()
        {
            // Arrange & Act
            var azureAdConfig = _configHelper.GetAzureAdConfig();

            // Assert
            Assert.NotNull(azureAdConfig);
            Assert.Equal("test-client-id", azureAdConfig.ClientId);
            Assert.Equal("test-tenant-id", azureAdConfig.TenantId);
            Assert.Equal("http://localhost:9090", azureAdConfig.RedirectUri);
            Assert.Equal(2, azureAdConfig.Scopes.Length);
            Assert.Equal("https://graph.microsoft.com/Mail.Read.Test", azureAdConfig.Scopes[0]);
            Assert.Equal("https://graph.microsoft.com/User.Read.Test", azureAdConfig.Scopes[1]);
        }

        [Fact]
        public void GetGraphApiConfig_ReturnsCorrectObject()
        {
            // Arrange & Act
            var graphApiConfig = _configHelper.GetGraphApiConfig();

            // Assert
            Assert.NotNull(graphApiConfig);
            Assert.Equal("https://graph.microsoft.com/test", graphApiConfig.BaseUrl);
            Assert.Equal(5, graphApiConfig.MaxRetries);
            Assert.Equal(2000, graphApiConfig.RetryDelay);
        }
    }

    // Test helper subclass to inject test settings file path
    public class ConfigurationHelperForTest : ConfigurationHelper
    {
        private readonly IConfiguration _testConfiguration;

        public ConfigurationHelperForTest(string configPath)
        {
            _testConfiguration = LoadTestConfiguration(configPath);
        }

        private IConfiguration LoadTestConfiguration(string configPath)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile(configPath, optional: false, reloadOnChange: true);

                return builder.Build();
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException($"Error loading test configuration: {ex.Message}", ex);
            }
        }

        // Override the LoadConfiguration method to use our test configuration
        protected override IConfiguration LoadConfiguration()
        {
            return _testConfiguration;
        }
    }
}
