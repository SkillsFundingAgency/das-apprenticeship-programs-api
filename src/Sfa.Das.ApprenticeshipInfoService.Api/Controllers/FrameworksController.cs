namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
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

    public class FrameworksController : ApiController
    {
        private readonly IGetFrameworks _getFrameworks;
        private readonly ILog _logger;

        public FrameworksController(
            IGetFrameworks getFrameworks,
            ILog logger)
        {
            _getFrameworks = getFrameworks;
            _logger = logger;
        }

        /// <summary>
        /// Get all the active frameworks
        /// </summary>
        /// <returns>a collection of frameworks</returns>
        [SwaggerOperation("GetAllFrameworks")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<FrameworkSummary>))]
        [Route("frameworks")]
        [ExceptionHandling]
        public IEnumerable<FrameworkSummary> Get()
        {
            var response = _getFrameworks.GetAllFrameworks().Where(x => x.IsActiveFramework).ToList();

            foreach (var item in response)
            {
                item.Uri = Resolve(item.Id);
            }

            return response;
        }

        /// <summary>
        /// Get a framework
        /// </summary>
        /// <param name="id">{FrameworkCode}-{ProgType}-{PathwayId}</param>
        /// <returns>a framework</returns>
        [SwaggerOperation("GetFrameworkById")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(Framework))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("frameworks/{id}")]
        [ExceptionHandling]
        public Framework Get(string id)
        {
            var response = _getFrameworks.GetFrameworkById(id);

            if (response == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            response.Uri = Resolve(response.FrameworkId);
            response.Providers = ResolveProvidersUrl(id);

            return response;
        }

        /// <summary>
        /// Do we have frameworks?
        /// </summary>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("frameworks")]
        [ExceptionHandling]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Head()
        {
            Get();
        }

        /// <summary>
        /// framework exists?
        /// </summary>
        /// <param name="id">{FrameworkCode}-{ProgType}-{PathwayId}</param>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("frameworks/{id}")]
        [ExceptionHandling]
        public void Head(string id)
        {
            Get(id);
        }

        /// <summary>
        /// Get all the active frameworks
        /// </summary>
        /// <returns>a collection of framework codes</returns>
        [SwaggerOperation("GetAllFrameworkCode")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<FrameworkCodeSummary>))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("frameworks/codes")]
        [ExceptionHandling]
        public IEnumerable<FrameworkCodeSummary> GetAllFrameworkCodes()
        {
            var response = _getFrameworks.GetAllFrameworkCodes();

            if (response == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            foreach (var item in response)
            {
                item.Uri = ResolveFrameworkCodeSummary(item.FrameworkCode);
            }

            return response;
        }

        /// <summary>
        /// Get a framework
        /// </summary>
        /// <param name="frameworkCode"></param>
        /// <returns>a framework resume</returns>
        [SwaggerOperation("GetByFrameworkCode")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(Framework))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("frameworks/codes/{frameworkCode}", Name = "GetByFrameworkCode")]
        [ExceptionHandling]
        public FrameworkCodeSummary GetByFrameworkCode(int frameworkCode)
        {
            var response = _getFrameworks.GetFrameworkByCode(frameworkCode);

            if (response == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            response.Uri = ResolveFrameworkCodeSummary(response.FrameworkCode);

            return response;
        }

        /// <summary>
        /// Do we have frameworks?
        /// </summary>
        /// /// <param name="frameworkCode">Framework code</param>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("frameworks/codes/{frameworkCode}")]
        [ExceptionHandling]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Head(int frameworkCode)
        {
            GetByFrameworkCode(frameworkCode);
        }

        private string Resolve(string id)
        {
            return Url.Link("DefaultApi", new { controller = "frameworks", id = id });
        }

        private string ResolveFrameworkCodeSummary(int responseFrameworkCode)
        {
            return Url.Link("GetByFrameworkCode", new { frameworkCode = responseFrameworkCode });
        }

        private ProvidersHref ResolveProvidersUrl(string id)
        {
            return new ProvidersHref
            {
                Href = Url.Link("GetFrameworkProviders", new { frameworkId = id })
            };
        }
    }
}
