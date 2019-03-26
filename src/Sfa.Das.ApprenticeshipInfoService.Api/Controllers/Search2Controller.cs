using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Web.Http;
using Sfa.Das.ApprenticeshipInfoService.Api.Attributes;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using SFA.DAS.Apprenticeships.Api.Types.V2;
using SFA.DAS.NLog.Logger;
using Swashbuckle.Swagger.Annotations;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    [ApiVersion("3.0")]
    [RoutePrefix("v{version:apiVersion}")]
    public class Search2Controller : ApiController
    {
        private readonly IApprenticeshipSearchServiceV2 _apprenticeshipSearchServiceV2;
        private readonly ILog _logger;

        public Search2Controller(
            IApprenticeshipSearchServiceV2 apprenticeshipSearchServiceV2,
            ILog logger)
        {
            _apprenticeshipSearchServiceV2 = apprenticeshipSearchServiceV2;
            _logger = logger;
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
        public IHttpActionResult SearchApprenticeshipsV2(string keywords, int page = 1, int pageSize = 20, int order = 0, string levels = null)
        {
            try
            {
                var selectedLevels = ParseForLevels(levels);
                var response = _apprenticeshipSearchServiceV2.SearchApprenticeships(keywords, page, pageSize, order, selectedLevels);

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
    }
}
