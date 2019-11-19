using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types.V4
{
    public class UniqueProviderApprenticeshipLocationSearchResult : PagedResults<UniqueProviderApprenticeshipLocationSearchResultItem>
    {
        public bool HasNationalProviders { get; set; }
    }
}
