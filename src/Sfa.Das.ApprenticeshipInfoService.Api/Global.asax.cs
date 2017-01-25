﻿namespace Sfa.Das.ApprenticeshipInfoService.Api
{
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Sfa.Das.ApprenticeshipInfoService.Core.Logging;

    public class WebApiApplication : System.Web.HttpApplication
    {
        private readonly ILog logger;

        public WebApiApplication()
        {
            logger = DependencyResolver.Current.GetService<ILog>();
        }

        protected void Application_Start()
        {
            logger.Info("Starting Web Role");

            RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configure(WebApiConfig.Register);

            logger.Info("Web Role started");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();
            var logger = DependencyResolver.Current.GetService<ILog>();

            if (ex is HttpException && ((HttpException)ex).GetHttpCode() != 404)
            {
                logger.Error(ex, ex.Message);
            }
        }

        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "HealthRoute",
                url: "health/{action}/{id}",
                defaults: new { controller = "Health", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "HomeRoute",
                url: "home/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "DefaultRoute",
                url: string.Empty,
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}