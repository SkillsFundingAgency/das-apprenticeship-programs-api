using Nest;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Models.Responses;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch.V3;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using SFA.DAS.Apprenticeships.Api.Types.V3;

namespace Sfa.Das.Sas.Infrastructure.Elasticsearch
{
    using System.Threading.Tasks;
    using SFA.DAS.NLog.Logger;


    public class ProviderNameSearchServiceV3 : IProviderNameSearchServiceV3
    {
        private readonly ILog _logger;
        private readonly IProviderNameSearchMapping _nameSearchMapping;
        private readonly IProviderNameSearchProviderQuery _providerNameSearchProviderQuery;

        public ProviderNameSearchServiceV3(ILog logger, IProviderNameSearchMapping nameSearchMapping, IProviderNameSearchProviderQuery providerNameSearchProviderQuery)
        {

            _logger = logger;
            _nameSearchMapping = nameSearchMapping;
            _providerNameSearchProviderQuery = providerNameSearchProviderQuery;
        }

        public async Task<ProviderSearchResults> SearchProviderNameAndAliases(string searchTerm, int page, int take)
        {
            var formattedSearchTerm = QueryHelper.FormatQueryReturningEmptyStringIfEmptyOrNull(searchTerm).Trim();

            _logger.Info(
                $"Provider Name Search provider formatting query: SearchTerm: [{searchTerm}] formatted to: [{formattedSearchTerm}]");

            if (formattedSearchTerm.Length < 3)
            {
                _logger.Info(
                    $"Formatted search term causing SearchTermTooShort: [{formattedSearchTerm}]");

               // return MapProviderNameSearchResultsAndPaginationTooShortDetails(formattedSearchTerm);
            }

            var term = $"*{formattedSearchTerm}*";
            var totalHits = _providerNameSearchProviderQuery.GetTotalMatches(term);


            var returnedResults = _providerNameSearchProviderQuery.GetResults(term, take, (page - 1) * take);

            var resultsMappedAndPaginated = MapResultsAndPaginationDetails(page, take, formattedSearchTerm, returnedResults, totalHits);

            return resultsMappedAndPaginated;
        }

        //private static ProviderSearchResults MapProviderNameSearchResultsAndPaginationTooShortDetails(string formattedSearchTerm)
        //{
        //    return new ProviderSearchResults
        //    {
        //        PageNumber = 1,
        //        };
        //}

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