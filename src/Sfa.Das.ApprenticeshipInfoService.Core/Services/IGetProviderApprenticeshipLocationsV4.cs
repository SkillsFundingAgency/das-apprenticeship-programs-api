namespace Sfa.Das.ApprenticeshipInfoService.Core.Services
{
    using Sfa.Das.ApprenticeshipInfoService.Core.Models;
    using SFA.DAS.Apprenticeships.Api.Types.V4;

    public interface IGetProviderApprenticeshipLocationsV4
    {
        UniqueProviderApprenticeshipLocationSearchResult SearchStandardProviderLocations(int standardId, Coordinate coordinates, int actualPage, int pageSize, bool showForNonLevyOnly, bool showNationalOnly);
        UniqueProviderApprenticeshipLocationSearchResult SearchFrameworkProvidersLocations(string id, Coordinate coordinates, int actualPage, int pageSize, bool showForNonLevyOnly, bool showNationalOnly);
        ProviderLocationsSearchResults GetClosestLocationsForStandard(long ukprn, int standardId, Coordinate coordinates, int page, int pageSize, bool showForNonLevyOnly);
        ProviderLocationsSearchResults GetClosestLocationsForFramework(long ukprn, string frameworkId, Coordinate coordinates, int page, int pageSize, bool showForNonLevyOnly);
    }
}
