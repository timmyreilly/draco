// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Api.Modules;
using Draco.Api.Modules.ExtensionServices;
using Draco.Core.Hosting.Extensions;
using Draco.IntegrationTests.HowdyService;
using ExtensionService.Api.Modules;
using ExtensionService.Api.Modules.Factories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;

namespace Draco.ExtensionService.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Draco Extension Service Provider API",
                    Version = "v1"
                });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "ExtensionService.Api.xml");

                c.IncludeXmlComments(filePath);
            });

            services.AddSwaggerGenNewtonsoftSupport();

            services.ConfigureServices<StubExecutionServiceModule>(Configuration) // Stubbed - replace w/ core module in production.
                    .ConfigureServices<ExtensionServiceProviderFactoryModule>(Configuration); // Register extension services here.

            // Configure additional extension services here...
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Draco Extension Service Provider API v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
