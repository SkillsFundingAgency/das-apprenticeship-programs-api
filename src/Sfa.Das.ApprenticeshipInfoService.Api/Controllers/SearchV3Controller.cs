using Microsoft.Web.Http;
using Sfa.Das.ApprenticeshipInfoService.Api.Attributes;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using SFA.DAS.Apprenticeships.Api.Types.V3;
using SFA.DAS.NLog.Logger;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers.V3
{
    [ApiVersion("3.0")]
    [RoutePrefix("v{version:apiVersion}")]
    public class SearchV3Controller : ApiController
    {
        private readonly IApprenticeshipSearchServiceV3 _apprenticeshipSearchServiceV3;
        private readonly IProviderNameSearchServiceV3 _providerNameSearchService;
        private readonly ILog _logger;

        public SearchV3Controller(
            IApprenticeshipSearchServiceV3 apprenticeshipSearchServiceV2,
            ILog logger, IProviderNameSearchServiceV3 providerNameSearchService)
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
        [SwaggerOperation("SearchActiveApprenticeships")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(ApprenticeshipSearchResults))]
        [Route("apprenticeship-programmes/search/")]
        [HttpGet]
        [ExceptionHandling]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult SearchApprenticeships(string keywords, int page = 1, int pageSize = 20, int order = 0, string levels = null)
        {
            try
            {
                var selectedLevels = ParseForLevels(levels);
                var response = _apprenticeshipSearchServiceV3.SearchApprenticeships(keywords, page, pageSize, order, selectedLevels);

                return Ok(response);
            }
            catch (ArgumentException)
            {
                return BadRequest();
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
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(ProviderSearchResults))]
        [Route("providers/search")]
        [HttpGet]
        [ExceptionHandling]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IHttpActionResult> SearchProviders(string keywords, int page = 1, int pageSize = 20)
        {
            try
            {


                var response = await _providerNameSearchService.SearchProviderNameAndAliases(keywords, page, pageSize);

                foreach (var providerSearchResponseItem in response.Results)
                {
                    providerSearchResponseItem.Uri = ResolveProviderUri(providerSearchResponseItem.Ukprn.ToString());
                }

                return Ok(response);
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, "/providers/search");
                return BadRequest(ex.ToString());
            }
            catch (Exception e)
            {
                _logger.Error(e, "/providers/search");
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
