using Microsoft.Azure;
using NLog.Internal;
using SFA.DAS.NLog.Logger;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace Sfa.Das.ApprenticeshipInfoService.Api
{
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class WebApiApplication : System.Web.HttpApplication
    {
        private ILog _logger;

        public WebApiApplication()
        {
            _logger = DependencyResolver.Current.GetService<ILog>();
        }

        protected void Application_Start()
        {
            _logger.Info("Starting Web Role");

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configure(WebApiConfig.Register);

            _logger.Info("Web Role started");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();
            var logger = DependencyResolver.Current.GetService<ILog>();

            var exception = ex as HttpException;
            if (exception == null || exception.GetHttpCode() != 404)
            {
                logger.Error(ex, ex.Message);
            }
            else
            {
                logger.Warn(ex, "App_Warn");
            }
        }

        protected internal void Application_BeginRequest(object sender, EventArgs e)
        {
            _logger = DependencyResolver.Current.GetService<ILog>();

            var application = sender as HttpApplication;
            application?.Context?.Response.Headers.Remove("Server");

            HttpContext context = base.Context;
            if (!context.Request.Path.Equals("/")
                && !context.Request.Path.Contains("swagger")
                && !context.Request.Path.StartsWith("/__browserlink"))
            {
                _logger.Info($"{context.Request.HttpMethod} {context.Request.Path}");
            }

            var redirectUrl = CloudConfigurationManager.GetSetting("RedirectUrl");
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                var requestRawUrl = application?.Context?.Request.RawUrl;
                _logger.Info($"Redirecting request from UserHost:{context.Request.UserHostAddress} to {redirectUrl} for request {requestRawUrl}");

                Response.RedirectPermanent($"{redirectUrl}{requestRawUrl}");
            }
        }
    }
}