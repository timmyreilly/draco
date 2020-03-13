// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Api.Modules;
using Draco.Api.InternalModels.Extensions;
using Draco.Api.Modules;
using Draco.Api.Modules.Azure;
using Draco.Core.Hosting.Extensions;
using Draco.ObjectStorageProvider.Api.Modules;
using Draco.ObjectStorageProvider.Api.Modules.Factories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;

namespace Draco.ObjectStorageProvider.Api
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
                    Title = "Draco Object Storage Provider API",
                    Version = "v1"
                });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "ObjectStorageProvider.Api.xml");

                c.IncludeXmlComments(filePath);
            });

            services.AddSwaggerGenNewtonsoftSupport();

            ConfigureCoreServices(services);
        }

        private void ConfigureCoreServices(IServiceCollection services) =>
           services.ConfigureServices<StubObjectStorageModule>(Configuration) // Stubbed - replace w/ core storage module in production.
                   .ConfigureServices<InputObjectAccessorProviderFactoryModule>(Configuration) // Configure additional input object accessor providers here.
                   .ConfigureServices<OutputObjectAccessorProviderFactoryModule>(Configuration) // Configure additional output object accessor providers here.
                   .ConfigureServices<SignerModule>(Configuration);

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Draco Object Storage Provider API v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
