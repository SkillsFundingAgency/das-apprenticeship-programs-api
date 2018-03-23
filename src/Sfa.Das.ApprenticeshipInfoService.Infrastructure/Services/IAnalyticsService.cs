using System.Threading.Tasks;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models
{
    public interface IAnalyticsService
    {
        Task TrackApiCall(GaRouteTrackingArg gaRouteArgs);
    }
}