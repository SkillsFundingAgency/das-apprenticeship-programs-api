using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    [ApiExplorerSettings(GroupName = "v1")]
    public class ApprenticeshipProgrammesController : ControllerBase
    {
        private readonly IGetFrameworks _getFrameworks;
        private readonly IGetStandards _getStandards;
        private readonly IApprenticeshipMapping _apprenticeshipMapping;
        private readonly ILogger<ApprenticeshipProgrammesController> _logger;

        public ApprenticeshipProgrammesController(
            IGetFrameworks getFrameworks,
            IGetStandards getStandards,
            IApprenticeshipMapping apprenticeshipMapping,
            ILogger<ApprenticeshipProgrammesController> logger)
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
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("/apprenticeship-programmes", Name="GetAllApprenticeships")]
        public ActionResult<IEnumerable<ApprenticeshipSummary>> Get()
        {
            var response = GetActiveFrameworks();
            response.AddRange(GetActiveStandards());

            return response;
        }

        /// <summary>
        /// Do we have apprenticeships?
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = false)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpHead("/apprenticeship-programmes")]
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
            return Url.Link("GetFrameworkById", new { id = id });
        }

        private string ResolveStandardUri(string id)
        {
            return Url.Link("GetStandardById", new { id = id });
        }
    }
}
