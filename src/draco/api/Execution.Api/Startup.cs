// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Api.Modules;
using Draco.Api.Modules.Azure;
using Draco.Api.Modules.ExtensionServices;
using Draco.Core.Hosting.Extensions;
using Draco.Execution.Api.Modules;
using Draco.Execution.Api.Modules.Azure;
using Draco.Execution.Api.Modules.Factories;
using Execution.Api.Modules.Factories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;

namespace Draco.Execution.Api
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
                    Title = "Draco Execution API",
                    Version = "v1"
                });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "Execution.Api.xml");

                c.IncludeXmlComments(filePath);
            });

            services.AddSwaggerGenNewtonsoftSupport();

            ConfigureCoreServices(services);
            ConfigureAzureServices(services);
        }

        private void ConfigureCoreServices(IServiceCollection services) =>
            services.ConfigureServices<DefaultUserContextModule>(Configuration) // Only during testing.
                    .ConfigureServices<CoreExecutionApiModule>(Configuration)
                    .ConfigureServices<CoreObjectStorageModule>(Configuration)
                    .ConfigureServices<CoreExecutionPipelineModule>(Configuration)
                    .ConfigureServices<ExecutionProcessorFactoryModule>(Configuration) // Configure additional execution adapters here.
                    .ConfigureServices<ExecutionServiceProviderFactoryModule>(Configuration) // Configure additional service providers here.
                    .ConfigureServices<InputObjectAccessorProviderFactoryModule>(Configuration) // Configure additional input object accessor providers here.
                    .ConfigureServices<OutputObjectAccessorProviderFactoryModule>(Configuration)
                    .ConfigureServices<JsonHttpExecutionAdapterModule>(Configuration)
                    .ConfigureServices<HowdyExecutionServiceModule>(Configuration)
                    .ConfigureServices<SignerModule>(Configuration);

        // Follow this pattern when adding additional platforms.
        private void ConfigureAzureServices(IServiceCollection services) =>
            services.ConfigureServices<AzureExecutionPipelineModule>(Configuration)
                    .ConfigureServices<AzureObjectStorageModule>(Configuration)
                    .ConfigureServices<AzureRepositoryModule>(Configuration);

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Draco Execution API v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
