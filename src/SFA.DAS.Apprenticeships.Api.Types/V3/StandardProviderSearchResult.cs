using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types.V3
{
    public class StandardProviderSearchResult
    {
        public long TotalResults { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public IEnumerable<ProviderSearchResultItem> Results { get; set; }
    }
}
