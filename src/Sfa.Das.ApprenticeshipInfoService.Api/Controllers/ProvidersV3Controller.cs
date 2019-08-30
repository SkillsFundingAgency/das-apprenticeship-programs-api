using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using SFA.DAS.Apprenticeships.Api.Types.V3;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers.V3
{
    //[ApiVersion("3.0")]
    //[RoutePrefix("v{version:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v3")]
    [Route("/v3")]
    public class ProvidersV3Controller : ControllerBase
    {
        private readonly IGetProviderApprenticeshipLocationsV3 _getProviders;
        private readonly IControllerHelper _controllerHelper;
        private readonly ILogger<ProvidersV3Controller> _logger;

        public ProvidersV3Controller(
            IGetProviderApprenticeshipLocationsV3 getProviders,
            IControllerHelper controllerHelper,
            ILogger<ProvidersV3Controller> logger)
        {
            _getProviders = getProviders;
            _controllerHelper = controllerHelper;
            _logger = logger;
        }

        /// <summary>
        /// Get Providers for a given apprenticeship at a given location
        /// </summary>
        /// <param name="id">{StandardCode} OR {FrameworkCode}-{ProgType}-{PathwayId}</param>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        /// <param name="page">Requested page</param>
        /// <param name="pageSize">Results per page</param>
        /// <param name="showForNonLevyOnly">Show only Non-Levy Providers</param>
        /// <param name="showNationalOnly">Show only National Providers</param>
        /// <param name="deliveryModes">Comma separated list of: 0 - Day Release, 1 - Block Release, 2 - At Employers Location</param>
        /// <returns>a paginated search result</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("apprenticeships/{id}/providers", Name="GetByApprenticeshipIdAndLocation")]
        public ActionResult<ProviderApprenticeshipLocationSearchResult> GetByApprenticeshipIdAndLocation(string id, double lat, double lon, int page = 1, int pageSize = 20, bool showForNonLevyOnly = false, bool showNationalOnly = false, string deliveryModes = null)
        {
            try
            {
                var actualPage = _controllerHelper.GetActualPage(page);
                var selectedDeliveryModes = ParseForDeliveryModes(deliveryModes);
                var coordinates = new Coordinate { Lat = lat, Lon = lon };

                ProviderApprenticeshipLocationSearchResult responseContent;

                int standardId;
                if (int.TryParse(id, out standardId))
                {
                    responseContent = _getProviders.SearchStandardProviders(standardId, coordinates, actualPage, pageSize, showForNonLevyOnly, showNationalOnly, selectedDeliveryModes);
                }
                else
                {
                    responseContent = _getProviders.SearchFrameworkProviders(id, coordinates, actualPage, pageSize, showForNonLevyOnly, showNationalOnly, selectedDeliveryModes);
                }

                return responseContent;
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Request.GetEncodedPathAndQuery());
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