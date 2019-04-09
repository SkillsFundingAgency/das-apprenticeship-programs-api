using System.Collections.Generic;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch.V3;
using SFA.DAS.Apprenticeships.Api.Types.V3;
using SFA.DAS.NLog.Logger;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch
{
    public sealed class ProviderRepositoryV3 : IGetV3Providers
    {
        private readonly ILog _logger;
        private readonly IProviderLocationSearchProviderV3 _providerLocationSearchProvider;

        public ProviderRepositoryV3(
            ILog applicationLogger,
            IProviderLocationSearchProviderV3 providerLocationSearchProvider)
        {
            _logger = applicationLogger;
            _providerLocationSearchProvider = providerLocationSearchProvider;
        }

        public StandardProviderSearchResult GetByStandardIdAndLocation(int id, double lat, double lon, int page, int pageSize, bool showForNonLevyOnly, bool showNationalOnly, List<DeliveryMode> deliveryModes)
        {
            var coordinates = new Coordinate
            {
                Lat = lat,
                Lon = lon
            };

            var providers = _providerLocationSearchProvider.SearchStandardProviders(id, coordinates, page, pageSize, showForNonLevyOnly, showNationalOnly, deliveryModes);

            return providers;
        }
     }
}
