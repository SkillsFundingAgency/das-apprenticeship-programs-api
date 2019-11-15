namespace Sfa.Das.ApprenticeshipInfoService.Core.Services
{
    using System.Collections.Generic;
    using Sfa.Das.ApprenticeshipInfoService.Core.Models;
    using SFA.DAS.Apprenticeships.Api.Types.V3;

    public interface IGetProviderApprenticeshipLocationsV3
    {
        ProviderApprenticeshipLocationSearchResult SearchStandardProviders(int standardId, Coordinate coordinates, int page, int pageSize, bool showForNonLevyOnly, bool showNationalOnly, List<DeliveryMode> deliverModes, int orderBy = 0);
        ProviderApprenticeshipLocationSearchResult SearchFrameworkProviders(string frameworkId, Coordinate coordinates, int page, int pageSize, bool showForNonLevyOnly, bool showNationalOnly, List<DeliveryMode> deliverModes, int orderBy = 0);
        UniqueProviderApprenticeshipLocationSearchResult SearchStandardProviderLocations(int standardId, Coordinate coordinates, int actualPage, int pageSize, bool showForNonLevyOnly, bool showNationalOnly);
        UniqueProviderApprenticeshipLocationSearchResult SearchFrameworkProvidersLocations(string id, Coordinate coordinates, int actualPage, int pageSize, bool showForNonLevyOnly, bool showNationalOnly);


        ProviderLocationsSearchResults GetClosestLocationsForStandard(long ukprn, int standardId, Coordinate coordinates, int page, int pageSize, bool showForNonLevyOnly);
        ProviderLocationsSearchResults GetClosestLocationsForFramework(long ukprn, string frameworkId, Coordinate coordinates, int page, int pageSize, bool showForNonLevyOnly);

    
    }
}
