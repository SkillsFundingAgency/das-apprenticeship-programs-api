using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Microsoft.Web.Http;
using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using SFA.DAS.Apprenticeships.Api.Types.V3;
using SFA.DAS.NLog.Logger;
using Swashbuckle.Swagger.Annotations;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers.V3
{
    [ApiVersion("3.0")]
    [RoutePrefix("v{version:apiVersion}")]
    public class ProvidersV3Controller : ApiController
    {
        private readonly IGetProviderApprenticeshipLocationsV3 _getProviders;
        private readonly IControllerHelper _controllerHelper;
        private readonly ILog _logger;

        public ProvidersV3Controller(
            IGetProviderApprenticeshipLocationsV3 getProviders,
            IControllerHelper controllerHelper,
            ILog logger)
        {
            _getProviders = getProviders;
            _controllerHelper = controllerHelper;
            _logger = logger;
        }

        /// <summary>
        /// Get Providers for a standard at a given location
        /// </summary>
        /// <param name="id">Standard Code</param>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        /// <param name="page">Requested page</param>
        /// <param name="pageSize">Results per page</param>
        /// <param name="showForNonLevyOnly">Show only Non-Levy Providers</param>
        /// <param name="showNationalOnly">Show only National Providers</param>
        /// <param name="deliveryModes">Comma separated list of: 0 - Day Release, 1 - Block Release, 2 - At Employers Location</param>
        /// <returns>a paginated search result</returns>
        [SwaggerOperation("GetByStandardIdAndLocation")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(List<ProviderApprenticeshipLocationSearchResult>))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Route("standards/{id}/providers")]
        public IHttpActionResult GetByStandardIdAndLocation(int id, double lat, double lon, int page = 1, int pageSize = 20, bool showForNonLevyOnly = false, bool showNationalOnly = false, string deliveryModes = null)
        {
            try
            {
                var actualPage = _controllerHelper.GetActualPage(page);
                var selectedDeliveryModes = ParseForDeliveryModes(deliveryModes);
                var coordinates = new Coordinate { Lat = lat, Lon = lon };

                var responseContent = _getProviders.SearchStandardProviders(id, coordinates, actualPage, pageSize, showForNonLevyOnly, showNationalOnly, selectedDeliveryModes);

                return Ok(responseContent);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, Request.RequestUri.PathAndQuery);
                throw;
            }
        }


        /// <summary>
        /// Get Providers for a framework at a given location
        /// </summary>
        /// <param name="id">{FrameworkCode}-{ProgType}-{PathwayId}</param>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        /// <param name="page">Requested page</param>
        /// <param name="pageSize">Results per page</param>
        /// <param name="showForNonLevyOnly">Show only Non-Levy Providers</param>
        /// <param name="showNationalOnly">Show only National Providers</param>
        /// <param name="deliveryModes">Comma separated list of: 0 - Day Release, 1 - Block Release, 2 - At Employers Location</param>
        /// <returns>a paginated search result</returns>
        [SwaggerOperation("GetByFrameworkIdAndLocation")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(List<ProviderApprenticeshipLocationSearchResult>))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Route("frameworks/{id}/providers")]
        public IHttpActionResult GetByFrameworkIdAndLocation(string id, double lat, double lon, int page = 1, int pageSize = 20, bool showForNonLevyOnly = false, bool showNationalOnly = false, string deliveryModes = null)
        {
            try
            {
                var actualPage = _controllerHelper.GetActualPage(page);
                var selectedDeliveryModes = ParseForDeliveryModes(deliveryModes);
                var coordinates = new Coordinate { Lat = lat, Lon = lon };

                var responseContent = _getProviders.SearchFrameworkProviders(id, coordinates, actualPage, pageSize, showForNonLevyOnly, showNationalOnly, selectedDeliveryModes);

                return Ok(responseContent);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, Request.RequestUri.PathAndQuery);
                throw;
            }
        }

        private List<DeliveryMode> ParseForDeliveryModes(string ids)
        {
            if (ids != null && !string.IsNullOrWhiteSpace(ids))
            {
                var response = new List<DeliveryMode>();

                DeliveryMode mode;

                foreach (var idElement in ids.Split(','))
                {
                    var validInt = Enum.TryParse<DeliveryMode>(idElement, out mode);

                    if (validInt)
                    {
                        response.Add(mode);
                    }
                    else
                    {
                        throw new ArgumentException($"{idElement} is not a valid delivery mode value");
                    }
                }

                return response;
            }

            return null;
        }
    }
}