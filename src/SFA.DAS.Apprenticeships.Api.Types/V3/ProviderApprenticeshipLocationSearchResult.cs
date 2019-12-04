using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types.V3
{
    public class ProviderApprenticeshipLocationSearchResult : PagedResults<ProviderSearchResultItem>
    {
        public Dictionary<string, long?> TrainingOptionsAggregation { get; set; }

        public Dictionary<string, long?> NationalProvidersAggregation { get; set; }
    }
}

