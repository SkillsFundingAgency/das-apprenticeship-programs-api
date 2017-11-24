namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Configuration;
    using Core.Models;
    using Core.Models.Responses;
    using Core.Services;
    using FeatureToggle.Core.Fluent;
    using FeatureToggles;
    using Mapping;
    using Nest;
    using SFA.DAS.Apprenticeships.Api.Types.Providers;
    using SFA.DAS.NLog.Logger;

    public sealed class ProviderRepository : IGetProviders
    {
        private readonly IElasticsearchCustomClient _elasticsearchCustomClient;
        private readonly ILog _applicationLogger;
        private readonly IConfigurationSettings _applicationSettings;
        private readonly IProviderLocationSearchProvider _providerLocationSearchProvider;
        private readonly IProviderMapping _providerMapping;
        private readonly IQueryHelper _queryHelper;
        private readonly string _providerDocumentType;

        public ProviderRepository(
            IElasticsearchCustomClient elasticsearchCustomClient,
            ILog applicationLogger,
            IConfigurationSettings applicationSettings,
            IProviderLocationSearchProvider providerLocationSearchProvider,
            IProviderMapping providerMapping,
            IQueryHelper queryHelper)
        {
            _elasticsearchCustomClient = elasticsearchCustomClient;
            _applicationLogger = applicationLogger;
            _applicationSettings = applicationSettings;
            _providerLocationSearchProvider = providerLocationSearchProvider;
            _providerMapping = providerMapping;
            _queryHelper = queryHelper;

            _providerDocumentType = Is<RoatpProvidersFeature>.Enabled ? "providerapidocument" : "providerdocument";
        }

        public IEnumerable<ProviderSummary> GetAllProviders()
        {
            var take = _queryHelper.GetProvidersTotalAmount();
            var results =
                _elasticsearchCustomClient.Search<Provider>(
                    s =>
                    s.Index(_applicationSettings.ProviderIndexAlias)
                        .Type(Types.Parse(_providerDocumentType))
                        .From(0)
                        .Sort(sort => sort.Ascending(f => f.Ukprn))
                        .Take(take)
                        .MatchAll());

            if (results.ApiCall.HttpStatusCode != 200)
            {
                _applicationLogger.Warn($"httpStatusCode was {results.ApiCall.HttpStatusCode}");
                throw new ApplicationException("Failed query all providers");
            }

            return results.Documents.Select(provider => _providerMapping.MapToProviderDto(provider)).ToList();
        }

        public Provider GetProviderByUkprn(long ukprn)
        {
            var results =
                _elasticsearchCustomClient.Search<Provider>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .Type(Types.Parse(_providerDocumentType))
                            .From(0)
                            .Sort(sort => sort.Ascending(f => f.Ukprn))
                            .Take(100)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.Ukprn)
                                    .Terms(ukprn))));

            if (results.ApiCall.HttpStatusCode != 200)
            {
                _applicationLogger.Warn($"httpStatusCode was {results.ApiCall.HttpStatusCode}");
                throw new ApplicationException("Failed query provider by ukprn");
            }

            if (results.Documents.Count() > 1)
            {
                _applicationLogger.Warn($"found {results.Documents.Count()} providers for the ukprn {ukprn}");
            }

            return results.Documents.FirstOrDefault();
        }

        public IEnumerable<Provider> GetProviderByUkprnList(List<long> ukprns)
        {
            var results =
                _elasticsearchCustomClient.Search<Provider>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .Type(Types.Parse(_providerDocumentType))
                            .From(0)
                            .Sort(sort => sort.Ascending(f => f.Ukprn))
                            .Take(ukprns.Count)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.Ukprn)
                                    .Terms(ukprns))));

            if (results.ApiCall.HttpStatusCode != 200)
            {
                _applicationLogger.Warn($"httpStatusCode was {results.ApiCall.HttpStatusCode}");
                throw new ApplicationException("Failed query provider by ukprn");
            }

            if (results.Documents.Count() > 1)
            {
                _applicationLogger.Warn($"found {results.Documents.Count()} providers for the ukprns provided");
            }

            return results.Documents;
        }

        public IEnumerable<ProviderFramework> GetFrameworksByProviderUkprn(long ukprn)
        {
            var take = _applicationSettings.ProviderApprenticeshipsMaximum;
            var providers =
                _elasticsearchCustomClient.Search<Provider>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .Type(Types.Parse(_providerDocumentType))
                            .From(0)
                            .Sort(sort => sort.Ascending(f => f.Ukprn))
                            .Take(take)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.Ukprn)
                                    .Terms(ukprn))));

            if (providers.ApiCall.HttpStatusCode != 200)
            {
                _applicationLogger.Warn($"httpStatusCode was {providers.ApiCall.HttpStatusCode}");
                throw new ApplicationException("Failed query frameworks by provider ukprn");
            }

            if (providers.Documents.Count() > 1)
            {
                _applicationLogger.Warn($"found {providers.Documents.Count()} providers (checking frameworks) for the ukprn {ukprn}");
            }

            var provider = providers.Documents.FirstOrDefault();

            return provider?.Frameworks;
        }

        public IEnumerable<ProviderStandard> GetStandardsByProviderUkprn(long ukprn)
        {
            var take = _applicationSettings.ProviderApprenticeshipsMaximum;

            var providers =
                _elasticsearchCustomClient.Search<Provider>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .Type(Types.Parse(_providerDocumentType))
                            .From(0)
                            .Sort(sort => sort.Ascending(f => f.Ukprn))
                            .Take(take)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.Ukprn)
                                    .Terms(ukprn))));

            if (providers.ApiCall.HttpStatusCode != 200)
            {
                _applicationLogger.Warn($"httpStatusCode was {providers.ApiCall.HttpStatusCode}");
                throw new ApplicationException("Failed query standards by provider ukprn");
            }

            if (providers.Documents.Count() > 1)
            {
                _applicationLogger.Warn($"found {providers.Documents.Count()} providers (checking standards) for the ukprn {ukprn}");
            }

            var provider = providers.Documents.FirstOrDefault();

            return provider?.Standards;
        }

        public List<StandardProviderSearchResultsItemResponse> GetByStandardIdAndLocation(int id, double lat, double lon, int page)
        {
            var coordinates = new Coordinate
            {
                Lat = lat,
                Lon = lon
            };

            var providers = _providerLocationSearchProvider.SearchStandardProviders(id, coordinates, page);

            return providers.Select(provider => _providerMapping.MapToStandardProviderResponse(provider)).ToList();
        }

        public List<FrameworkProviderSearchResultsItemResponse> GetByFrameworkIdAndLocation(int id, double lat, double lon, int page)
        {
            var coordinates = new Coordinate
            {
                Lat = lat,
                Lon = lon
            };

            var providers = _providerLocationSearchProvider.SearchFrameworkProviders(id, coordinates, page);

            return providers.Select(provider => _providerMapping.MapToFrameworkProviderResponse(provider)).ToList();
        }

        public IEnumerable<StandardProviderSearchResultsItem> GetProvidersByStandardId(string standardId)
        {
            var take = _queryHelper.GetProvidersByStandardTotalAmount(standardId);

            var results =
                _elasticsearchCustomClient.Search<StandardProviderSearchResultsItem>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .From(0)
                            .Sort(sort => sort.Ascending(f => f.Ukprn))
                            .Take(take)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.StandardCode)
                                    .Terms(standardId))));

            if (results.ApiCall.HttpStatusCode != 200)
            {
                _applicationLogger.Warn($"httpStatusCode was {results.ApiCall.HttpStatusCode}");
                throw new ApplicationException("Failed query providers by standard code");
            }

            if (results.Documents.Count() > 1)
            {
                _applicationLogger.Warn($"found {results.Documents.Count()} providers for the standard {standardId}");
            }

            return results.Documents;
        }

        public IEnumerable<FrameworkProviderSearchResultsItem> GetProvidersByFrameworkId(string frameworkId)
        {
            var take = _queryHelper.GetProvidersByFrameworkTotalAmount(frameworkId);

            var results =
                _elasticsearchCustomClient.Search<FrameworkProviderSearchResultsItem>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .From(0)
                            .Sort(sort => sort.Ascending(f => f.Ukprn))
                            .Take(take)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.FrameworkId)
                                    .Terms(frameworkId))));

            if (results.ApiCall.HttpStatusCode != 200)
            {
                _applicationLogger.Warn($"httpStatusCode was {results.ApiCall.HttpStatusCode}");
                throw new ApplicationException("Failed query providers by standard code");
            }

            if (results.Documents.Count() > 1)
            {
                _applicationLogger.Warn($"found {results.Documents.Count()} providers for the framework {frameworkId}");
            }

            return results.Documents;
        }

       
    }
}
