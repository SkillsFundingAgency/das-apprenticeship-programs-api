namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Sfa.Das.ApprenticeshipInfoService.Api.Attributes;
    using Sfa.Das.ApprenticeshipInfoService.Api.Helpers;
    using Sfa.Das.ApprenticeshipInfoService.Core.Models;
    using Sfa.Das.ApprenticeshipInfoService.Core.Models.Responses;
    using Sfa.Das.ApprenticeshipInfoService.Core.Services;
    using SFA.DAS.Apprenticeships.Api.Types.Providers;
    using SFA.DAS.NLog.Logger;
    using Swashbuckle.Swagger.Annotations;
    using IControllerHelper = Sfa.Das.ApprenticeshipInfoService.Core.Helpers.IControllerHelper;

    public class ProvidersController : ApiController
    {
        private readonly IGetProviders _getProviders;
        private readonly IControllerHelper _controllerHelper;
        private readonly IApprenticeshipProviderRepository _apprenticeshipProviderRepository;
        private readonly ILog _logger;

        public ProvidersController(
            IGetProviders getProviders,
            IControllerHelper controllerHelper,
            IApprenticeshipProviderRepository apprenticeshipProviderRepository,
            ILog logger)
        {
            _getProviders = getProviders;
            _controllerHelper = controllerHelper;
            _apprenticeshipProviderRepository = apprenticeshipProviderRepository;
            _logger = logger;
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
            try
            {
                var response = _getProviders.GetAllProviders();

                foreach (var provider in response)
                {
                    provider.Uri = Resolve(provider.Ukprn);
                }

                return response;
            }
            catch (Exception e)
            {
                _logger.Error(e, "/providers");
                throw;
            }
        }

        /// <summary>
        /// Get a provider
        /// </summary>
        /// <param name="ukprn">UKPRN</param>
        /// <returns>A Provider</returns>
        [SwaggerOperation("GetByUkprn")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(Provider))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("providers/{ukprn}")]
        [ExceptionHandling]
        public Provider Get(long ukprn)
        {
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
        [SwaggerOperation("GetByUkprn")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<StandardProviderSearchResultsItem>))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("providers/standard/{standardId}")]
        [ExceptionHandling]
        public IEnumerable<StandardProviderSearchResultsItem> GetStandardProviders(string standardId)
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
        /// Get a list of providers for an specific framework
        /// </summary>
        /// <param name="apprenticeshipId">Framework id</param>
        /// <returns>A list of Providers</returns>
        [SwaggerOperation("GetByUkprn")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<FrameworkProviderSearchResultsItem>))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("providers/framework/{frameworkId}")]
        [ExceptionHandling]
        public IEnumerable<FrameworkProviderSearchResultsItem> GetFrameworkProviders(string frameworkId)
        {
            var response = _getProviders.GetProvidersByFrameworkId(frameworkId);

            if (response == null || !response.Any())
            {
                throw HttpResponseFactory.RaiseException(
                    HttpStatusCode.NotFound,
                    $"No providers found by standard with id {frameworkId}");
            }

            return response;
        }

        // GET standards/5/providers?lat=<latitude>&long=<longitude>&page=#
        [SwaggerOperation("GetByStandardIdAndLocation")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(List<StandardProviderSearchResultsItem>))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("standards/{id}/providers")]
        public List<StandardProviderSearchResultsItemResponse> GetByStandardIdAndLocation(int id, double? lat = null, double? lon = null, int page = 1)
        {
            // TODO 404 if standard doesn't exists
            try
            {
                var actualPage = _controllerHelper.GetActualPage(page);

                if (lat.HasValue && lon.HasValue)
                {
                    return _getProviders.GetByStandardIdAndLocation(id, lat.Value, lon.Value, actualPage);
                }

                throw HttpResponseFactory.RaiseException(
                    HttpStatusCode.BadRequest,
                    "A valid Latitude and Longitude is required");
            }
            catch (Exception e)
            {
                _logger.Error(e, $"standards/{id}/providers");
                throw;
            }
        }

        // GET frameworks/5/providers?lat=<latitude>&long=<longitude>&page=#
        [SwaggerOperation("GetByFrameworkIdAndLocation")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(List<FrameworkProviderSearchResultsItem>))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [Route("frameworks/{id}/providers")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ExceptionHandling]
        public List<FrameworkProviderSearchResultsItemResponse> GetByFrameworkIdAndLocation(int id, double? lat = null, double? lon = null, int page = 1)
        {
            // TODO 404 if framework doesn't exists
            try
            {
                var actualPage = _controllerHelper.GetActualPage(page);

                if (lat.HasValue && lon.HasValue)
                {
                    return _getProviders.GetByFrameworkIdAndLocation(id, lat.Value, lon.Value, actualPage);
                }

                throw HttpResponseFactory.RaiseException(
                    HttpStatusCode.BadRequest,
                    "A valid Latitude and Longitude is required");
            }
            catch (Exception e)
            {
                _logger.Error(e, $"frameworks/{id}/providers");
                throw;
            }
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