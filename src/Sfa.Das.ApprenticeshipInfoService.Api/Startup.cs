using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Sfa.Das.ApprenticeshipInfoService.Api.Middleware;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.DependencyResolution;

namespace Sfa.Das.ApprenticeshipInfoService.Api
{
    public class Startup
    {
        private const string CorsPolicyName = "AllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var corsWhitelist = Configuration.GetSection("AllowedCorsUrls").Get<string[]>();

            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName,
                builder =>
                {
                    builder.WithOrigins(corsWhitelist);
                });
            });

            services.AddApplicationInsightsTelemetry();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => 
                {
                    options.UseMemberCasing();
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "Apprenticeship Programmes API", 
                    Description = "Provides details about apprenticeship programmes, training providers and assessment organisations.",
                    Version = "v1" 
                });
                c.SwaggerDoc("v3", new OpenApiInfo 
                { 
                    Title = "Apprenticeship Programmes API", 
                    Description = "Provides details about apprenticeship programmes, training providers and assessment organisations.",
                    Version = "v3" 
                });
                c.CustomSchemaIds((type) => type.FullName);

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddApplication(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                var configuration = app.ApplicationServices.GetService<Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration>();
                configuration.DisableTelemetry = true;

                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseRootRedirect("/swagger/index.html");

            app.UseCors(CorsPolicyName);

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
            });

            app.Map("/ping", x => x.Run(async context => context.Response.StatusCode = (int)HttpStatusCode.NoContent ));

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api version 1");
                c.SwaggerEndpoint("/swagger/v3/swagger.json", "Api version 3");
                c.EnableValidator();
            });
        }
    }
}
