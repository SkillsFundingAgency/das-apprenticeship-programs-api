using System.Collections.Generic;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;

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
        private readonly ILog _logger;

        public SearchController(IApprenticeshipSearchService apprenticeshipSearchService, ILog logger)
        {
            _apprenticeshipSearchService = apprenticeshipSearchService;
            _logger = logger;
        }

        /// <summary>
        /// Search all apprenticeships
        /// </summary>
        /// <returns>a search result object</returns>
        [SwaggerOperation("GetAllActiveStandards")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<ApprenticeshipSearchResultsItem>))]
        [Route("searchApprenticeships/{keywords}/{page}/{take}")]
        [ExceptionHandling]
        public IEnumerable<ApprenticeshipSearchResultsItem> Get(string keywords, int page, int take)
        {
            try
            {
                var response = _apprenticeshipSearchService.SearchApprenticeships(keywords, page, take);

                return response;
            }
            catch (Exception e)
            {
                _logger.Error(e, "/search");
                throw;
            }
        }
    }
}
