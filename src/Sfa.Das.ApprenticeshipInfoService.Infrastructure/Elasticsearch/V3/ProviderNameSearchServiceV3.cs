using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nest;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch.Querys;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
using SFA.DAS.Apprenticeships.Api.Types.V3;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch.V3
{
    public class ProviderNameSearchServiceV3 : IProviderNameSearchServiceV3
    {
        private readonly ILogger<ProviderNameSearchServiceV3> _logger;
        private readonly IProviderNameSearchMapping _nameSearchMapping;
        private readonly IProviderNameSearchProviderQuery _providerNameSearchProviderQuery;

        public ProviderNameSearchServiceV3(ILogger<ProviderNameSearchServiceV3> logger, IProviderNameSearchMapping nameSearchMapping, IProviderNameSearchProviderQuery providerNameSearchProviderQuery)
        {

            _logger = logger;
            _nameSearchMapping = nameSearchMapping;
            _providerNameSearchProviderQuery = providerNameSearchProviderQuery;
        }

        public ProviderSearchResults SearchProviderNameAndAliases(string searchTerm, int page, int take)
        {
            var formattedSearchTerm = Core.Helpers.QueryHelper.FormatQueryReturningEmptyStringIfEmptyOrNull(searchTerm).Trim();

            _logger.LogInformation(
                $"Provider Name Search provider formatting query: SearchTerm: [{searchTerm}] formatted to: [{formattedSearchTerm}]");

            if (formattedSearchTerm.Length < 3)
            {
                _logger.LogInformation(
                    $"Formatted search term causing SearchTermTooShort: [{formattedSearchTerm}]");

              throw new ValidationException($"The search term must be at least 3 characters long, entered search term: {formattedSearchTerm}");
            }

            var term = $"*{formattedSearchTerm}*";
            var totalHits = _providerNameSearchProviderQuery.GetTotalMatches(term);


            var returnedResults = _providerNameSearchProviderQuery.GetResults(term, take, (page - 1) * take);

            var resultsMappedAndPaginated = MapResultsAndPaginationDetails(page, take, formattedSearchTerm, returnedResults, totalHits);

            return resultsMappedAndPaginated;
        }

        private ProviderSearchResults MapResultsAndPaginationDetails(int page, int take, string formattedSearchTerm, ISearchResponse<ProviderNameSearchResult> returnedResults, long totalHits)
        {
            return new ProviderSearchResults
            {
                PageNumber = page,
                PageSize = take,
                Results = _nameSearchMapping.FilterNonMatchingAliases(formattedSearchTerm, returnedResults.Documents),
                TotalResults = totalHits,
               };
        }
    }
}