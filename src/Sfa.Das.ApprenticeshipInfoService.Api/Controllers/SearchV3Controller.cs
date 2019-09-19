using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using SFA.DAS.Apprenticeships.Api.Types.V3;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers.V3
{
    [ApiExplorerSettings(GroupName = "v3")]   
    [Route("v3")]   
    public class SearchV3Controller : ControllerBase
    {
        private readonly IApprenticeshipSearchServiceV3 _apprenticeshipSearchServiceV3;
        private readonly IProviderNameSearchServiceV3 _providerNameSearchService;
        private readonly ILogger<SearchV3Controller> _logger;

        public SearchV3Controller(
            IApprenticeshipSearchServiceV3 apprenticeshipSearchServiceV2,
            ILogger<SearchV3Controller> logger, IProviderNameSearchServiceV3 providerNameSearchService)
        {
            _apprenticeshipSearchServiceV3 = apprenticeshipSearchServiceV2;
            _logger = logger;
            _providerNameSearchService = providerNameSearchService;
        }

        /// <summary>
        /// Search all apprenticeships
        /// </summary>
        /// <param name="keywords">Keywords to search for</param>
        /// <param name="page">Requested page</param>
        /// <param name="pageSize">Results per page</param>
        /// <param name="order">1 - Best match, 2 - Level (desc), 3 - Level (asc)</param>
        /// <param name="levels">Levels, coma separated</param>
        /// <returns>a search result object</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("apprenticeship-programmes/search/", Name="SearchApprenticeships")]
        public ActionResult<ApprenticeshipSearchResults> SearchApprenticeships(string keywords, int page = 1, int pageSize = 20, int order = 0, string levels = null)
        {
            try
            {
                var selectedLevels = ParseForLevels(levels);
                var response = _apprenticeshipSearchServiceV3.SearchApprenticeships(keywords, page, pageSize, order, selectedLevels);

                return response;
            }
            catch (ArgumentException)
            {
                return BadRequest();
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
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("providers/search", Name="SearchForProviders")]
        public ActionResult<ProviderSearchResults> SearchProviders(string keywords, int page = 1, int pageSize = 20)
        {
            try
            {
                var response = _providerNameSearchService.SearchProviderNameAndAliases(keywords, page, pageSize);

                foreach (var providerSearchResponseItem in response.Results)
                {
                    providerSearchResponseItem.Uri = ResolveProviderUri(providerSearchResponseItem.Ukprn.ToString());
                }

                return response;
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "/providers/search");
                
                return BadRequest(ex.ToString());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "/providers/search");
                throw;
            }
        }

        /// <summary>
        /// Search all apprenticeships
        /// </summary>
        /// <param name="searchString">String to search for</param>
        /// <returns>a search result object</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("apprenticeship-programmes/autocomplete/", Name="SearchActiveApprenticeshipsAutocomplete")]
        public ActionResult<ApprenticeshipAutocompleteSearchResults> ApprenticeshipsAutocomplete(string searchString)
        {
            try
            {
                var response = _apprenticeshipSearchServiceV3.GetCompletions(searchString);

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "/apprenticeship-programmes/autocomplete");
                throw;
            }
        }

        private List<int> ParseForLevels(string ids)
        {
            if (ids != null && !string.IsNullOrWhiteSpace(ids))
            {
                var response = new List<int>();

                foreach (var idElement in ids.Split(','))
                {
                    var id = 0;
                    var validInt = int.TryParse(idElement, out id);

                    if (validInt)
                    {
                        response.Add(id);
                    }
                    else
                    {
                        throw new ArgumentException($"{idElement} is not a valid level value");
                    }
                }

                return response;
            }

            return null;
        }

        private string ResolveProviderUri(string id)
        {
            return Url.Link("DefaultApi", new { controller = "Providers", id = id });
        }
    }
}
