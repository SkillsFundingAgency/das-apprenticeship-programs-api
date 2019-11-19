using System;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using SFA.DAS.Apprenticeships.Api.Types.V4;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers.V3
{
    [ApiExplorerSettings(GroupName = "v4")]
    [Route("/v4")]
    public class ProvidersV4Controller : ControllerBase
    {
        private readonly IGetProviderApprenticeshipLocationsV4 _getProviders;
        private readonly IControllerHelper _controllerHelper;
        private readonly ILogger<ProvidersV4Controller> _logger;

        public ProvidersV4Controller(
            IGetProviderApprenticeshipLocationsV4 getProviders,
            IControllerHelper controllerHelper,
            ILogger<ProvidersV4Controller> logger)
        {
            _getProviders = getProviders;
            _controllerHelper = controllerHelper;
            _logger = logger;
        }

        /// <summary>
        /// Search for Provider locations for an apprenticeship for a given geo point (lat/lon).
        /// Only the first location for a provider is returned. 
        /// </summary>
        /// <param name="id">{StandardCode} OR {FrameworkCode}-{ProgType}-{PathwayId}</param>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        /// <param name="page">Requested page</param>
        /// <param name="pageSize">Results per page</param>
        /// <param name="showForNonLevyOnly">Show only Non-Levy Providers</param>
        /// <param name="showNationalOnly">Show only National Providers</param>
        /// <returns>a paginated search result</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("apprenticeships/{id}/provider-locations/", Name="GetUniqueProviderByApprenticeshipIdAndLocation")]
        public ActionResult<UniqueProviderApprenticeshipLocationSearchResult> GetByApprenticeshipIdAndLatLon(string id, double lat, double lon, int page = 1, int pageSize = 20, bool showForNonLevyOnly = false, bool showNationalOnly = false)
        {
             try
            {
                var actualPage = _controllerHelper.GetActualPage(page);
                var coordinates = new Coordinate { Lat = lat, Lon = lon };

                UniqueProviderApprenticeshipLocationSearchResult responseContent;

                int standardId;
                if (int.TryParse(id, out standardId))
                {
                    responseContent = _getProviders.SearchStandardProviderLocations(standardId, coordinates, actualPage, pageSize, showForNonLevyOnly, showNationalOnly);
                }
                else
                {
                    responseContent = _getProviders.SearchFrameworkProvidersLocations(id, coordinates, actualPage, pageSize, showForNonLevyOnly, showNationalOnly);
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

        /// <summary>
        /// Get closest locations for a provider of an apprenticeship that cover the given lat/lon
        /// </summary>
        /// <param name="ukprn">Provider ukprn</param>
        /// <param name="id">{StandardCode} OR {FrameworkCode}-{ProgType}-{PathwayId}</param>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        /// <param name="page">Requested page</param>
        /// <param name="pageSize">Results per page</param>
        /// <returns>a paginated search result</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("apprenticeships/{id}/provider/{ukprn}/locations/", Name="GetClosestProviderLocationsThatCoverPointForApprenticeship")]
        public ActionResult<ProviderLocationsSearchResults> GetClosestProviderLocationsThatCoverPointForApprenticeship(long ukprn, string id, double lat, double lon, bool showForNonLevyOnly = false, int page = 1, int pageSize = 5)
        {
            try
            {
                var actualPage = _controllerHelper.GetActualPage(page);
                var coordinates = new Coordinate { Lat = lat, Lon = lon };

                ProviderLocationsSearchResults responseContent;

                int standardId;
                if (int.TryParse(id, out standardId))
                {
                    responseContent = _getProviders.GetClosestLocationsForStandard(ukprn, standardId, coordinates, actualPage, pageSize, showForNonLevyOnly);
                }
                else
                {
                    responseContent = _getProviders.GetClosestLocationsForFramework(ukprn, id, coordinates, actualPage, pageSize, showForNonLevyOnly);
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
    }
}