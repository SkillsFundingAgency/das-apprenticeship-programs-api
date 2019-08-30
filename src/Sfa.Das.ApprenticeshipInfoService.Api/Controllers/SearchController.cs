using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.Providers;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    [ApiExplorerSettings(GroupName = "v1")]   
    public class SearchController : ControllerBase
    {
        private readonly IApprenticeshipSearchServiceV1 _apprenticeshipSearchServiceV1;
        private readonly IProviderSearchService _providerSearchService;
        private readonly IProviderMapping _providerMapping;
        private readonly ILogger<SearchController> _logger;

        public SearchController(
            IApprenticeshipSearchServiceV1 apprenticeshipSearchServiceV1,
            IProviderSearchService providerSearchService,
            IProviderMapping providerMapping,
            ILogger<SearchController> logger)
        {
            _apprenticeshipSearchServiceV1 = apprenticeshipSearchServiceV1;
            _providerSearchService = providerSearchService;
            _providerMapping = providerMapping;
            _logger = logger;
        }

        /// <summary>
        /// Search all apprenticeships
        /// </summary>
        /// <returns>a search result object</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("apprenticeship-programmes/search", Name="SearchActiveApprenticeships")]
        public ActionResult<IEnumerable<ApprenticeshipSearchResultsItem>> SearchApprenticeships(string keywords, int page = 1)
        {
            try
            {
                var response = _apprenticeshipSearchServiceV1.SearchApprenticeships(keywords, page);

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "/apprenticeship-programmes/search");
                throw;
            }
        }

        /// <summary>
        /// Search for providers
        /// </summary>
        /// <returns>a search result object</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("providers/search", Name="SearchProviders")]
        public ActionResult<IEnumerable<ProviderSearchResponseItem>> SearchProviders(string keywords, int page = 1)
        {
            try
            {
                var providerSearchResults = _providerSearchService.SearchProviders(keywords, page);
                var response = providerSearchResults.Select(providerSearchResultsItem => _providerMapping.MapToProviderSearchItem(providerSearchResultsItem)).ToList();

                foreach (var providerSearchResponseItem in response)
                {
                    providerSearchResponseItem.Uri = ResolveProviderUri(providerSearchResponseItem.Ukprn);
                }

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "/providers/search");
                throw;
            }
        }

        private string ResolveProviderUri(string id)
        {
            return Url.Link("DefaultApi", new { controller = "Providers", id = id });
        }
    }
}
