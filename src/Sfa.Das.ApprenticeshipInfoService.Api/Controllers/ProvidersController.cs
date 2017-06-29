namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Sfa.Das.ApprenticeshipInfoService.Api.Attributes;
    using Sfa.Das.ApprenticeshipInfoService.Api.Helpers;
    using Sfa.Das.ApprenticeshipInfoService.Core.Models;
    using Sfa.Das.ApprenticeshipInfoService.Core.Models.Responses;
    using Sfa.Das.ApprenticeshipInfoService.Core.Services;
    using SFA.DAS.Apprenticeships.Api.Types.Providers;
    using Swashbuckle.Swagger.Annotations;
    using IControllerHelper = Sfa.Das.ApprenticeshipInfoService.Core.Helpers.IControllerHelper;

    public class ProvidersController : ApiController
    {
        private readonly IGetProviders _getProviders;
        private readonly IControllerHelper _controllerHelper;
        private readonly IApprenticeshipProviderRepository _apprenticeshipProviderRepository;

        private static readonly Regex UkprnPattern = new Regex(@"^\d{8}");
        private const string BadUkprnMessage = "the ukprn wasn't 8 digits long";

        public ProvidersController(
            IGetProviders getProviders,
            IControllerHelper controllerHelper,
            IApprenticeshipProviderRepository apprenticeshipProviderRepository)
        {
            _getProviders = getProviders;
            _controllerHelper = controllerHelper;
            _apprenticeshipProviderRepository = apprenticeshipProviderRepository;
        }

        /// <summary>
        /// Get all the active providers
        /// </summary>
        /// <returns>a collection of providers</returns>
        [SwaggerOperation("GetAll")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<ProviderSummary>))]
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
        [SwaggerOperation("GetByUkprn")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(Provider))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [Route("providers/{ukprn}")]
        [ExceptionHandling]
        public Provider Get(long ukprn)
        {
            if (!UkprnPattern.IsMatch(ukprn.ToString()))
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
        [Route("providers/{ukprn}")]
        [ExceptionHandling]
        public void Head(long ukprn)
        {
            Get(ukprn);
        }

        /// <summary>
        /// Get a list of providers for an specific standard
        /// </summary>
        /// <param name="apprenticeshipId">Standard id</param>
        /// <returns>A list of Providers</returns>
        [SwaggerOperation("GetProvidersByStandardId")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<Provider>))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("providers/standard/{standardId}", Name = "GetStandardProviders")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ExceptionHandling]
        public IEnumerable<Provider> GetStandardProviders(string standardId)
        {
            var response = _getProviders.GetProvidersByStandardId(standardId);

            if (response == null || !response.Any())
            {
                throw HttpResponseFactory.RaiseException(
                    HttpStatusCode.NotFound,
                    $"No providers found by standard with id {standardId}");
            }

            var providersList = response.GroupBy(x => x.Ukprn).Select(x => x.First());

            var ukprns = providersList.Select(item => item.Ukprn).Select(dummy => (long) dummy).ToList();

            return _getProviders.GetProviderByUkprnList(ukprns);
        }

        /// <summary>
        /// Get a list of providers for an specific framework
        /// </summary>
        /// <param name="apprenticeshipId">Framework id</param>
        /// <returns>A list of Providers</returns>
        [SwaggerOperation("GetProvidersByFrameworkId")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<Provider>))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("providers/framework/{frameworkId}", Name = "GetFrameworkProviders")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ExceptionHandling]
        public IEnumerable<Provider> GetFrameworkProviders(string frameworkId)
        {
            var response = _getProviders.GetProvidersByFrameworkId(frameworkId);

            if (response == null || !response.Any())
            {
                throw HttpResponseFactory.RaiseException(
                    HttpStatusCode.NotFound,
                    $"No providers found by framework with id {frameworkId}");
            }

            var providersList = response.GroupBy(x => x.Ukprn).Select(x => x.First());

            var ukprns = providersList.Select(item => item.Ukprn).Select(dummy => (long)dummy).ToList();

            return _getProviders.GetProviderByUkprnList(ukprns);
        }

        /// <summary>
        /// Get a list of providers locations for an specific standard
        /// </summary>
        /// <param name="apprenticeshipId">Standard id</param>
        /// <returns>A list of Providers</returns>
        [SwaggerOperation("GetProviderLocationByStandardId")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<StandardProviderSearchResultsItem>))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("providers/standardLocation/{standardId}", Name = "GetStandardProviderLocations")]
        [ExceptionHandling]
        public IEnumerable<StandardProviderSearchResultsItem> GetStandardProviderLocations(string standardId)
        {
            var response = _getProviders.GetProvidersByStandardId(standardId);

            if (response == null || !response.Any())
            {
                throw HttpResponseFactory.RaiseException(
                    HttpStatusCode.NotFound,
                    $"No providers found by standard with id {standardId}");
            }

            return response;
        }

        /// <summary>
        /// Get a list of providers locations for an specific framework
        /// </summary>
        /// <param name="apprenticeshipId">Framework id</param>
        /// <returns>A list of Providers</returns>
        [SwaggerOperation("GetProviderLocationByFrameworkId")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<FrameworkProviderSearchResultsItem>))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("providers/frameworkLocation/{frameworkId}", Name = "GetFrameworkProviderLocations")]
        [ExceptionHandling]
        public IEnumerable<FrameworkProviderSearchResultsItem> GetFrameworkProviderLocations(string frameworkId)
        {
            var response = _getProviders.GetProvidersByFrameworkId(frameworkId);

            if (response == null || !response.Any())
            {
                throw HttpResponseFactory.RaiseException(
                    HttpStatusCode.NotFound,
                    $"No providers found by framework with id {frameworkId}");
            }

            return response;
        }

        // GET standards/5/providers?lat=<latitude>&long=<longitude>&page=#
        [SwaggerOperation("GetByStandardIdAndLocation")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(List<StandardProviderSearchResultsItem>))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [ApiExplorerSettings(IgnoreApi = true)]
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