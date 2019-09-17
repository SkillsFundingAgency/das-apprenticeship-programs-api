using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    [ApiExplorerSettings(GroupName = "v1")]
    public class AssessmentOrgsController : ControllerBase
    {
        private readonly IGetAssessmentOrgs _getAssessmentOrgs;
        private readonly ILogger<AssessmentOrgsController> _logger;

        public AssessmentOrgsController(
            IGetAssessmentOrgs getAssessmentOrgs,
            ILogger<AssessmentOrgsController> logger)
        {
            _getAssessmentOrgs = getAssessmentOrgs;
            _logger = logger;
        }

        /// <summary>
        /// Get all the assessment organisations
        /// </summary>
        /// <returns>colllection of assessment organisation summaries</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("/assessment-organisations", Name="GetAllAssessmentOrgs")]
        public ActionResult<IEnumerable<OrganisationSummary>> Get()
        {
            try
            {
                var response = _getAssessmentOrgs.GetAllOrganisations().ToList();

                foreach (var organisation in response)
                {
                    organisation.Uri = Resolve(organisation.Id);
                    organisation.Links = ResolveLinks(organisation.Id);
                }

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "/assessment-organisations");
                throw;
            }
        }

        /// <summary>
        /// Get an assessment organisation
        /// </summary>
        /// <param name="id">EPA00001</param>
        /// <returns>an organisation</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("/assessment-organisations/{id}", Name="GetAssessmentOrgById")]
        public ActionResult<Organisation> Get(string id)
        {
            var response = _getAssessmentOrgs.GetOrganisationById(id);

            if (response == null)
            {
                return NotFound($"No organisation with EpaOrganisationIdentifier {id} found");
            }

            response.Uri = Resolve(response.Id);
            response.Links = ResolveLinks(response.Id);

            return response;
        }

        /// <summary>
        /// Do we have assessment organisations?
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = false)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpHead("/assessment-organisations")]
        public void Head()
        {
            Get();
        }

        /// <summary>
        /// Assessment organisation exists?
        /// </summary>
        /// <param name="id">EPA00001</param>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpHead("/assessment-organisations/{id}")]
        public void Head(string id)
        {
            Get(id);
        }

        /// <summary>
        /// Get assessment organisations by a standard
        /// </summary>
        /// <param name="id">standard code</param>
        /// <returns>a collection of organisations</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("/assessment-organisations/standards/{id}", Name="GetAssessmentOrgByStandardId")]
        public ActionResult<IEnumerable<Organisation>> GetByStandardId(string id)
        {
            var response = _getAssessmentOrgs.GetOrganisationsByStandardId(id);

            if (response == null)
            {
                return NotFound($"No organisation found for Standard {id}");
            }

            var result = response.ToList();
            foreach (var organisation in result)
            {
                organisation.Uri = Resolve(organisation.Id);
                organisation.Links = ResolveLinks(organisation.Id);
            }

            return result;
        }

        /// <summary>
        /// Get standards by assessment organisation
        /// </summary>
        /// <param name="organisationId">Assessment Organisation Id</param>
        /// <returns>colllection of standards by specific organisation identifier</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("/assessment-organisations/{organisationId}/standards", Name="GetStandardsByAssessmentOrgId")]
        public ActionResult<IEnumerable<StandardOrganisationSummary>> GetStandardsByOrganisationId(string organisationId)
        {
            var response = _getAssessmentOrgs.GetStandardsByOrganisationIdentifier(organisationId).ToList();

            foreach (var standardOrganisation in response)
            {
                standardOrganisation.Uri = ResolveStandardUri(standardOrganisation.StandardCode);
            }

            return response;
        }

        private string Resolve(string organisationId)
        {
            return Url.Link("GetAssessmentOrgById", new { id = organisationId });
        }

        private string ResolveStandardUri(string standardCode)
        {
            return Url.Link("GetStandardById", new { id = standardCode });
        }

        private List<Link> ResolveLinks(string organisationId)
        {
            return new List<Link>
            {
                new Link
                {
                    Title = "Standards",
                    Href = Url.Link("GetStandardsByAssessmentOrgId", new { organisationId = organisationId })
                }
            };
        }
    }
}