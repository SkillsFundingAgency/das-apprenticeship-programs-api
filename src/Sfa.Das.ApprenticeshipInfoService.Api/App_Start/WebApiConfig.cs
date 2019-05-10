using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Azure;
using Newtonsoft.Json;
using Sfa.Das.ApprenticeshipInfoService.Api.Attributes;
using Sfa.Das.ApprenticeshipInfoService.Api.Swagger;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.FeatureToggles;
using SFA.DAS.NLog.Logger;

namespace Sfa.Das.ApprenticeshipInfoService.Api
{
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

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }

        public static void ConfigureCors(HttpConfiguration config, ILog logger)
        {
            var corsUrls = CloudConfigurationManager.GetSetting("AllowedCorsUrls");

            logger.Debug("Allowing CORS for: " + corsUrls);

            if (!string.IsNullOrWhiteSpace(corsUrls))
            {
                var corsAttr = new EnableCorsAttribute(corsUrls, "*", "*");
                config.EnableCors(corsAttr);
            }
        }
    }
}
