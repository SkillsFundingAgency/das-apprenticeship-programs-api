using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using SFA.DAS.Apprenticeships.Api.Types.Providers;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Services
{
    using System.Collections.Generic;
    using Models.Responses;

    public interface IGetProviders
    {
        IEnumerable<ProviderSummary> GetAllProviders();

        Provider GetProviderByUkprn(long ukprn);

        IEnumerable<Provider> GetProviderByUkprnList(List<long> ukprns);

        List<StandardProviderSearchResultsItemResponse> GetByStandardIdAndLocation(int id, double lat, double lon, int page);

        List<FrameworkProviderSearchResultsItemResponse> GetByFrameworkIdAndLocation(int id, double lat, double lon, int page);

        IEnumerable<StandardProviderSearchResultsItem> GetProvidersByStandardId(string standardId);

        IEnumerable<FrameworkProviderSearchResultsItem> GetProvidersByFrameworkId(string frameworkId);

        IEnumerable<ProviderFramework> GetFrameworksByProviderUkprn(long ukprn);
        IEnumerable<ProviderStandard> GetStandardsByProviderUkprn(long ukprn);
    }
}
