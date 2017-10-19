namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Sfa.Das.ApprenticeshipInfoService.Api.Attributes;
    using Sfa.Das.ApprenticeshipInfoService.Core.Services;
    using SFA.DAS.Apprenticeships.Api.Types;
    using SFA.DAS.NLog.Logger;
    using Swashbuckle.Swagger.Annotations;

    public class StandardsController : ApiController
    {
        private readonly IGetStandards _getStandards;
        private readonly ILog _logger;

        public StandardsController(IGetStandards getStandards, ILog logger)
        {
            _getStandards = getStandards;
            _logger = logger;
        }

        /// <summary>
        /// Get all the active standards
        /// </summary>
        /// <returns>a collection of standards</returns>
        [SwaggerOperation("GetAllStandards")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<StandardSummary>))]
        [Route("standards")]
        [ExceptionHandling]
        public IEnumerable<StandardSummary> Get()
        {
            try
            {
                var response = _getStandards.GetAllStandards().ToList();

                foreach (var item in response)
                {
                    item.Uri = Resolve(item.Id);
                }

                return response;
            }
            catch (Exception e)
            {
                _logger.Error(e, "/assessment-organisations");
                throw;
            }
        }

        /// <summary>
        /// Get a standard
        /// </summary>
        /// <param name="id">{standardid}</param>
        /// <returns>a standard</returns>
        [SwaggerOperation("GetStandardById")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(Standard))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("standards/{id}")]
        [ExceptionHandling]
        public Standard Get(string id)
        {
            var standard = _getStandards.GetStandardById(id);

            if (standard == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            standard.Uri = Resolve(standard.StandardId);
            standard.Providers = ResolveProvidersUrl(id);
            return standard;
        }

        /// <summary>
        /// Do we have standards?
        /// </summary>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("standards")]
        [ExceptionHandling]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Head()
        {
            Get();
        }

        /// <summary>
        /// Standard exists?
        /// </summary>
        /// <param name="id">{standardid}</param>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("standards/{id}")]
        [ExceptionHandling]
        public void Head(string id)
        {
            Get(id);
        }

        private string Resolve(string id)
        {
            return Url.Link("DefaultApi", new { controller = "standards", id = id });
        }

        private ProvidersHref ResolveProvidersUrl(string id)
        {
            return new ProvidersHref
            {
                Href = Url.Link("GetStandardProviders", new { standardId = id })
            };
        }
    }
}
