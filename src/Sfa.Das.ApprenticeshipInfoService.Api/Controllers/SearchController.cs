using System.Collections.Generic;
using System.Linq;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.Providers;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    using System;
    using System.Net;
    using System.Web.Http;
    using Sfa.Das.ApprenticeshipInfoService.Api.Attributes;
    using Sfa.Das.ApprenticeshipInfoService.Core.Services;
    using SFA.DAS.NLog.Logger;
    using Swashbuckle.Swagger.Annotations;

    public class SearchController : ApiController
    {
        private readonly IApprenticeshipSearchService _apprenticeshipSearchService;
        private readonly IProviderSearchService _providerSearchService;
        private readonly IProviderMapping _providerMapping;
        private readonly ILog _logger;

        public SearchController(IApprenticeshipSearchService apprenticeshipSearchService, IProviderSearchService providerSearchService, IProviderMapping providerMapping, ILog logger)
        {
            _apprenticeshipSearchService = apprenticeshipSearchService;
            _providerSearchService = providerSearchService;
            _providerMapping = providerMapping;
            _logger = logger;
        }

        /// <summary>
        /// Search all apprenticeships
        /// </summary>
        /// <returns>a search result object</returns>
        [SwaggerOperation("SearchActiveApprenticeships")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<ApprenticeshipSearchResultsItem>))]
        [Route("apprenticeship-programmes/search")]
		[HttpGet]
        [ExceptionHandling]
        public IEnumerable<ApprenticeshipSearchResultsItem> SearchApprenticeships(string keywords, int page = 1)
        {
            try
            {
                var response = _apprenticeshipSearchService.SearchApprenticeships(keywords, page);

                return response;
            }
            catch (Exception e)
            {
                _logger.Error(e, "/apprenticeship-programmes/search");
                throw;
            }
        }

        /// <summary>
        /// Search for providers
        /// </summary>
        /// <returns>a search result object</returns>
        [SwaggerOperation("SearchProviders")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<ProviderSearchResponseItem>))]
        [Route("providers/search")]
		[HttpGet]
        [ExceptionHandling]
        public IEnumerable<ProviderSearchResponseItem> SearchProviders(string keywords, int page = 1)
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
                _logger.Error(e, "/providers/search");
                throw;
            }
        }

        private string ResolveProviderUri(string id)
        {
            return Url.Link("DefaultApi", new { controller = "Providers", id = id });
        }
    }
}
