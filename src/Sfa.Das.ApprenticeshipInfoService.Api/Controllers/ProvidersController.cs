namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Attributes;
    using Core.Models;
    using Core.Models.Responses;
    using Core.Services;
    using Helpers;
    using SFA.DAS.Apprenticeships.Api.Types.Providers;
    using Swashbuckle.Swagger.Annotations;
    using IControllerHelper = Core.Helpers.IControllerHelper;

    public class ProvidersController : ApiController
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
        [SwaggerOperation("GetAllProviders")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<ProviderSummary>))]
        [Route("v{version:apiVersion}/providers")]
        [Route("providers")]
        [ExceptionHandling]
        public IEnumerable<ProviderSummary> Get()
        {
            var response = _getProviders.GetAllProviders();

            foreach (var provider in response)
            {
                provider.Uri = Resolve(provider.Ukprn);
            }

            return response;
        }

        /// <summary>
        /// Get a provider
        /// </summary>
        /// <param name="ukprn">UKPRN</param>
        /// <returns>A Provider</returns>
        [SwaggerOperation("GetProviderByUkprn")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(Provider))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.BadRequest, BadUkprnMessage)]
        [Route("v{version:apiVersion}/providers/{ukprn:long}")]
        [Route("providers/{ukprn:long}")]
        [ExceptionHandling]
        public Provider Get(long ukprn)
        {
            if (ukprn.ToString().Length != 8)
            {
                throw HttpResponseFactory.RaiseException(HttpStatusCode.BadRequest, BadUkprnMessage);
            }

            var response = _getProviders.GetProviderByUkprn(ukprn);

            if (response == null)
            {
                throw HttpResponseFactory.RaiseException(
                    HttpStatusCode.NotFound,
                    $"No provider with Ukprn {ukprn} found");
            }

            response.Uri = Resolve(response.Ukprn);

            return response;
        }

        /// <summary>
        /// Do we have providers?
        /// </summary>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("v{version:apiVersion}/providers")]
        [Route("providers")]
        [ExceptionHandling]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Head()
        {
            Get();
        }

        /// <summary>
        /// Provider exists?
        /// </summary>
        /// <param name="ukprn">UKPRN</param>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [Route("v{version:apiVersion}/providers/{ukprn:long}")]
        [Route("providers/{ukprn:long}")]
        [ExceptionHandling]
        public void Head(long ukprn)
        {
            Get(ukprn);
        }

        /// <summary>
        /// Get list of active apprenticeships for a given provider
        /// </summary>
        /// <param name="ukprn">unique id</param>
        /// <returns>A list of active apprenticeships sorted by name alphabetically, then type, then level</returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<ApprenticeshipTraining>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [SwaggerResponse(HttpStatusCode.BadRequest, BadUkprnMessage)]
        [Route("v{version:apiVersion}/providers/{ukprn:long}/active-apprenticeship-training")]
        [Route("providers/{ukprn:long}/active-apprenticeship-training", Name = "GetActiveApprenticeshipsByProvider")]
        [ExceptionHandling]
        public ApprenticeshipTrainingSummary GetActiveApprenticeshipTrainingByProvider(long ukprn)
        {
            return GetActiveApprenticeshipTrainingByProvider(ukprn, 1);
        }

        /// <summary>
        /// Get list of active apprenticeships for a given provider
        /// </summary>
        /// <param name="ukprn">unique id</param>
        /// <param name="page">number of page for which results are returned (default 1)</param>
        /// <returns>A list of active apprenticeships sorted by name alphabetically, then type, then level</returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<ApprenticeshipTraining>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [SwaggerResponse(HttpStatusCode.BadRequest, BadUkprnMessage)]
        [SwaggerOperation("GetActiveApprenticeshipTrainingByProviderAndPage")]
        [Route("v{version:apiVersion}/providers/{ukprn:long}/active-apprenticeship-training/{page}")]
        [Route("providers/{ukprn:long}/active-apprenticeship-training/{page}", Name = "GetActiveApprenticeshipsByProviderByPage")]
        [ExceptionHandling]
        public ApprenticeshipTrainingSummary GetActiveApprenticeshipTrainingByProvider(long ukprn, int page)
        {
            if (ukprn.ToString().Length != 8)
            {
                throw HttpResponseFactory.RaiseException(HttpStatusCode.BadRequest, BadUkprnMessage);
            }

            return _getProviders.GetActiveApprenticeshipTrainingByProvider(ukprn, page);
        }

        /// <summary>
        /// Get a list of providers for an specific standard
        /// </summary>
        /// <param name="apprenticeshipId">Standard id</param>
        /// <returns>A list of Providers</returns>
        [SwaggerOperation("GetProvidersByStandardId")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<Provider>))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route("v{version:apiVersion}/providers/standard/{standardId}")]
        [Route("providers/standard/{standardId}", Name = "GetStandardProviders")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ExceptionHandling]
        public IEnumerable<Provider> GetStandardProviders(string standardId)
        {
            if (_getStandards.GetStandardById(standardId) == null)
            {
                throw HttpResponseFactory.RaiseException(
                    HttpStatusCode.NotFound,
                    $"The standard {standardId} is not found");
            }

            var response = _getProviders.GetProvidersByStandardId(standardId);

            var providersList = response.GroupBy(x => x.Ukprn).Select(x => x.First());

            var ukprns = providersList.Select(item => item.Ukprn).Select(dummy => (long) dummy).ToList();

            return _getProviders.GetProviderByUkprnList(ukprns);
        }

        /// <summary>
        /// Get a list of providers for an specific framework
        /// </summary>
        /// <param name="frameworkId">Framework id</param>
        /// <returns>A list of Providers</returns>
        [SwaggerOperation("GetProvidersByFrameworkId")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<Provider>))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route("v{version:apiVersion}/providers/framework/{frameworkId}")]
        [Route("providers/framework/{frameworkId}", Name = "GetFrameworkProviders")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ExceptionHandling]
        public IEnumerable<Provider> GetFrameworkProviders(string frameworkId)
        {
            if (_getFrameworks.GetFrameworkById(frameworkId) == null)
            {
                throw HttpResponseFactory.RaiseException(
                    HttpStatusCode.NotFound,
                    $"The framework {frameworkId} is not found, it should be in the format {{framework code}}-{{program type}}-{{pathway code}}");
            }

            var response = _getProviders.GetProvidersByFrameworkId(frameworkId);

            var providersList = response.GroupBy(x => x.Ukprn).Select(x => x.First());

            var ukprns = providersList.Select(item => item.Ukprn).Select(dummy => (long)dummy).ToList();

            return _getProviders.GetProviderByUkprnList(ukprns);
        }

        /// <summary>
        /// Get a list of providers locations for an specific standard
        /// TODO update url
        /// </summary>
        /// <param name="apprenticeshipId">Standard id</param>
        /// <returns>A list of Providers</returns>
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
        [SwaggerOperation("GetByStandardIdAndLocation")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(List<StandardProviderSearchResultsItem>))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("v{version:apiVersion}/standards/{id}/providers")]
        [Route("standards/{id}/providers")]
        public List<StandardProviderSearchResultsItemResponse> GetByStandardIdAndLocation(int id, double? lat = null,
            double? lon = null, int page = 1)
        {
            // TODO 404 if standard doesn't exists
            var actualPage = _controllerHelper.GetActualPage(page);

            if (lat.HasValue && lon.HasValue)
            {
                return _getProviders.GetByStandardIdAndLocation(id, lat.Value, lon.Value, actualPage);
            }

            throw HttpResponseFactory.RaiseException(
                HttpStatusCode.BadRequest,
                "A valid Latitude and Longitude is required");
        }

        // GET frameworks/5/providers?lat=<latitude>&long=<longitude>&page=#
        [SwaggerOperation("GetByFrameworkIdAndLocation")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(List<FrameworkProviderSearchResultsItem>))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [Route("v{version:apiVersion}/frameworks/{id}/providers")]
        [Route("frameworks/{id}/providers")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ExceptionHandling]
        public List<FrameworkProviderSearchResultsItemResponse> GetByFrameworkIdAndLocation(int id, double? lat = null,
            double? lon = null, int page = 1)
        {
            // TODO 404 if framework doesn't exists
            var actualPage = _controllerHelper.GetActualPage(page);

            if (lat.HasValue && lon.HasValue)
            {
                return _getProviders.GetByFrameworkIdAndLocation(id, lat.Value, lon.Value, actualPage);
            }

            throw HttpResponseFactory.RaiseException(
                HttpStatusCode.BadRequest,
                "A valid Latitude and Longitude is required");
        }

        // GET standards/<standardId>/providers?ukprn=<ukprn>&location=<locationId>
        [SwaggerOperation("GetStandardProviderDetails")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(ApprenticeshipDetails))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("v{version:apiVersion}/standards/{standardCode}/providers")]
        [Route("standards/{standardCode}/providers")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ExceptionHandling]
        public ApprenticeshipDetails GetStandardProviderDetails(string standardCode, int ukprn, int location)
        {
            var model = _apprenticeshipProviderRepository.GetCourseByStandardCode(
                ukprn,
                location,
                standardCode);

            if (model != null)
            {
                return model;
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        // GET frameworks/<frameworkId>/providers?ukprn=<ukprn>&location=<locationId>
        [SwaggerOperation("GetFrameworkProviderDetails")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(ApprenticeshipDetails))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("v{version:apiVersion}/frameworks/{frameworkId}/providers")]
        [Route("frameworks/{frameworkId}/providers")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ExceptionHandling]
        public ApprenticeshipDetails GetFrameworkProviderDetails(string frameworkId, int ukprn, int location)
        {
            var model = _apprenticeshipProviderRepository.GetCourseByFrameworkId(
                ukprn,
                location,
                frameworkId);

            if (model != null)
            {
                return model;
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        private string Resolve(long ukprn)
        {
            return Url.Link("DefaultApi", new { controller = "providers", id = ukprn });
        }
    }
}