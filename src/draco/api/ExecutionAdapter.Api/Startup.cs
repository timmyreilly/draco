// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Api.Modules;
using Draco.Core.Hosting.Extensions;
using Draco.ExecutionAdapter.Api.Modules;
using Draco.ExecutionAdapter.Api.Modules.Factories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;

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
        }

        private void ConfigureCoreServices(IServiceCollection services) =>
           services.ConfigureServices<StubObjectStorageModule>(Configuration) // Stubbed - replace w/ core module in production.
                   .ConfigureServices<StubExecutionPipelineModule>(Configuration) // Stubbed - replace w/ core module in production.
                   .ConfigureServices<StubExecutionServiceModule>(Configuration) // Stubbed - replace w/ core module in production.
                   .ConfigureServices<ExecutionProcessorFactoryModule>(Configuration) // Configure additional execution adapters here.
                   .ConfigureServices<ExecutionServiceProviderFactoryModule>(Configuration) // Configure additional service providers here.
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Draco Execution Adapter API v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
