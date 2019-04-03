using System.Web.Http.Cors;

namespace Sfa.Das.ApprenticeshipInfoService.Api
{
    using System.Web.Http;
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
            //config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            var corsAttr = new EnableCorsAttribute("http://localhost:3000,https://localhost:1045,http://fire-it-up.herokuapp.com,https://esfa-shopping-basket.herokuapp.com", "*", "*");
            config.EnableCors(corsAttr);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }
    }
}
