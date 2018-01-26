using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models;

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
    using SFA.DAS.NLog.Logger;

    public sealed class ProviderRepository : IGetProviders
    {
        private const string ProviderIndexType = "providerapidocument";

        private readonly IElasticsearchCustomClient _elasticsearchCustomClient;
        private readonly ILog _applicationLogger;
        private readonly IConfigurationSettings _applicationSettings;
        private readonly IProviderLocationSearchProvider _providerLocationSearchProvider;
        private readonly IProviderMapping _providerMapping;
        private readonly IQueryHelper _queryHelper;

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
        }

        public IEnumerable<ProviderSummary> GetAllProviders()
        {
            var take = _queryHelper.GetProvidersTotalAmount();
            var results =
                _elasticsearchCustomClient.Search<Provider>(
                    s =>
                    s.Index(_applicationSettings.ProviderIndexAlias)
                        .Type(Types.Parse(ProviderIndexType))
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
                            .Type(Types.Parse(ProviderIndexType))
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
                _applicationLogger.Debug($"found {results.Documents.Count()} providers for the ukprn {ukprn}");
            }

            return results.Documents.FirstOrDefault();
        }

        public IEnumerable<Provider> GetProviderByUkprnList(List<long> ukprns)
        {
            var results =
                _elasticsearchCustomClient.Search<Provider>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .Type(Types.Parse(ProviderIndexType))
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
                _applicationLogger.Debug($"found {results.Documents.Count()} providers for the ukprns provided");
            }

            return results.Documents;
        }

        public IEnumerable<ProviderFramework> GetFrameworksByProviderUkprn(long ukprn)
        {
            var totalTakeFromFrameworkProviders = _queryHelper.GetFrameworkProviderTotalAmount();

            var matchedIds =
                _elasticsearchCustomClient.Search<ProviderFrameworkDto>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .Type(Types.Parse("frameworkprovider"))
                            .From(0)
                            .Take(totalTakeFromFrameworkProviders)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.Ukprn)
                                    .Terms(ukprn))));

            if (matchedIds.ApiCall.HttpStatusCode != 200)
            {
                _applicationLogger.Warn($"httpStatusCode was {matchedIds.ApiCall.HttpStatusCode} when querying provider frameworks for ukprn [{ukprn}]");
                throw new ApplicationException($"Failed to query provider frameworks for ukprn [{ukprn}]");
            }

            var totalTakeForFrameworkDocuments = _queryHelper.GetFrameworksTotalAmount();

            var providerFrameworks =
                _elasticsearchCustomClient.Search<ProviderFramework>(
                    s =>
                        s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                            .Type(Types.Parse("frameworkdocument"))
                            .From(0)
                            .Take(totalTakeForFrameworkDocuments)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.FrameworkId)
                                    .Terms(matchedIds.Documents.Select(x => x.FrameworkId)))));

            if (providerFrameworks.ApiCall.HttpStatusCode != 200)
            {
                _applicationLogger.Warn($"httpStatusCode was {providerFrameworks.ApiCall.HttpStatusCode} when querying provider frameworks apprenticeship details for ukprn [{ukprn}]");
                throw new ApplicationException($"Failed to query provider frameworks apprenticeship details for ukprn [{ukprn}]");
            }

            return providerFrameworks.Documents;
        }

        public IEnumerable<ProviderStandard> GetStandardsByProviderUkprn(long ukprn)
        {
            _applicationLogger.Debug($"For 'GetStandardsByProviderUkprn', fetching a count of totalTakeFromStandardProviders");
            var totalTakeFromStandardProviders = 0;
            try
            {
                totalTakeFromStandardProviders = _queryHelper.GetStandardProviderTotalAmount();
            }
            catch (Exception ex)
            {
                _applicationLogger.Error(ex, $"For 'GetStandardsByProviderUkprn', fetching a count of totalTakeFromStandardProviders caused an error");

            }

            _applicationLogger.Debug($"For 'GetStandardsByProviderUkprn', fetched a count of totalTakeFromStandardProviders: {totalTakeFromStandardProviders}");

            var matchedIds =
                _elasticsearchCustomClient.Search<ProviderStandardDto>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .Type(Types.Parse("standardprovider"))
                            .From(0)
                            .Take(totalTakeFromStandardProviders)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.Ukprn)
                                    .Terms(ukprn))));

            _applicationLogger.Debug($"For 'GetStandardsByProviderUkprn', fetched a collection of matchedIds.Documents");

            _applicationLogger.Debug($"For 'GetStandardsByProviderUkprn', fetched a total number of matchedIds.Documents: {matchedIds.Documents.Count}");

            if (matchedIds.ApiCall.HttpStatusCode != 200)
            {
                 _applicationLogger.Warn($"httpStatusCode was {matchedIds.ApiCall.HttpStatusCode} when querying provider standards for ukprn [{ukprn}]");

                throw new ApplicationException($"Failed to query provider standards for ukprn [{ukprn}]");
            }
            _applicationLogger.Debug($"For 'GetStandardsByProviderUkprn', fetching a count of totalTakeForStandardDocuments");

            var totalTakeForStandardDocuments = 0;
            try
            {
                totalTakeForStandardDocuments = _queryHelper.GetStandardsTotalAmount();
            }
            catch (Exception ex)
            {
                _applicationLogger.Error(ex, $"For 'GetStandardsByProviderUkprn', fetching a count of totalTakeForStandardDocuments caused an error");

            }

            _applicationLogger.Debug($"For 'GetStandardsByProviderUkprn', fetched a count of totalTakeForStandardDocuments: {totalTakeForStandardDocuments}");

             var providerStandards =
                _elasticsearchCustomClient.Search<ProviderStandard>(
                    s =>
                        s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                            .Type(Types.Parse("standarddocument"))
                            .From(0)
                            .Take(totalTakeForStandardDocuments)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.StandardId)
                                    .Terms(matchedIds.Documents.Select(x => x.StandardCode)))));

            if (providerStandards.ApiCall.HttpStatusCode != 200)
            {
                _applicationLogger.Warn($"httpStatusCode was {providerStandards.ApiCall.HttpStatusCode} when querying provider standards apprenticeship details for ukprn [{ukprn}]");
                throw new ApplicationException($"Failed to query provider standards apprenticeship details for ukprn [{ukprn}]");
            }

           return providerStandards.Documents;
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
