﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Nest;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
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

        public ProviderApprenticeshipLocationSearchResult SearchStandardProviders(int standardId, Coordinate coordinates, int page, int pageSize, bool showForNonLevyOnly, bool showNationalOnly, List<DeliveryMode> deliverModes, int orderBy = 0)
        {
            var qryStr = CreateStandardProviderSearchQuery(standardId.ToString(), coordinates, showForNonLevyOnly, showNationalOnly, deliverModes, orderBy);
            return PerformStandardProviderSearchWithQuery(qryStr, page, pageSize);
        }

        public ProviderApprenticeshipLocationSearchResult SearchFrameworkProviders(string frameworkId, Coordinate coordinates, int page, int pageSize, bool showForNonLevyOnly, bool showNationalOnly, List<DeliveryMode> deliverModes, int orderBy = 0)
        {
            var qryStr = CreateFrameworkProviderSearchQuery(frameworkId, coordinates, showForNonLevyOnly, showNationalOnly, deliverModes, orderBy);
            return PerformFrameworkProviderSearchWithQuery(qryStr, page, pageSize);
        }

        private SearchDescriptor<StandardProviderSearchResultsItem> CreateStandardProviderSearchQuery(string standardId, Coordinate coordinates, bool showForNonLevyOnly, bool showNationalOnly, List<DeliveryMode> deliverModes, int orderBy)
        {
            return CreateProviderQuery<StandardProviderSearchResultsItem>(x => x.StandardCode, standardId, coordinates, showForNonLevyOnly, showNationalOnly, deliverModes, orderBy);
        }

        private SearchDescriptor<FrameworkProviderSearchResultsItem> CreateFrameworkProviderSearchQuery(string frameworkId, Coordinate coordinates, bool showForNonLevyOnly, bool showNationalOnly, List<DeliveryMode> deliverModes, int orderBy)
        {
            return CreateProviderQuery<FrameworkProviderSearchResultsItem>(x => x.FrameworkId, frameworkId, coordinates, showForNonLevyOnly, showNationalOnly, deliverModes, orderBy);
        }

        private ProviderApprenticeshipLocationSearchResult PerformStandardProviderSearchWithQuery(SearchDescriptor<StandardProviderSearchResultsItem> qryStr, int page, int pageSize)
        {
            var skipAmount = pageSize * (page - 1);

            var results = _elasticsearchCustomClient.Search<StandardProviderSearchResultsItem>(_ => qryStr.Skip(skipAmount).Take(pageSize));

            if (results.ApiCall?.HttpStatusCode != 200)
            {
                return new ProviderApprenticeshipLocationSearchResult();
            }

            return MapToProviderApprenticeshipLocationSearchResult(results, page, pageSize);
        }

        private ProviderApprenticeshipLocationSearchResult PerformFrameworkProviderSearchWithQuery(SearchDescriptor<FrameworkProviderSearchResultsItem> qryStr, int page, int pageSize)
        {
            var skipAmount = pageSize * (page - 1);

            var results = _elasticsearchCustomClient.Search<FrameworkProviderSearchResultsItem>(_ => qryStr.Skip(skipAmount).Take(pageSize));

            if (results.ApiCall?.HttpStatusCode != 200)
            {
                return new ProviderApprenticeshipLocationSearchResult();
            }

            return MapToProviderApprenticeshipLocationSearchResult(results, page, pageSize);
        }

        private SearchDescriptor<T> CreateProviderQuery<T>(Expression<Func<T, object>> selector, string code, Coordinate location, bool showForNonLevyOnly, bool showNationalOnly, List<DeliveryMode> deliveryModes, int orderBy)
            where T : class, IApprenticeshipProviderSearchResultsItem
        {
            var descriptor =
                new SearchDescriptor<T>()
                    .Index(_applicationSettings.ProviderIndexAlias)
                    .Query(q => q
                        .Bool(ft => ft
                            .Filter(GenerateFilters(selector, code, showForNonLevyOnly))
                            .Must(NestedLocationsQuery<T>(location))))
                    .Sort(GetSortDescriptor<T>(orderBy, location))
                    .Aggregations(GetProviderSearchAggregationsSelector<T>())
                    .PostFilter(pf => GeneratePostFilter(pf, deliveryModes?.Select(x => x.GetMemberDescription() ?? string.Empty), showNationalOnly));

            return descriptor;
        }

        private static QueryContainer GeneratePostFilter<T>(QueryContainerDescriptor<T> descriptor, IEnumerable<string> deliveryModes, bool showNationalOnly)
            where T : class, IApprenticeshipProviderSearchResultsItem
        {
            if (deliveryModes == null || !deliveryModes.Any())
            {
                return descriptor;
            }

            if (showNationalOnly)
            {
                return descriptor.Bool(b => b
                .Filter(
                    f => f
                    .Terms(t => t
                        .Field(x => x.DeliveryModesKeywords)
                        .Terms(deliveryModes)),
                    f => f
                    .Term(t => t
                        .Field(x => x.NationalProvider)
                        .Value(showNationalOnly))));
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
                .Terms(NationalProviderAggregateName, tt => tt.Field(fi => fi.NationalProvider).MinimumDocumentCount(0));
        }

        private static IEnumerable<Func<QueryContainerDescriptor<T>, QueryContainer>> GenerateFilters<T>(Expression<Func<T, object>> selector, string apprenticeshipIdentifier, bool showForNonLevyOnly)
            where T : class, IApprenticeshipProviderSearchResultsItem
        {
            yield return f => f.Term(t => t.Field(selector).Value(apprenticeshipIdentifier));

            yield return f => f.Term(t => t.Field(fi => fi.HasNonLevyContract).Value(showForNonLevyOnly));
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
                .Points(new GeoLocation(location.Lat, location.Lon))
                .Unit(DistanceUnit.Miles)
                .Ascending());
        }

        private static Func<QueryContainerDescriptor<T>, QueryContainer> FilterByLocation<T>(Coordinate location)
            where T : class, IApprenticeshipProviderSearchResultsItem
        {
            return f => f.GeoShape(gp => gp
                            .Field(fd => fd.TrainingLocations.First().Location)
                            .Shape(s => s.Point(new GeoCoordinate(location.Lat, location.Lon))));
        }

        private static Func<SortDescriptor<T>, IPromise<IList<ISort>>> GetSortDescriptor<T>(int orderBy, Coordinate location)
             where T : class, IApprenticeshipProviderSearchResultsItem
        {
            var sortDescriptor = new SortDescriptor<T>();

            if (orderBy == 1)
            {
                sortDescriptor.Field(f => f.Field(fd => fd.ProviderName.Suffix("keyword")).Ascending());
            }

            if (orderBy == 2)
            {
                sortDescriptor.Field(f => f.Field(fd => fd.ProviderName.Suffix("keyword")).Descending());
            }

            return s => sortDescriptor.GeoDistance(GetGeoDistanceSearch<T>(location));
        }

        private static Func<GeoDistanceSortDescriptor<T>, IGeoDistanceSort> GetGeoDistanceSearch<T>(Coordinate location)
            where T : class, IApprenticeshipProviderSearchResultsItem
        { 
            return g => g
                .Nested(x => x.Path(p => p.TrainingLocations))
                .Field(fd => fd.TrainingLocations.First().LocationPoint)
                .Points(new GeoLocation(location.Lat, location.Lon))
                .Unit(DistanceUnit.Miles)
                .Ascending();
        }

        private static ProviderApprenticeshipLocationSearchResult MapToProviderApprenticeshipLocationSearchResult(ISearchResponse<StandardProviderSearchResultsItem> searchResponse, int page, int pageSize)
        {
            var trainingOptionsAggregation = RetrieveAggregationElements(searchResponse.Aggregations.Terms(TrainingTypeAggregateName));
            var nationalProvidersAggregation = RetrieveAggregationElements(searchResponse.Aggregations.Terms(NationalProviderAggregateName), useKeyAsString: true);

            var result = new ProviderApprenticeshipLocationSearchResult
            {
                TotalResults = searchResponse.HitsMetadata?.Total.Value ?? 0,
                PageNumber = page,
                PageSize = pageSize,
                Results = searchResponse.Hits?.Select(MapHitToProviderSearchResultItem),
                TrainingOptionsAggregation = trainingOptionsAggregation,
                NationalProvidersAggregation = nationalProvidersAggregation
            };

            return result;
        }

        private static ProviderApprenticeshipLocationSearchResult MapToProviderApprenticeshipLocationSearchResult(ISearchResponse<FrameworkProviderSearchResultsItem> searchResponse, int page, int pageSize)
        {
            var trainingOptionsAggregation = RetrieveAggregationElements(searchResponse.Aggregations.Terms(TrainingTypeAggregateName));
            var nationalProvidersAggregation = RetrieveAggregationElements(searchResponse.Aggregations.Terms(NationalProviderAggregateName), useKeyAsString: true);

            var result = new ProviderApprenticeshipLocationSearchResult
            {
                TotalResults = searchResponse.HitsMetadata?.Total.Value ?? 0,
                PageNumber = page,
                PageSize = pageSize,
                Results = searchResponse.Hits?.Select(MapHitToProviderSearchResultItem),
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

        private static ProviderSearchResultItem MapHitToProviderSearchResultItem(IHit<IApprenticeshipProviderSearchResultsItem> hit)
        {
            return new ProviderSearchResultItem
            {
                Ukprn = hit.Source.Ukprn,
                Location = hit.InnerHits.First().Value.Hits.Hits.First().Source.As<SFA.DAS.Apprenticeships.Api.Types.V3.TrainingLocation>(),
                ProviderName = hit.Source.ProviderName,
                OverallAchievementRate = hit.Source.OverallAchievementRate,
                NationalProvider = hit.Source.NationalProvider,
                DeliveryModes = hit.Source.DeliveryModes,
                Distance = hit.Sorts != null ? Math.Round(double.Parse(hit.Sorts.DefaultIfEmpty(0).Last().ToString()), 1) : 0,
                EmployerSatisfaction = hit.Source.EmployerSatisfaction,
                LearnerSatisfaction = hit.Source.LearnerSatisfaction,
                NationalOverallAchievementRate = hit.Source.NationalOverallAchievementRate,
                OverallCohort = hit.Source.OverallCohort,
                HasNonLevyContract = hit.Source.HasNonLevyContract,
                IsLevyPayerOnly = hit.Source.IsLevyPayerOnly,
                CurrentlyNotStartingNewApprentices = hit.Source.CurrentlyNotStartingNewApprentices,
                IsHigherEducationInstitute = hit.Source.IsHigherEducationInstitute
            };
        }
    }
}
