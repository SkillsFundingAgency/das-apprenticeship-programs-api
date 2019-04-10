using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Nest;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch.V3;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Helpers;
using SFA.DAS.Apprenticeships.Api.Types.V3;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch
{
    public class ElasticsearchProviderLocationSearchProviderV3 : IGetProviderApprenticeshipLocationsV3
    {
        private const string TrainingTypeAggregateName = "training_type";
        private const string NationalProviderAggregateName = "national_provider";
        private readonly IConfigurationSettings _applicationSettings;
        private readonly IElasticsearchCustomClient _elasticsearchCustomClient;

        public ElasticsearchProviderLocationSearchProviderV3(
            IConfigurationSettings applicationSettings,
            IElasticsearchCustomClient elasticsearchCustomClient)
        {
            _applicationSettings = applicationSettings;
            _elasticsearchCustomClient = elasticsearchCustomClient;
        }

        public StandardProviderSearchResult SearchStandardProviders(int standardId, Coordinate coordinates, int page, int pageSize, bool showForNonLevyOnly, bool showNationalOnly, List<DeliveryMode> deliverModes)
        {
            var qryStr = CreateStandardProviderSearchQuery(standardId.ToString(), coordinates, showForNonLevyOnly, showNationalOnly, deliverModes);
            return PerformStandardProviderSearchWithQuery(qryStr, page, pageSize);
        }

        private SearchDescriptor<StandardProviderSearchResultsItem> CreateStandardProviderSearchQuery(string standardId, Coordinate coordinates, bool showForNonLevyOnly, bool showNationalOnly, List<DeliveryMode> deliverModes)
        {
            return CreateProviderQuery<StandardProviderSearchResultsItem>(x => x.StandardCode, standardId, coordinates, showForNonLevyOnly, showNationalOnly, deliverModes);
        }

        private StandardProviderSearchResult PerformStandardProviderSearchWithQuery(SearchDescriptor<StandardProviderSearchResultsItem> qryStr, int page, int pageSize)
        {
            var skipAmount = pageSize * (page - 1);

            var results = _elasticsearchCustomClient.Search<StandardProviderSearchResultsItem>(_ => qryStr.Skip(skipAmount).Take(pageSize));

            if (results.ApiCall?.HttpStatusCode != 200)
            {
                return new StandardProviderSearchResult();
            }

            return MapToStandardProviderSearchResult(results, page, pageSize);
        }

        private SearchDescriptor<T> CreateProviderQuery<T>(Expression<Func<T, object>> selector, string code, Coordinate location, bool showForNonLevyOnly, bool showNationalOnly, List<DeliveryMode> deliveryModes)
            where T : class, IApprenticeshipProviderSearchResultsItem
        {
            var lee = Enumerable.Empty<string>();
            var descriptor =
                new SearchDescriptor<T>()
                    .Index(_applicationSettings.ProviderIndexAlias)
                    .Query(q => q
                        .Bool(ft => ft
                            .Filter(GenerateFilters(selector, code, showForNonLevyOnly, showNationalOnly, deliveryModes))
                            .Must(NestedLocationsQuery<T>(location))))
                    .Sort(SortByDistanceFromGivenLocation<T>(location))
                    .Aggregations(GetProviderSearchAggregationsSelector<T>())
                    .PostFilter(pf => FilterByDeliveryModes(pf, deliveryModes?.Select(x => x.GetMemberDescription() ?? string.Empty)));

            return descriptor;
        }

        private static QueryContainer FilterByDeliveryModes<T>(QueryContainerDescriptor<T> descriptor, IEnumerable<string> deliveryModes)
            where T : class, IApprenticeshipProviderSearchResultsItem
        {
            if (deliveryModes == null || !deliveryModes.Any())
            {
                return descriptor;
            }

            return descriptor
                .Terms(t => t
                    .Field(x => x.DeliveryModesKeywords)
                    .Terms(deliveryModes));
        }

        private Func<AggregationContainerDescriptor<T>, IAggregationContainer> GetProviderSearchAggregationsSelector<T>()
            where T : class, IApprenticeshipProviderSearchResultsItem
        {
            return aggs => aggs
                .Terms(TrainingTypeAggregateName, tt => tt.Field(fi => fi.DeliveryModesKeywords).MinimumDocumentCount(0))
                .Terms(NationalProviderAggregateName, tt => tt.Field(fi => fi.NationalProvider));
        }

        private static IEnumerable<Func<QueryContainerDescriptor<T>, QueryContainer>> GenerateFilters<T>(Expression<Func<T, object>> selector, string apprenticeshipIdentifier, bool showForNonLevyOnly, bool showNationalOnly, List<DeliveryMode> deliveryModes)
            where T : class, IApprenticeshipProviderSearchResultsItem
        {
            yield return f => f.Term(t => t.Field(selector).Value(apprenticeshipIdentifier));

            yield return f => f.Term(t => t.Field(fi => fi.HasNonLevyContract).Value(showForNonLevyOnly));

            if (showNationalOnly)
            {
                yield return f => f.Term(t => t.Field(fi => fi.NationalProvider).Value(showNationalOnly));
            }

            if (deliveryModes != null && deliveryModes.Count > 0)
            {
                yield return f => f.Terms(t => t.Field(fi => fi.DeliveryModes).Terms(deliveryModes.Select(x => x.GetMemberDescription())));
            }
        }

        private Func<QueryContainerDescriptor<T>, QueryContainer> NestedLocationsQuery<T>(Coordinate location)
            where T : class, IApprenticeshipProviderSearchResultsItem
        {
            return f => f.Nested(n => n
                .InnerHits(h => h
                    .Size(1)
                    .Sort(NestedSortByDistanceFromGivenLocation<T>(location)))
                .Path(p => p.TrainingLocations)
                .Query(q => q
                    .Bool(b => b
                        .Filter(FilterByLocation<T>(location)))));
        }

        private static Func<SortDescriptor<T>, IPromise<IList<ISort>>> NestedSortByDistanceFromGivenLocation<T>(Coordinate location)
            where T : class, IApprenticeshipProviderSearchResultsItem
        {
            return f => f.GeoDistance(g => g
                .Field(fd => fd.TrainingLocations.First().LocationPoint)
                .PinTo(new GeoLocation(location.Lat, location.Lon))
                .Unit(DistanceUnit.Miles)
                .Ascending());
        }

        private static Func<QueryContainerDescriptor<T>, QueryContainer> FilterByLocation<T>(Coordinate location)
            where T : class, IApprenticeshipProviderSearchResultsItem
        {
            return f => f.GeoShapePoint(gp => gp.Field(fd => fd.TrainingLocations.First().Location).Coordinates(location.Lon, location.Lat));
        }

        private static Func<SortDescriptor<T>, IPromise<IList<ISort>>> SortByDistanceFromGivenLocation<T>(Coordinate location)
            where T : class, IApprenticeshipProviderSearchResultsItem
        {
            return f => f.GeoDistance(g => g
                .NestedPath(x => x.TrainingLocations)
                .Field(fd => fd.TrainingLocations.First().LocationPoint)
                .PinTo(new GeoLocation(location.Lat, location.Lon))
                .Unit(DistanceUnit.Miles)
                .Ascending());
        }

        private static StandardProviderSearchResult MapToStandardProviderSearchResult(ISearchResponse<StandardProviderSearchResultsItem> searchResponse, int page, int pageSize)
        {
            var trainingOptionsAggregation = RetrieveAggregationElements(searchResponse.Aggs.Terms(TrainingTypeAggregateName));
            var nationalProvidersAggregation = RetrieveAggregationElements(searchResponse.Aggs.Terms(NationalProviderAggregateName), useKeyAsString: true);

            var result = new StandardProviderSearchResult
            {
                TotalResults = searchResponse.HitsMetaData?.Total ?? 0,
                PageNumber = page,
                PageSize = pageSize,
                Results = searchResponse.Hits?.Select(MapHitToStandardProviderSearchResultItem),
                TrainingOptionsAggregation = trainingOptionsAggregation,
                NationalProvidersAggregation = nationalProvidersAggregation
            };

            return result;
        }

        private static Dictionary<string, long?> RetrieveAggregationElements(TermsAggregate<string> termsAggregate, bool useKeyAsString = false)
        {
            var aggregationResult = new Dictionary<string, long?>();

            string keySelector(KeyedBucket<string> b) => useKeyAsString ? b.KeyAsString : b.Key;

            if (termsAggregate.Buckets != null)
            {
                foreach (var item in termsAggregate.Buckets)
                {
                    aggregationResult.Add(keySelector(item), item.DocCount);
                }
            }

            return aggregationResult;
        }

        private static ProviderSearchResultItem MapHitToStandardProviderSearchResultItem(IHit<StandardProviderSearchResultsItem> hit)
        {
            return new ProviderSearchResultItem
            {
                Ukprn = hit.Source.Ukprn,
                Location = hit.InnerHits.First().Value.Hits.Hits.First().Source.As<SFA.DAS.Apprenticeships.Api.Types.V3.TrainingLocation>(),
                ProviderName = hit.Source.ProviderName,
                OverallAchievementRate = hit.Source.OverallAchievementRate,
                NationalProvider = hit.Source.NationalProvider,
                DeliveryModes = hit.Source.DeliveryModes,
                Distance = hit.Sorts != null ? Math.Round(double.Parse(hit.Sorts.DefaultIfEmpty(0).First().ToString()), 1) : 0,
                EmployerSatisfaction = hit.Source.EmployerSatisfaction,
                LearnerSatisfaction = hit.Source.LearnerSatisfaction,
                NationalOverallAchievementRate = hit.Source.NationalOverallAchievementRate,
                OverallCohort = hit.Source.OverallCohort,
                HasNonLevyContract = hit.Source.HasNonLevyContract,
                IsLevyPayerOnly = hit.Source.IsLevyPayerOnly,
                CurrentlyNotStartingNewApprentices = hit.Source.CurrentlyNotStartingNewApprentices
            };
        }
    }
}
