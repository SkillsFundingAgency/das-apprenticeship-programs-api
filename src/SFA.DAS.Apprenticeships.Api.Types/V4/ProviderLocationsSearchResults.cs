namespace SFA.DAS.Apprenticeships.Api.Types.V4
{
    public class ProviderLocationsSearchResults : PagedResults<ProviderLocationsSearchResultsItem>
    {
        public string ProviderName { get; set; }
    }
}