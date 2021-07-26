using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using TrueLayer.DataProduct.Pokemon.Manager.Manager;
using TrueLayer.DataProduct.Pokemon.Shared.Contract.Manager;
using TrueLayer.DataProduct.Pokemon.Shared.Infra;

namespace TrueLayer.DataProduct.Pokemon.RestService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var apiConfig = Configuration.GetSection(nameof(ApiConfiguration)).Get<ApiConfiguration>();
            services.Configure<ApiConfiguration>(options => Configuration.GetSection(nameof(ApiConfiguration)).Bind(options));

            services.AddHttpClient();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TrueLayer.DataProduct.Pokemon.RestService", Version = "v1" });
            });

            services.AddHttpClient("pokeapi", c =>
            {
                c.BaseAddress = new Uri(apiConfig.PokeapiEndPoint);
                c.DefaultRequestHeaders.Add("Accept", apiConfig.HeaderAccept);
                c.DefaultRequestHeaders.Add("User-Agent", apiConfig.UserAgent);
            });

            services.AddHttpClient("translateapi", c =>
            {
                c.BaseAddress = new Uri(apiConfig.TranslateapiEndPoint);
                c.DefaultRequestHeaders.Add("Accept", apiConfig.HeaderAccept);
                c.DefaultRequestHeaders.Add("User-Agent", apiConfig.UserAgent);
            });

            services.AddScoped<IPokemonManager, PokemonManager>();
        }

        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TrueLayer.DataProduct.Pokemon.RestService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
