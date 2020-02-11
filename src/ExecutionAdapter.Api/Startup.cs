using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Draco.Api.Modules;
using Draco.Api.Modules.Azure;
using Draco.Api.Modules.ExtensionServices;
using Draco.Core.Hosting.Extensions;
using Draco.ExecutionAdapter.Api.Modules;
using Draco.ExecutionAdapter.Api.Modules.Azure;
using Draco.ExecutionAdapter.Api.Modules.Factories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Draco.ExecutionAdapter.Api
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
                    Title = "Draco Execution Adapter API",
                    Version = "v1"
                });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "ExecutionAdapter.Api.xml");

                c.IncludeXmlComments(filePath);
            });

            services.AddSwaggerGenNewtonsoftSupport();

            ConfigureCoreServices(services);
            ConfigureAzureServices(services);
        }

        private void ConfigureCoreServices(IServiceCollection services) =>
           services.ConfigureServices<CoreObjectStorageModule>(Configuration)
                   .ConfigureServices<CoreExecutionPipelineModule>(Configuration)
                   .ConfigureServices<ExecutionProcessorFactoryModule>(Configuration) // Configure additional execution adapters here.
                   .ConfigureServices<ExecutionServiceProviderFactoryModule>(Configuration) // Configure additional service providers here.
                   .ConfigureServices<ObjectAccessorProviderFactoryModule>(Configuration) // Configure additional object accessor providers here.
                   .ConfigureServices<JsonHttpExecutionAdapterModule>(Configuration)
                   .ConfigureServices<HowdyExecutionServiceModule>(Configuration)
                   .ConfigureServices<SignerModule>(Configuration);

        // Follow this pattern when adding additional platforms.
        private void ConfigureAzureServices(IServiceCollection services) =>
            services.ConfigureServices<AzureExecutionPipelineModule>(Configuration)
                    .ConfigureServices<AzureObjectStorageModule>(Configuration);

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Draco Execution Adapter API v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
