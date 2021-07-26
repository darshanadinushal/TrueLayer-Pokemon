using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WireMock.Server;
using TrueLayer.DataProduct.Pokemon.RestService;

namespace TrueLayer.DataProduct.RestService.Integration.Tests.TestFixtures
{
    [CollectionDefinition("Integration test collection")]
    public class IntegrationTestCollectionEmployeeApi : ICollectionFixture<IntegrationTestFixture<Startup>>
    {

    }

    /// <summary>
    /// Test fixture to help setup server and client for integration test.
    /// </summary>
    /// <typeparam name="TStartup"></typeparam>
    public class IntegrationTestFixture<TStartup> : IDisposable, IAsyncDisposable where TStartup : class
    {

        private readonly IntegrationTestAppFactory<TStartup> appFactory;

        private readonly Lazy<TestServer> server;

        public IConfiguration Configuration => server.Value.Services.GetRequiredService<IConfiguration>();

        public WireMockServer WireMockServer => server.Value.Services.GetRequiredService<WireMockServer>();

        public IntegrationTestFixture()
            : this("Integration", null, null)
        {

        }

        private IntegrationTestFixture(
            string environment,
            IEnumerable<KeyValuePair<string, string>> initialConfig = null,
            Action<WebHostBuilderContext, IServiceCollection> configureServices = null)
        {
            appFactory = new IntegrationTestAppFactory<TStartup>(environment, initialConfig, configureServices);
            server = new Lazy<TestServer>(() => appFactory.Server);
        }

        /// <summary>
        /// Create <see cref="HttpClient"/> with predefined headers.
        /// </summary>
        /// <returns>HttpClient.</returns>

        public HttpClient CreateClient()
        {
            return CreateClient(new Dictionary<string, StringValues>
            {
                { "Content-Type", "application/json" }
            });

        }

        /// <summary>
        /// Create <see cref="HttpClient"/> with customized headers.
        /// </summary>
        /// <returns>HttpClient.</returns>
        public HttpClient CreateClient(Dictionary<string, StringValues> headerValues)
        {
            var applicationUrl = Configuration.GetSection("HostApplicationUrl").Get<string>();
            if (appFactory.Server != null)
            {
                appFactory.Server.BaseAddress = new Uri(applicationUrl);
            }
            else
            {
                appFactory.ClientOptions.BaseAddress = new Uri(applicationUrl);
            }
            var client = server.Value.CreateClient();
            foreach (var item in headerValues)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, (IEnumerable<string>)item.Value);
            }
            return client;
        }

        public string GenerateFullUrl(string relativeUrl)
        {
            return $"{server.Value.BaseAddress}{relativeUrl}";
        }

        public void Dispose()
        {
            appFactory.Server?.Dispose();
            appFactory.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            appFactory.Server?.Dispose();
            appFactory.Dispose();
        }
    }

}
