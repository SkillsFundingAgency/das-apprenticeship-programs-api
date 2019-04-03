using System.Web.Http.Cors;

namespace Sfa.Das.ApprenticeshipInfoService.Api
{
    using System.Web.Http;
    using Microsoft.Azure;
    using Newtonsoft.Json;
    using Sfa.Das.ApprenticeshipInfoService.Api.Attributes;
    using Sfa.Das.ApprenticeshipInfoService.Api.Swagger;
    using Sfa.Das.ApprenticeshipInfoService.Infrastructure.FeatureToggles;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            SwaggerSetup.Configure(config);
            if (new GaApiAnalyticsFeature().FeatureEnabled)
            {
                // Web API configuration and services
                config.Filters.Add(new GaActionFilterAttribute());
            }

            // Web API routes
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            ConfigureCors(config);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }

        private static void ConfigureCors(HttpConfiguration config)
        {
            var corsUrls = CloudConfigurationManager.GetSetting("AllowedCorsUrls");

            if (!string.IsNullOrWhiteSpace(corsUrls))
            {
                var corsAttr = new EnableCorsAttribute(corsUrls, "*", "*");
                config.EnableCors(corsAttr);
            }
        }
    }
}
