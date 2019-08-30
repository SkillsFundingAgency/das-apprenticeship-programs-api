using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    [ApiExplorerSettings(GroupName = "v1")]
    public class FrameworksController : ControllerBase
    {
        private readonly IGetFrameworks _getFrameworks;
        private readonly ILogger<FrameworksController> _logger;

        public FrameworksController(
            IGetFrameworks getFrameworks,
            ILogger<FrameworksController> logger)
        {
            _getFrameworks = getFrameworks;
            _logger = logger;
        }

        /// <summary>
        /// Get all the active frameworks
        /// </summary>
        /// <returns>a collection of frameworks</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("/frameworks", Name="GetAllActiveFrameworks")]
        public ActionResult<IEnumerable<FrameworkSummary>> Get()
        {
            var response = _getFrameworks.GetAllFrameworks().Where(x => x.IsActiveFramework).ToList();

            foreach (var item in response)
            {
                item.Uri = Resolve(item.Id);
            }

            return response;
        }

        /// <summary>
        /// Get all frameworks
        /// </summary>
        /// <returns>a collection of frameworks</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("/frameworks/v2", Name="GetAllFrameworks")]
        public ActionResult<IEnumerable<FrameworkSummary>> GetAll()
        {
            var response = _getFrameworks.GetAllFrameworks().ToList();

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
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("/frameworks/{id}", Name="GetFrameworkById")]
        public ActionResult<Framework> Get(string id)
        {
            var response = _getFrameworks.GetFrameworkById(id);

            if (response == null)
            {
                return NotFound();
            }

            response.Uri = Resolve(response.FrameworkId);
            response.Providers = ResolveProvidersUrl(id);

            return response;
        }

        /// <summary>
        /// Do we have frameworks?
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpHead("/frameworks")]
        public void Head()
        {
            Get();
        }

        /// <summary>
        /// framework exists?
        /// </summary>
        /// <param name="id">{FrameworkCode}-{ProgType}-{PathwayId}</param>
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpHead("/frameworks/{id}")]
        public void Head(string id)
        {
            Get(id);
        }

        /// <summary>
        /// Get all the active frameworks
        /// </summary>
        /// <returns>a collection of framework codes</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("/frameworks/codes", Name="GetAllFrameworkCode")]
        public ActionResult<IEnumerable<FrameworkCodeSummary>> GetAllFrameworkCodes()
        {
            var response = _getFrameworks.GetAllFrameworkCodes();

            if (response == null)
            {
                return NotFound();
            }

            foreach (var item in response)
            {
                item.Uri = ResolveFrameworkCodeSummary(item.FrameworkCode);
            }

            return response.ToList();
        }

        /// <summary>
        /// Get a framework
        /// </summary>
        /// <param name="frameworkCode"></param>
        /// <returns>a framework resume</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("/frameworks/codes/{frameworkCode}", Name="GetByFrameworkCode")]
        public ActionResult<FrameworkCodeSummary> GetByFrameworkCode(int frameworkCode)
        {
            var response = _getFrameworks.GetFrameworkByCode(frameworkCode);

            if (response == null)
            {
                return NotFound();
            }

            response.Uri = ResolveFrameworkCodeSummary(response.FrameworkCode);

            return response;
        }

        /// <summary>
        /// Do we have frameworks?
        /// </summary>
        /// /// <param name="frameworkCode">Framework code</param>
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpHead("/frameworks/codes/{frameworkCode}")]
        public void Head(int frameworkCode)
        {
            GetByFrameworkCode(frameworkCode);
        }

        private string Resolve(string id)
        {
            return Url.Link("GetFrameworkById", new { id = id });
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
