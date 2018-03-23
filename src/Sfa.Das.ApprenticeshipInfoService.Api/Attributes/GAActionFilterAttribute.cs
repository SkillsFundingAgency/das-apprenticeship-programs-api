using System.Diagnostics;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Attributes
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http.Controllers;
    using System.Web.Mvc;
    using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models;
    using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models;

    public class GaActionFilterAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        private const string OriginHeaderKey = "origin";
        private const string RemoteAddressHeaderKey = "REMOTE_ADDR";
        private const string DefaultClientId = "b87aa228-6bdb-44fd-ac5c-9acde7e0ba4c";
        private readonly IAnalyticsService _analyticsService;

        public GaActionFilterAttribute()
        {
            _analyticsService = DependencyResolver.Current.GetService<IAnalyticsService>();
        }

        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var incomingReq = HttpContext.Current.Request;

            RemoveRestrictedHeaders(incomingReq);

            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var assemblyInformationalVersion = fileVersionInfo.ProductVersion;

            var gaRouteArgs = new GaRouteTrackingArg
            {
                ApplicationName = AppDomain.CurrentDomain.FriendlyName,
                CtrlName = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                ActionName = actionContext.ActionDescriptor.ActionName,
                ApiVersion = assemblyInformationalVersion,
                ClientId = incomingReq.ServerVariables[OriginHeaderKey] ?? DefaultClientId,
                Host = incomingReq.Url.Host,
                RawUrl = incomingReq.RawUrl,
                AbsoluteUrl = incomingReq.Url.AbsoluteUri,
                UrlReferrer = incomingReq.UrlReferrer?.AbsoluteUri ?? string.Empty,
                RemoteIp = (incomingReq.IsLocal ? string.Empty : incomingReq.ServerVariables[RemoteAddressHeaderKey]) ?? string.Empty,
                UserAgent = incomingReq.UserAgent,
                Path = incomingReq.ApplicationPath,
                OriginalRequestHeaders = incomingReq.Headers
            };

            _analyticsService.TrackApiCall(gaRouteArgs);

            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        private void RemoveRestrictedHeaders(HttpRequest incomingReq)
        {
            incomingReq.Headers.Remove("Accept");
            incomingReq.Headers.Remove("Connection");
            incomingReq.Headers.Remove("Content-Length");
            incomingReq.Headers.Remove("Content-Type");
            incomingReq.Headers.Remove("Date");
            incomingReq.Headers.Remove("Expect");
            incomingReq.Headers.Remove("Host");
            incomingReq.Headers.Remove("If-Modified-Since");
            incomingReq.Headers.Remove("Range");
            incomingReq.Headers.Remove("Referer");
            incomingReq.Headers.Remove("Transfer-Encoding");
            incomingReq.Headers.Remove("User-Agent");
            incomingReq.Headers.Remove("Proxy-Connection");
        }
    }
}