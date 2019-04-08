using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models;
using SFA.DAS.Apprenticeships.Api.Types;


namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Configuration;
    using Core.Models;
    using Core.Models.Responses;
    using Core.Services;
    using Mapping;
    using Nest;
    using SFA.DAS.Apprenticeships.Api.Types.Providers;
    using SFA.DAS.Apprenticeships.Api.Types.V3;
    using SFA.DAS.NLog.Logger;

    public sealed class ProviderRepositoryV3 : IGetV3Providers
    {
        private const string ProviderIndexType = "providerapidocument";

        private readonly IElasticsearchCustomClient _elasticsearchCustomClient;
        private readonly ILog _applicationLogger;
        private readonly IConfigurationSettings _applicationSettings;
        private readonly IProviderLocationSearchProvider _providerLocationSearchProvider;
        private readonly IProviderMapping _providerMapping;
        private readonly IQueryHelper _queryHelper;
        private readonly IActiveApprenticeshipChecker _activeApprenticeshipChecker;
        private readonly IPaginationHelper _paginationHelper;

        public ProviderRepositoryV3(
            IElasticsearchCustomClient elasticsearchCustomClient,
            ILog applicationLogger,
            IConfigurationSettings applicationSettings,
            IProviderLocationSearchProvider providerLocationSearchProvider,
            IProviderMapping providerMapping,
            IQueryHelper queryHelper,
            IActiveApprenticeshipChecker activeApprenticeshipChecker,
            IPaginationHelper paginationHelper)
        {
            _elasticsearchCustomClient = elasticsearchCustomClient;
            _applicationLogger = applicationLogger;
            _applicationSettings = applicationSettings;
            _providerLocationSearchProvider = providerLocationSearchProvider;
            _providerMapping = providerMapping;
            _queryHelper = queryHelper;
            _activeApprenticeshipChecker = activeApprenticeshipChecker;
            _paginationHelper = paginationHelper;
        }

        public StandardProviderSearchResult GetByStandardIdAndLocation(int id, double lat, double lon, int page, int pageSize, bool showForNonLevyOnly, bool showNationalOnly, List<DeliveryMode> deliverModes)
        {
            throw new NotImplementedException();
        }
     }
}
