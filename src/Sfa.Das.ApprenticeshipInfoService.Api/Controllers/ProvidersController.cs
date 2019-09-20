using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Sfa.Das.ApprenticeshipInfoService.Api.Conventions;
using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Models.Responses;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using SFA.DAS.Apprenticeships.Api.Types.Providers;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    [ApiExplorerSettings(GroupName = "v1")]
    public class ProvidersController : ControllerBase
    {
        private const string BadUkprnMessage = "A valid UKPRN as defined in the UK Register of Learning Providers (UKRLP) is 8 digits in the format 10000000 - 99999999";

        private readonly IGetProviders _getProviders;
        private readonly IControllerHelper _controllerHelper;
        private readonly IGetStandards _getStandards;
        private readonly IGetFrameworks _getFrameworks;
        private readonly IApprenticeshipProviderRepository _apprenticeshipProviderRepository;

        public ProvidersController(
            IGetProviders getProviders,
            IControllerHelper controllerHelper,
            IGetStandards getStandards,
            IGetFrameworks getFrameworks,
            IApprenticeshipProviderRepository apprenticeshipProviderRepository
            )
        {
            _getProviders = getProviders;
            _controllerHelper = controllerHelper;
            _getStandards = getStandards;
            _getFrameworks = getFrameworks;
            _apprenticeshipProviderRepository = apprenticeshipProviderRepository;
        }

        /// <summary>
        /// Get all the active providers
        /// </summary>
        /// <returns>a collection of providers</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("/providers", Name="GetAllProviders")]
        public ActionResult<IEnumerable<ProviderSummary>> Get()
        {
            var response = _getProviders.GetAllProviders();

            foreach (var provider in response)
            {
                provider.Uri = Resolve(provider.Ukprn);
            }

            return response.ToList();
        }

        /// <summary>
        /// Get a provider
        /// </summary>
        /// <param name="ukprn">UKPRN</param>
        /// <returns>A Provider</returns>
        /// <response code="400">A valid UKPRN as defined in the UK Register of Learning Providers (UKRLP) is 8 digits in the format 10000000 - 99999999</response>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpGet("/providers/{ukprn:long}", Name="GetProviderByUkprn")]
        public ActionResult<Provider> Get(long ukprn)
        {
            if (ukprn.ToString().Length != 8)
            {
                return BadRequest(BadUkprnMessage);
            }

            var response = _getProviders.GetProviderByUkprn(ukprn);

            if (response == null)
            {
                return NotFound($"No provider with Ukprn {ukprn} found");
            }

            response.Uri = Resolve(response.Ukprn);

            return response;
        }

        /// <summary>
        /// Do we have providers?
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpHead("/providers")]
        public void Head()
        {
            Get();
        }

        /// <summary>
        /// Provider exists?
        /// </summary>
        /// <param name="ukprn">UKPRN</param>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpHead("/providers/{ukprn:long}")]
        public void Head(long ukprn)
        {
            Get(ukprn);
        }

        /// <summary>
        /// Get list of active apprenticeships for a given provider
        /// </summary>
        /// <param name="ukprn">unique id</param>
        /// <returns>A list of active apprenticeships sorted by name alphabetically, then type, then level</returns>
        /// <response code="400">A valid UKPRN as defined in the UK Register of Learning Providers (UKRLP) is 8 digits in the format 10000000 - 99999999</response>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpGet("/providers/{ukprn:long}/active-apprenticeship-training", Name="GetActiveApprenticeshipsByProvider")]
        public ActionResult<ApprenticeshipTrainingSummary> GetActiveApprenticeshipTrainingByProvider(long ukprn)
        {
            return GetActiveApprenticeshipTrainingByProvider(ukprn, 1);
        }

        /// <summary>
        /// Get list of active apprenticeships for a given provider
        /// </summary>
        /// <param name="ukprn">unique id</param>
        /// <param name="page">number of page for which results are returned (default 1)</param>
        /// <returns>A list of active apprenticeships sorted by name alphabetically, then type, then level</returns>
        /// <response code="400">A valid UKPRN as defined in the UK Register of Learning Providers (UKRLP) is 8 digits in the format 10000000 - 99999999</response>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpGet("/providers/{ukprn:long}/active-apprenticeship-training/{page}", Name="GetActiveApprenticeshipsByProviderByPage")]
        public ActionResult<ApprenticeshipTrainingSummary> GetActiveApprenticeshipTrainingByProvider(long ukprn, int page)
        {
            if (ukprn.ToString().Length != 8)
            {
                return BadRequest(BadUkprnMessage);
            }

            return _getProviders.GetActiveApprenticeshipTrainingByProvider(ukprn, page);
        }

        /// <summary>
        /// Get a list of providers for an specific standard
        /// </summary>
        /// <param name="apprenticeshipId">Standard id</param>
        /// <returns>A list of Providers</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet("/providers/standard/{standardId}", Name="GetStandardProviders")]
        public ActionResult<IEnumerable<Provider>> GetStandardProviders(string standardId)
        {
            if (_getStandards.GetStandardById(standardId) == null)
            {
                return NotFound($"The standard {standardId} is not found");
            }

            var response = _getProviders.GetProvidersByStandardId(standardId);

            var providersList = response.GroupBy(x => x.Ukprn).Select(x => x.First());

            var ukprns = providersList.Select(item => item.Ukprn).Select(dummy => (long) dummy).ToList();

            return _getProviders.GetProviderByUkprnList(ukprns).ToList();
        }

        /// <summary>
        /// Get a list of providers for an specific framework
        /// </summary>
        /// <param name="frameworkId">Framework id</param>
        /// <returns>A list of Providers</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet("/providers/framework/{frameworkId}", Name="GetFrameworkProviders")]
        public ActionResult<IEnumerable<Provider>> GetFrameworkProviders(string frameworkId)
        {
            if (_getFrameworks.GetFrameworkById(frameworkId) == null)
            {
                return NotFound($"The framework {frameworkId} is not found, it should be in the format {{framework code}}-{{program type}}-{{pathway code}}");
            }

            var response = _getProviders.GetProvidersByFrameworkId(frameworkId);

            var providersList = response.GroupBy(x => x.Ukprn).Select(x => x.First());

            var ukprns = providersList.Select(item => item.Ukprn).Select(dummy => (long)dummy).ToList();

            return _getProviders.GetProviderByUkprnList(ukprns).ToList();
        }

        // /// <summary>
        // /// Get a list of providers locations for an specific standard
        // /// TODO update url
        // /// </summary>
        // /// <param name="apprenticeshipId">Standard id</param>
        // /// <returns>A list of Providers</returns>
        //[SwaggerOperation("GetProviderLocationByStandardId")]
        //[SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<StandardProviderSearchResultsItem>))]
        //[SwaggerResponse(HttpStatusCode.NotFound)]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[Route("providers/standardLocation/{standardId}", Name = "GetStandardProviderLocations")]
        //[ExceptionHandling]
        //public IEnumerable<StandardProviderSearchResultsItem> GetStandardProviderLocations(string standardId)
        //{
        //    var response = _getProviders.GetProvidersByStandardId(standardId);

        //    if (response == null || !response.Any())
        //    {
        //        throw HttpResponseFactory.RaiseException(
        //            HttpStatusCode.NotFound,
        //            $"No providers found by standard with id {standardId}");
        //    }

        //    return response;
        //}

        /// <summary>
        /// Get a list of providers locations for an specific framework
        /// TODO update url
        /// </summary>
        /// <param name="apprenticeshipId">Framework id</param>
        /// <returns>A list of Providers</returns>
        //[SwaggerOperation("GetProviderLocationByFrameworkId")]
        //[SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<FrameworkProviderSearchResultsItem>))]
        //[SwaggerResponse(HttpStatusCode.NotFound)]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[Route("providers/frameworkLocation/{frameworkId}", Name = "GetFrameworkProviderLocations")]
        //[ExceptionHandling]
        //public IEnumerable<FrameworkProviderSearchResultsItem> GetFrameworkProviderLocations(string frameworkId)
        //{
        //    var response = _getProviders.GetProvidersByFrameworkId(frameworkId);

        //    if (response == null || !response.Any())
        //    {
        //        throw HttpResponseFactory.RaiseException(
        //            HttpStatusCode.NotFound,
        //            $"No providers found by framework with id {frameworkId}");
        //    }

        //    return response;
        //}

        // GET standards/5/providers?lat=<latitude>&long=<longitude>&page=#
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpGet("/standards/{id}/providers", Name="GetByStandardIdAndLocation")]
        public ActionResult<List<StandardProviderSearchResultsItemResponse>> GetByStandardIdAndLocation(int id, [RequiredFromQuery]double? lat = null,
            [RequiredFromQuery]double? lon = null, int page = 1)
        {
            // TODO 404 if standard doesn't exists
            var actualPage = _controllerHelper.GetActualPage(page);

            if (lat.HasValue && lon.HasValue)
            {
                return _getProviders.GetByStandardIdAndLocation(id, lat.Value, lon.Value, actualPage);
            }

            return BadRequest("A valid Latitude and Longitude is required");
        }

        // GET frameworks/5/providers?lat=<latitude>&long=<longitude>&page=#
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpGet("/frameworks/{id}/providers", Name="GetByFrameworkIdAndLocation")]
        public ActionResult<List<FrameworkProviderSearchResultsItemResponse>> GetByFrameworkIdAndLocation(int id, [RequiredFromQuery]double? lat = null,
            [RequiredFromQuery]double? lon = null, int page = 1)
        {
            // TODO 404 if framework doesn't exists
            var actualPage = _controllerHelper.GetActualPage(page);

            if (lat.HasValue && lon.HasValue)
            {
                return _getProviders.GetByFrameworkIdAndLocation(id, lat.Value, lon.Value, actualPage);
            }

            return BadRequest("A valid Latitude and Longitude is required");
        }

        // GET standards/<standardId>/providers?ukprn=<ukprn>&location=<locationId>
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("/standards/{standardCode}/providers", Name="GetStandardProviderDetails")]
        public ActionResult<ApprenticeshipDetails> GetStandardProviderDetails(string standardCode, [RequiredFromQuery]int ukprn, [RequiredFromQuery]int location)
        {
            var model = _apprenticeshipProviderRepository.GetCourseByStandardCode(
                ukprn,
                location,
                standardCode);

            if (model != null)
            {
                return model;
            }

            return NotFound();
        }

        // GET frameworks/<frameworkId>/providers?ukprn=<ukprn>&location=<locationId>
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("/frameworks/{frameworkId}/providers", Name="GetFrameworkProviderDetails")]
        public ActionResult<ApprenticeshipDetails> GetFrameworkProviderDetails(string frameworkId, [RequiredFromQuery]int ukprn, [RequiredFromQuery]int location)
        {
            var model = _apprenticeshipProviderRepository.GetCourseByFrameworkId(
                ukprn,
                location,
                frameworkId);

            if (model != null)
            {
                return model;
            }

            return NotFound();
        }

        private string Resolve(long ukprn)
        {
            return Url.Link("GetProviderByUkprn", new { ukprn = ukprn });
        }
    }
}