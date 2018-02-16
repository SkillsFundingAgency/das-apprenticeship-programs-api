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
        private readonly IActiveApprenticeshipChecker _activeApprenticeshipChecker;
        private readonly IPaginationHelper _paginationHelper;

        public ProviderRepository(
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

            if (results.Documents.Count > 1)
            {
                _applicationLogger.Debug($"found {results.Documents.Count} providers for the ukprn {ukprn}");
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

            if (results.Documents.Count > 1)
            {
                _applicationLogger.Debug($"found {results.Documents.Count} providers for the ukprns provided");
            }

            return results.Documents;
        }

        public IEnumerable<ProviderFramework> GetFrameworksByProviderUkprn(long ukprn)
        {
            var totalTakeForFrameworkDocuments = _queryHelper.GetFrameworksTotalAmount();
            var aggregateKeyName = "levelId";
            var matchedIds =
                _elasticsearchCustomClient.Search<ProviderFrameworkDto>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .Type(Types.Parse("frameworkprovider"))
                            .From(0)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.Ukprn)
                                    .Terms(ukprn)))
                            .Aggregations(agg => agg.Terms(aggregateKeyName, frm => frm.Size(totalTakeForFrameworkDocuments).Field(fi => fi.FrameworkId))));

            if (matchedIds.ApiCall.HttpStatusCode != 200)
            {
                _applicationLogger.Warn($"httpStatusCode was {matchedIds.ApiCall.HttpStatusCode} when querying provider frameworks for ukprn [{ukprn}]");
                throw new ApplicationException($"Failed to query provider frameworks for ukprn [{ukprn}]");
            }

            var matchingFrameworkIds = ExtractFrameworkIdsFromAggregation(matchedIds, aggregateKeyName);

            var providerFrameworks =
                _elasticsearchCustomClient.Search<ProviderFramework>(
                    s =>
                        s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                            .Type(Types.Parse("frameworkdocument"))
                            .From(0)
                            .Take(matchingFrameworkIds.Count)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.FrameworkId)
                                    .Terms(matchingFrameworkIds))));

            if (providerFrameworks.ApiCall.HttpStatusCode != 200)
            {
                _applicationLogger.Warn($"httpStatusCode was {providerFrameworks.ApiCall.HttpStatusCode} when querying provider frameworks apprenticeship details for ukprn [{ukprn}]");
                throw new ApplicationException($"Failed to query provider frameworks apprenticeship details for ukprn [{ukprn}]");
            }

            return providerFrameworks.Documents;
        }

        public IEnumerable<ProviderStandard> GetStandardsByProviderUkprn(long ukprn)
        {
            var totalTakeForStandardDocuments = _queryHelper.GetStandardsTotalAmount();
            var aggregateKeyName = "levelId";
            var matchedIds =
                _elasticsearchCustomClient.Search<ProviderStandardDto>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .Type(Types.Parse("standardprovider"))
                            .From(0)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.Ukprn)
                                    .Terms(ukprn)))
                                .Aggregations(agg => agg.Terms(
                                    aggregateKeyName,
                                frm => frm.Size(totalTakeForStandardDocuments).Field(fi => fi.StandardCode))));

            if (matchedIds.ApiCall.HttpStatusCode != 200)
            {
                 _applicationLogger.Warn($"httpStatusCode was {matchedIds.ApiCall.HttpStatusCode} when querying provider standards for ukprn [{ukprn}]");
                 throw new ApplicationException($"Failed to query provider standards for ukprn [{ukprn}]");
            }

            var matchingStandardIds = ExtractStandardCodesFromAggregation(matchedIds, aggregateKeyName);

            var providerStandards =
                _elasticsearchCustomClient.Search<ProviderStandard>(
                    s =>
                        s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                            .Type(Types.Parse("standarddocument"))
                            .From(0)
                            .Take(matchingStandardIds.Count)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.StandardId)
                                    .Terms(matchingStandardIds))));

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

            if (results.Documents.Count > 1)
            {
                _applicationLogger.Warn($"found {results.Documents.Count} providers for the standard {standardId}");
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

            if (results.Documents.Count > 1)
            {
                _applicationLogger.Warn($"found {results.Documents.Count} providers for the framework {frameworkId}");
            }

            return results.Documents;
        }

        public ApprenticeshipTrainingSummary GetActiveApprenticeshipTrainingByProvider(long ukprn, int page)
        {
             var apprenticeshipTrainingSummary = new ApprenticeshipTrainingSummary { Ukprn = ukprn };

            var apprenticeships = new List<ApprenticeshipTraining>();
            apprenticeships.AddRange(GetActiveStandardsForUkprn(ukprn));
            apprenticeships.AddRange(GetActiveFrameworksForUkprn(ukprn));

            var totalCount = apprenticeships.Count;

            var pageSize = _applicationSettings.PageSizeApprenticeshipSummary;
            var paginationDetails = _paginationHelper.GeneratePaginationDetails(page, pageSize, totalCount);
            apprenticeshipTrainingSummary.PaginationDetails = paginationDetails;

            apprenticeshipTrainingSummary.ApprenticeshipTrainingItems
                = apprenticeships.OrderBy(x => x.Name)
                    .ThenBy(x => x.Level)
                    .Skip(paginationDetails.NumberOfRecordsToSkip)
                    .Take(pageSize);

           return apprenticeshipTrainingSummary;
        }

        private static List<string> ExtractFrameworkIdsFromAggregation(ISearchResponse<ProviderFrameworkDto> matchedIds, string levelId)
        {

            if (matchedIds.Aggregations == null) { return new List<string>(); }

            var matchedList = ((BucketAggregate)matchedIds.Aggregations[levelId]).Items;
            return GetKeysFromAggregate(matchedList);
        }

        private static List<string> ExtractStandardCodesFromAggregation(ISearchResponse<ProviderStandardDto> matchedIds, string levelId)
        {
            if (matchedIds.Aggregations == null) { return new List<string>(); }
            var matchedList = ((BucketAggregate)matchedIds.Aggregations[levelId]).Items;
            return GetKeysFromAggregate(matchedList);
        }

        private static List<string> GetKeysFromAggregate(IReadOnlyCollection<IBucket> matchedList)
        {
            if (matchedList == null) { return new List<string>(); }

            var matchedDetails = new List<string>();
            using (var matchedEnumerator = matchedList.GetEnumerator())
            {
                while (matchedEnumerator.MoveNext())
                {
                    if (matchedEnumerator.Current != null)
                    {
                        var res = ((KeyedBucket<object>)matchedEnumerator.Current).Key.ToString();
                        matchedDetails.Add(res);
                    }
                }
            }

            return matchedDetails;
        }

        private IEnumerable<ApprenticeshipTraining> GetActiveFrameworksForUkprn(long ukprn)
        {
            var frameworks = GetFrameworksByProviderUkprn(ukprn);

            return frameworks
                .Where(x => _activeApprenticeshipChecker.CheckActiveFramework(x.FrameworkId, x.EffectiveFrom, x.EffectiveTo))
                .Select(framework => new ApprenticeshipTraining
                {
                    Name = framework.PathwayName,
                    Level = framework.Level,
                    Type = ApprenticeshipTrainingType.Framework.ToString(),
                    TrainingType = ApprenticeshipTrainingType.Framework,
                    Identifier = framework.FrameworkId
                })
                .ToList();
        }

        private IEnumerable<ApprenticeshipTraining> GetActiveStandardsForUkprn(long ukprn)
        {
            var standards = GetStandardsByProviderUkprn(ukprn);

            return standards
                .Where(x => _activeApprenticeshipChecker.CheckActiveStandard(x.StandardId.ToString(), x.EffectiveFrom, x.EffectiveTo))
                .Select(standard => new ApprenticeshipTraining
                {
                    Name = standard.Title,
                    Level = standard.Level,
                    Type = ApprenticeshipTrainingType.Standard.ToString(),
                    TrainingType = ApprenticeshipTrainingType.Standard,
                    Identifier = standard.StandardId.ToString()
                })
                .ToList();
        }
     }
}
