using System.Collections.Generic;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using SFA.DAS.Apprenticeships.Api.Types.V3;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch.V3
{
    public interface IProviderLocationSearchProviderV3
    {
        StandardProviderSearchResult SearchStandardProviders(int standardId, Coordinate coordinates, int page, int pageSize, bool showForNonLevyOnly, bool showNationalOnly, List<DeliveryMode> deliverModes);

        //List<FrameworkProviderSearchResultsItem> SearchFrameworkProviders(int id, Coordinate coordinates, int page);
    }
}
