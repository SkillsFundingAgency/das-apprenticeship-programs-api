namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Core.Models;
    using Core.Models.Responses;
    using Core.Services;
    using Helpers;
    using Microsoft.Web.Http;
    using Swashbuckle.Swagger.Annotations;
    using IControllerHelper = Core.Helpers.IControllerHelper;

    [ApiVersion("3.0")]
    [RoutePrefix("v{version:apiVersion}")]
    public class Providers2Controller : ApiController
    {
        private readonly IGetProviders _getProviders;
        private readonly IControllerHelper _controllerHelper;

        public Providers2Controller(IGetProviders getProviders, IControllerHelper controllerHelper)
        {
            _getProviders = getProviders;
            _controllerHelper = controllerHelper;
        }

        // GET standards/5/providers?lat=<latitude>&long=<longitude>&page=#
        [SwaggerOperation("GetByStandardIdAndLocation")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(List<StandardProviderSearchResultsItem>))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Route("standards/{id}/providers")]
        public List<StandardProviderSearchResultsItemResponse> GetByStandardIdAndLocation(int id, double lat, double lon, int page = 1, int pageSize = 20, bool showForNonLevyOnly = false, bool showNationalOnly = false, string deliverModes = null)
        {
            // TODO 404 if standard doesn't exists
            var actualPage = _controllerHelper.GetActualPage(page);

            //if (lat.HasValue && lon.HasValue)
            //{
            //    return _getProviders.GetByStandardIdAndLocation(id, lat.Value, lon.Value, actualPage);
            //}

            //throw HttpResponseFactory.RaiseException(
            //    HttpStatusCode.BadRequest,
            //    "A valid Latitude and Longitude is required");

            throw new NotImplementedException();
        }

        //// GET frameworks/5/providers?lat=<latitude>&long=<longitude>&page=#
        //[SwaggerOperation("GetByFrameworkIdAndLocation")]
        //[SwaggerResponse(HttpStatusCode.OK, "OK", typeof(List<FrameworkProviderSearchResultsItem>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest)]
        //[Route("v{version:apiVersion}/frameworks/{id}/providers")]
        //[Route("frameworks/{id}/providers")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[ExceptionHandling]
        //public List<FrameworkProviderSearchResultsItemResponse> GetByFrameworkIdAndLocation(int id, double? lat = null,
        //    double? lon = null, int page = 1)
        //{
        //    // TODO 404 if framework doesn't exists
        //    var actualPage = _controllerHelper.GetActualPage(page);

        //    if (lat.HasValue && lon.HasValue)
        //    {
        //        return _getProviders.GetByFrameworkIdAndLocation(id, lat.Value, lon.Value, actualPage);
        //    }

        //    throw HttpResponseFactory.RaiseException(
        //        HttpStatusCode.BadRequest,
        //        "A valid Latitude and Longitude is required");
        //}
    }

    public enum DeliveryOptions
    {
        DayRelease,
        BlockRelease,
        AtLocation
    }
}