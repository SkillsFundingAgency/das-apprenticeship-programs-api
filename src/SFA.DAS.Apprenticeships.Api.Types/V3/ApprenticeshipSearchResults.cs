using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types.V3
{
    public sealed class ApprenticeshipSearchResults : PagedResults<ApprenticeshipSearchResultsItem>
    {
        public Dictionary<int, long?> LevelAggregation { get; set; }
    }
}
