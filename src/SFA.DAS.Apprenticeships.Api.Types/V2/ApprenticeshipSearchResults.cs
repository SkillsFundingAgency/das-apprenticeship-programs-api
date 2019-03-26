using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types.V2
{
    public sealed class ApprenticeshipSearchResults
    {
        public long TotalResults { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public IEnumerable<ApprenticeshipSearchResultsItem> Results { get; set; }

        public Dictionary<int, long?> LevelAggregation { get; set; }
    }
}
