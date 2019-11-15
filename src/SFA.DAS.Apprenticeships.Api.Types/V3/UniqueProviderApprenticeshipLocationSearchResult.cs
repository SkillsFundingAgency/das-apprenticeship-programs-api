using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types.V3
{
    public class UniqueProviderApprenticeshipLocationSearchResult : PagedResults<ProviderSearchResultItem>
    {
        public Dictionary<string, long?> NationalProvidersAggregation { get; set; }
        public int LocationCount { get; set; }

    }
}
