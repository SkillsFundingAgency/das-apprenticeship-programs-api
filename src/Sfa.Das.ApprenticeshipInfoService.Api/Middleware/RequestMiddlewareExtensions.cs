using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Middleware
{
    public static class RequestMiddlewareExtensions
    {
        public static IApplicationBuilder UseRootRedirect(this IApplicationBuilder builder, string redirectUrl)
        {
            return builder.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Response.Redirect(redirectUrl, true);
                    await Task.CompletedTask;
                }
                else
                {
                    await next.Invoke();
                }
            });
        }
    }
}