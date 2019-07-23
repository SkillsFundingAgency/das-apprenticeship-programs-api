using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types.V3
{
    public class ApprenticeshipAutocompleteSearchResults
    {
        public IEnumerable<ApprenticeshipAutocompleteSearchResultsItem> Results { get; set; } = new List<ApprenticeshipAutocompleteSearchResultsItem>();
    }
}
