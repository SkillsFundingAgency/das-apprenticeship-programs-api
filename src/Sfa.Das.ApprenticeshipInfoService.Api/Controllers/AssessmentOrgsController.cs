namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Sfa.Das.ApprenticeshipInfoService.Api.Attributes;
    using Sfa.Das.ApprenticeshipInfoService.Api.Helpers;
    using Sfa.Das.ApprenticeshipInfoService.Core.Services;
    using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;
    using SFA.DAS.NLog.Logger;
    using Swashbuckle.Swagger.Annotations;

    public class AssessmentOrgsController : ApiController
    {
        private readonly IGetAssessmentOrgs _getAssessmentOrgs;
        private readonly ILog _logger;

        public AssessmentOrgsController(
            IGetAssessmentOrgs getAssessmentOrgs,
            ILog logger)
        {
            _getAssessmentOrgs = getAssessmentOrgs;
            _logger = logger;
        }

        /// <summary>
        /// Get all the assessment organisations
        /// </summary>
        /// <returns>colllection of assessment organisation summaries</returns>
        [SwaggerOperation("GetAll")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<OrganisationSummary>))]
        [Route("assessment-organisations")]
        [ExceptionHandling]
        public IEnumerable<OrganisationSummary> Get()
        {
            try
            {
                var response = _getAssessmentOrgs.GetAllOrganisations().ToList();

                foreach (var organisation in response)
                {
                    organisation.Uri = Resolve(organisation.Id);
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
        /// Get an assessment organisation
        /// </summary>
        /// <param name="id">EPA00001</param>
        /// <returns>an organisation</returns>
        [SwaggerOperation("GetOrganisation")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(Organisation))]
        [Route("assessment-organisations/{id}")]
        [ExceptionHandling]
        public Organisation Get(string id)
        {
            var response = _getAssessmentOrgs.GetOrganisationById(id);

            if (response == null)
            {
                throw HttpResponseFactory.RaiseException(HttpStatusCode.NotFound,
                    $"No organisation with EpaOrganisationIdentifier {id} found");
            }

            response.Uri = Resolve(response.Id);

            return response;
        }

        /// <summary>
        /// Do we have assessment organisations?
        /// </summary>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("assessment-organisations")]
        [ExceptionHandling]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Head()
        {
            Get();
        }

        /// <summary>
        /// Assessment organisation exists?
        /// </summary>
        /// <param name="id">EPA00001</param>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("assessment-organisations/{id}")]
        [ExceptionHandling]
        public void Head(string id)
        {
            Get(id);
        }

        /// <summary>
        /// Get assessment organisations by a standard
        /// </summary>
        /// <param name="id">standard code</param>
        /// <returns>a collection of organisations</returns>
        [SwaggerOperation("GetByStandard")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<Organisation>))]
        [Route("assessment-organisations/standards/{id}")]
        [ExceptionHandling]
        public IEnumerable<Organisation> GetByStandardId(string id)
        {
            var response = _getAssessmentOrgs.GetOrganisationsByStandardId(id);

            if (response == null)
            {
                throw HttpResponseFactory.RaiseException(
                    HttpStatusCode.NotFound,
                    $"No organisation with EpaOrganisationIdentifier {id} found");
            }

            response = response.ToList();
            foreach (var organisation in response)
            {
                organisation.Uri = Resolve(organisation.Id);
            }

            return response;
        }

        /// <summary>
        /// Get standards by assessment organisation
        /// </summary>
        /// <returns>colllection of standards by specific organisation identifier</returns>
        [SwaggerOperation("GetStandardsByOrganisationId")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<StandardOrganisationSummary>))]
        [Route("assessment-organisations/{organisationId}/standards")]
        [ExceptionHandling]
        public IEnumerable<StandardOrganisationSummary> GetStandardsByOrganisationId(string organisationId)
        {
            try
            {
                var response = _getAssessmentOrgs.GetStandardsByOrganisationIdentifier(organisationId).ToList();

                foreach (var standardOrganisation in response)
                {
                    standardOrganisation.Uri = ResolveStandardUri(standardOrganisation.StandardCode);
                }

                return response;
            }
            catch (Exception e)
            {
                _logger.Error(e, $"/assessment-organisations/{organisationId}/standars");
                throw;
            }
        }

        private string Resolve(string organisationId)
        {
            return Url.Link("DefaultApi", new { controller = "assessmentorgs", id = organisationId }).Replace("assessmentorgs", "assessment-organisations");
        }

        private string ResolveStandardUri(string standardCode)
        {
            return Url.Link("DefaultApi", new { controller = "Standards", id = standardCode });
        }
    }
}