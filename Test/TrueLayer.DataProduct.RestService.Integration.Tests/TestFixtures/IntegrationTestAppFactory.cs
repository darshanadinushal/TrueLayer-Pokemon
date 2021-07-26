using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using WireMock.Server;
using WireMock.Settings;

namespace TrueLayer.DataProduct.RestService.Integration.Tests.TestFixtures
{
    /// <summary>
    /// Application factory based on <see cref="WebApplicationFactory{TEntryPoint}"/>.
    /// </summary>
    /// <typeparam name="TStartup">Startup class to be initialized.</typeparam>
    public class IntegrationTestAppFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {

        private readonly string environment;
        private readonly IEnumerable<KeyValuePair<string, string>> initialConfig;
        private readonly Action<WebHostBuilderContext, IServiceCollection> configureServices;



        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationTestAppFactory"/> class.
        /// </summary>
        /// <param name="environment">Environment to select appsettings.json.</param>
        /// <param name="initialConfig">Initial config to be added into in-memory configuration collection.</param>
        /// <param name="configureServices">Additional steps to configure service collection.</param>
        public IntegrationTestAppFactory(

            string environment,
            IEnumerable<KeyValuePair<string, string>> initialConfig,
            Action<WebHostBuilderContext, IServiceCollection> configureServices)

        {
            this.environment = environment;
            this.initialConfig = initialConfig;
            this.configureServices = configureServices;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(environment);
            var appsettingsPath = Path.Combine(Directory.GetCurrentDirectory(), $"appsettings.{environment}.json");

            builder.ConfigureAppConfiguration((WebHostBuilderContext context, IConfigurationBuilder configuration) =>
            {
                configuration.Sources.Clear();
                configuration.AddJsonFile(appsettingsPath);
            });

            builder.ConfigureAppConfiguration(configurationBuilder =>
            {
                if (initialConfig != null)
                {
                    configurationBuilder.AddInMemoryCollection(initialConfig);
                }
            });

            // Setup mock host from appsetting base URL.
            builder.ConfigureServices((WebHostBuilderContext context, IServiceCollection services) =>
            {
                var clientSettings = context.Configuration.GetSection("HttpClientConfiguration:ClientSettings");
                if (clientSettings.Exists())
                {
                    var setting = clientSettings.GetChildren();
                    var baseUrls = setting.Select(x =>
                    {
                        var uri = new Uri(x["BaseUrl"]);
                        return $"{uri.Scheme}://{uri.Authority}";
                    }).Distinct().ToArray();

                    var wiremockServer = WireMockServer.Start(new WireMockServerSettings
                    {
                        Urls = baseUrls
                    });
                    services.AddSingleton(wiremockServer);
                }
                else
                {
                    var wiremockServer = WireMockServer.Start();
                    services.AddSingleton(wiremockServer);
                }
            });

            if (configureServices != null)
            {
                builder.ConfigureServices(configureServices);
            }
            builder.ConfigureServices(sc => sc.AddSingleton(sc));
            builder.UseKestrel(options => options.Listen(IPAddress.Any, 80));
        }

    }
}
