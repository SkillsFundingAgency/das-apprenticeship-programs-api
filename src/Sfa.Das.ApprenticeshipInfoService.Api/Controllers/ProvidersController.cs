﻿namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    using System;
    using Sfa.Das.ApprenticeshipInfoService.Core.Logging;
    using System.Collections.Generic;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Sfa.Das.ApprenticeshipInfoService.Api.Attributes;
    using Sfa.Das.ApprenticeshipInfoService.Api.Helpers;
    using Sfa.Das.ApprenticeshipInfoService.Core.Models;
    using Sfa.Das.ApprenticeshipInfoService.Core.Models.Responses;
    using Sfa.Das.ApprenticeshipInfoService.Core.Services;
    using SFA.DAS.Apprenticeships.Api.Types;
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

        // GET /providers
        [SwaggerOperation("GetAll")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<Provider>))]
        [Route("providers")]
        [ExceptionHandling]
        public IEnumerable<Provider> Get()
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

        // GET /providers/10005318
        [SwaggerOperation("GetByUkprn")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<Provider>))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("providers/{ukprn}")]
        [ExceptionHandling]
        public IEnumerable<Provider> Get(int ukprn)
        {
            try
            {
                var response = _getProviders.GetProviderByUkprn(ukprn);
                
                if (response == null)
                {
                    throw HttpResponseFactory.RaiseException(HttpStatusCode.NotFound,
                        $"No provider with Ukprn {ukprn} found");
                }

                response.Uri = Resolve(response.Ukprn);

                return new List<Provider> { response };
            }
            catch (Exception e)
            {
                _logger.Error(e, $"providers/{ukprn}");
                throw;
            }
        }

        // HEAD /providers/10005318
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("providers/{ukprn}")]
        [ExceptionHandling]
        public void Head(int ukprn)
        {
            if (_getProviders.GetProviderByUkprn(ukprn) != null)
            {
                return;
            }

            throw HttpResponseFactory.RaiseException(HttpStatusCode.NotFound,
                $"No provider with Ukprn {ukprn} found");
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

                throw HttpResponseFactory.RaiseException(HttpStatusCode.BadRequest,
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

                throw HttpResponseFactory.RaiseException(HttpStatusCode.BadRequest,
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

        private string Resolve(int ukprn)
        {
            return Url.Link("DefaultApi", new { controller = "providers" }) + "/" + ukprn;
        }
    }
}