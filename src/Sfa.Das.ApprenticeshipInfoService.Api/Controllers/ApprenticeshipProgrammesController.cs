using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Microsoft.Web.Http;
    using Sfa.Das.ApprenticeshipInfoService.Api.Attributes;
    using Sfa.Das.ApprenticeshipInfoService.Core.Services;
    using SFA.DAS.Apprenticeships.Api.Types;
    using SFA.DAS.NLog.Logger;
    using Swashbuckle.Swagger.Annotations;

    public class ApprenticeshipProgrammesController : ApiController
    {
        private readonly IGetFrameworks _getFrameworks;
        private readonly IGetStandards _getStandards;
        private readonly IApprenticeshipMapping _apprenticeshipMapping;
        private readonly ILog _logger;

        public ApprenticeshipProgrammesController(
            IGetFrameworks getFrameworks,
            IGetStandards getStandards,
            IApprenticeshipMapping apprenticeshipMapping,
            ILog logger)
        {
            _getFrameworks = getFrameworks;
            _getStandards = getStandards;
            _apprenticeshipMapping = apprenticeshipMapping;
            _logger = logger;
        }

        /// <summary>
        /// Get all the active apprenticeships
        /// </summary>
        /// <returns>a collection of apprenticeships</returns>
        [SwaggerOperation("GetAllApprenticeships")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<ApprenticeshipSummary>))]
        [Route("v{version:apiVersion}/apprenticeship-programmes")]
        [Route("apprenticeship-programmes")]
        [ExceptionHandling]
        public IEnumerable<ApprenticeshipSummary> Get()
        {
            var response = GetActiveFrameworks();
            response.AddRange(GetActiveStandards());

            return response;
        }

        /// <summary>
        /// Do we have apprenticeships?
        /// </summary>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("v{version:apiVersion}/apprenticeship-programmes")]
        [Route("apprenticeship-programmes")]
        [ExceptionHandling]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Head()
        {
            Get();
        }

        private List<ApprenticeshipSummary> GetActiveStandards()
        {
            var activeStandards = _getStandards.GetAllStandards().Where(x => x.IsActiveStandard).ToList();

            foreach (var item in activeStandards)
            {
                item.Uri = ResolveStandardUri(item.Id);
            }

            return activeStandards.Select(standardSummary => _apprenticeshipMapping.MapToApprenticeshipSummary(standardSummary)).ToList();
        }

        private List<ApprenticeshipSummary> GetActiveFrameworks()
        {
            var activeFrameworks = _getFrameworks.GetAllFrameworks().Where(x => x.IsActiveFramework).ToList();

            foreach (var item in activeFrameworks)
            {
                item.Uri = ResolveFrameworkUri(item.Id);
            }

            return activeFrameworks.Select(frameworkSummary => _apprenticeshipMapping.MapToApprenticeshipSummary(frameworkSummary)).ToList();
        }

        private string ResolveFrameworkUri(string id)
        {
            return Url.Link("DefaultApi", new { controller = "frameworks", id = id });
        }

        private string ResolveStandardUri(string id)
        {
            return Url.Link("DefaultApi", new { controller = "standards", id = id });
        }
    }
}
