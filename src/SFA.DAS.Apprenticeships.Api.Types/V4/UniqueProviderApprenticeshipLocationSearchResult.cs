using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types.V4
{
    public class UniqueProviderApprenticeshipLocationSearchResult : PagedResults<UniqueProviderApprenticeshipLocationSearchResultItem>
    {
        public Dictionary<string, long?> NationalProvidersAggregation { get; set; }
    }
}
