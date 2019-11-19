using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Nest;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using SFA.DAS.Apprenticeships.Api.Types.V4;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch
{
    public class ElasticsearchProviderLocationSearchProviderV4 : IGetProviderApprenticeshipLocationsV4
    {
        private const string NationalProviderAggregateName = "national_provider";
        private const string LocationCollapsedInnerHitsGroupName = "collapsed-locations";
        private readonly IConfigurationSettings _applicationSettings;
        private readonly IElasticsearchCustomClient _elasticsearchCustomClient;

        public ElasticsearchProviderLocationSearchProviderV4(
            IConfigurationSettings applicationSettings,
            IElasticsearchCustomClient elasticsearchCustomClient)
        {
            _applicationSettings = applicationSettings;
            _elasticsearchCustomClient = elasticsearchCustomClient;
        }

        public UniqueProviderApprenticeshipLocationSearchResult SearchStandardProviderLocations(int standardId, Coordinate coordinates, int actualPage, int pageSize, bool showForNonLevyOnly, bool showNationalOnly)
        {
            var qryStr = CreateUniqueProviderLocationQuery<StandardProviderSearchResultsItem>(x => x.StandardCode, standardId.ToString(), coordinates, showForNonLevyOnly, showNationalOnly);
            return PerformStandardProviderLocationSearchWithQuery(qryStr, actualPage, pageSize);
        }

        public UniqueProviderApprenticeshipLocationSearchResult SearchFrameworkProvidersLocations(string frameworkId, Coordinate coordinates, int actualPage, int pageSize, bool showForNonLevyOnly, bool showNationalOnly)
        {
            var qryStr = CreateUniqueProviderLocationQuery<FrameworkProviderSearchResultsItem>(x => x.FrameworkId, frameworkId, coordinates, showForNonLevyOnly, showNationalOnly);
            return PerformFrameworkProviderLocationSearchWithQuery(qryStr, actualPage, pageSize);
        }

        public ProviderLocationsSearchResults GetClosestLocationsForStandard(long ukprn, int standardId, Coordinate coordinates, int page, int pageSize, bool showForNonLevyOnly)
        {
            var qryStr = CreateClosestLocationsQuery<StandardProviderSearchResultsItem>(x => x.StandardCode, standardId.ToString(), ukprn.ToString(), coordinates, showForNonLevyOnly);
            return PerformClosestLocationsQuery(qryStr, page, pageSize);
        }

        public ProviderLocationsSearchResults GetClosestLocationsForFramework(long ukprn, string frameworkId, Coordinate coordinates, int page, int pageSize, bool showForNonLevyOnly)
        {
            var qryStr = CreateClosestLocationsQuery<FrameworkProviderSearchResultsItem>(x => x.FrameworkId, frameworkId, ukprn.ToString(), coordinates, showForNonLevyOnly);
            return PerformClosestLocationsQuery(qryStr, page, pageSize);
        }

        private SearchDescriptor<T> CreateClosestLocationsQuery<T>(Expression<Func<T, object>> selector, string code, string ukprn, Coordinate location, bool showForNonLevyOnly)
            where T : class, IApprenticeshipProviderSearchResultsItem
        {
            var descriptor =
            new SearchDescriptor<T>()
                .Index(_applicationSettings.ProviderIndexAlias)
                .Query(q => 
                    +q.Term(t => t.Field(f => f.Ukprn).Value(ukprn)) &&
                    +q.Term(t => t.Field(selector).Value(code)) &&
                    +q.Term(t => t.Field(f => f.HasNonLevyContract).Value(showForNonLevyOnly)) &&
                    +q.Nested(n => n.InnerHits(h => h
                        .Size(1).Sort(NestedSortByDistanceFromGivenLocation<T>(location))).Path(p => p.TrainingLocations).Query(q1 => q1.Bool(b => b.Filter(FilterByLocation<T>(location))))))
                .Sort(s => s.GeoDistance(GetGeoDistanceSearch<T>(location)));

            return descriptor;
        }

        private ProviderLocationsSearchResults PerformClosestLocationsQuery<T>(SearchDescriptor<T> qryStr, int page, int pageSize)
        where T: class, IApprenticeshipProviderSearchResultsItem
        {
            var skipAmount = pageSize * (page - 1);

            var results = _elasticsearchCustomClient.Search<T>(_ => qryStr.Skip(skipAmount).Take(pageSize));

            if (results.ApiCall?.HttpStatusCode != 200)
            {
                return new ProviderLocationsSearchResults();
            }

            return MapToProviderLocationsSearchResults(results, page, pageSize);
        }

        private static ProviderLocationsSearchResults MapToProviderLocationsSearchResults(ISearchResponse<IApprenticeshipProviderSearchResultsItem> searchResponse, int page, int pageSize)
        {
            var result = new ProviderLocationsSearchResults
            {
                TotalResults = searchResponse.HitsMetadata?.Total.Value ?? 0,
                PageNumber = page,
                PageSize = pageSize,
                Results = searchResponse.Hits?.Select(MapHitToProviderLocationsSearchResultsItem)
                
            };

            return result;
        }

        private static ProviderLocationsSearchResultsItem MapHitToProviderLocationsSearchResultsItem(IHit<IApprenticeshipProviderSearchResultsItem> hit)
        {
            return new ProviderLocationsSearchResultsItem
            {
                Distance = hit.Sorts != null ? Math.Round(double.Parse(hit.Sorts.DefaultIfEmpty(0).Last().ToString()), 1) : 0,
                Location = hit.InnerHits.First().Value.Hits.Hits.First().Source.As<SFA.DAS.Apprenticeships.Api.Types.V4.TrainingLocation>()
            };
        }

        private UniqueProviderApprenticeshipLocationSearchResult PerformStandardProviderLocationSearchWithQuery(SearchDescriptor<StandardProviderSearchResultsItem> qryStr, int page, int pageSize)
        {
            var skipAmount = pageSize * (page - 1);

            var results = _elasticsearchCustomClient.Search<StandardProviderSearchResultsItem>(_ => qryStr.Skip(skipAmount).Take(pageSize));

            if (results.ApiCall?.HttpStatusCode != 200)
            {
                return new UniqueProviderApprenticeshipLocationSearchResult();
            }

            return MapToUniqueProviderApprenticeshipLocationSearchResult(results, page, pageSize);
        }

        private UniqueProviderApprenticeshipLocationSearchResult PerformFrameworkProviderLocationSearchWithQuery(SearchDescriptor<FrameworkProviderSearchResultsItem> qryStr, int page, int pageSize)
        {
            var skipAmount = pageSize * (page - 1);

            var results = _elasticsearchCustomClient.Search<FrameworkProviderSearchResultsItem>(_ => qryStr.Skip(skipAmount).Take(pageSize));

            if (results.ApiCall?.HttpStatusCode != 200)
            {
                return new UniqueProviderApprenticeshipLocationSearchResult();
            }

            return MapToUniqueProviderApprenticeshipLocationSearchResult(results, page, pageSize);
        }

        private SearchDescriptor<T> CreateUniqueProviderLocationQuery<T>(Expression<Func<T, object>> selector, string code, Coordinate location, bool showForNonLevyOnly, bool showNationalOnly)
            where T : class, IApprenticeshipProviderSearchResultsItem
        {
            var descriptor =
                new SearchDescriptor<T>()
                    .Index(_applicationSettings.ProviderIndexAlias)
                    .Query(q => 
                        +q.Term(t => t.Field(selector).Value(code)) &&
                        +q.Term(t => t.Field(f => f.HasNonLevyContract).Value(showForNonLevyOnly)) &&
                        +q.Nested(n => n.InnerHits(h => h
                            .Size(1).Sort(NestedSortByDistanceFromGivenLocation<T>(location))).Path(p => p.TrainingLocations).Query(q1 => q1.Bool(b => b.Filter(FilterByLocation<T>(location))))))
                    .Collapse(c => c.Field(f => f.Ukprn).InnerHits(ih => ih.Name(LocationCollapsedInnerHitsGroupName)))
                    .Sort(s => s.GeoDistance(GetGeoDistanceSearch<T>(location)))
                    .Aggregations(agg => agg
                        .Terms(NationalProviderAggregateName, tt => tt.Field(fi => fi.NationalProvider).MinimumDocumentCount(0)))
                    .PostFilter(pf => {
                        if (showNationalOnly)
                        {
                            return pf.Bool(b => b
                            .Filter(
                                f => f
                                .Term(t => t
                                    .Field(x => x.NationalProvider)
                                    .Value(showNationalOnly))));
                        }

                        return pf;
                    });

            return descriptor;
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

        private static UniqueProviderApprenticeshipLocationSearchResult MapToUniqueProviderApprenticeshipLocationSearchResult(ISearchResponse<StandardProviderSearchResultsItem> searchResponse, int page, int pageSize)
        {
            var nationalProvidersAggregation = RetrieveAggregationElements(searchResponse.Aggregations.Terms(NationalProviderAggregateName), useKeyAsString: true);

            var result = new UniqueProviderApprenticeshipLocationSearchResult
            {
                TotalResults = searchResponse.HitsMetadata?.Total.Value ?? 0,
                PageNumber = page,
                PageSize = pageSize,
                Results = searchResponse.Hits?.Select(MapHitToUniqueProviderSearchResultItem),
                HasNationalProviders = nationalProvidersAggregation["true"] > 0
            };

            return result;
        }

        private static UniqueProviderApprenticeshipLocationSearchResult MapToUniqueProviderApprenticeshipLocationSearchResult(ISearchResponse<FrameworkProviderSearchResultsItem> searchResponse, int page, int pageSize)
        {
            var nationalProvidersAggregation = RetrieveAggregationElements(searchResponse.Aggregations.Terms(NationalProviderAggregateName), useKeyAsString: true);

            var result = new UniqueProviderApprenticeshipLocationSearchResult
            {
                TotalResults = searchResponse.HitsMetadata?.Total.Value ?? 0,
                PageNumber = page,
                PageSize = pageSize,
                Results = searchResponse.Hits?.Select(MapHitToUniqueProviderSearchResultItem),
                HasNationalProviders = nationalProvidersAggregation["true"] > 0
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

        private static UniqueProviderApprenticeshipLocationSearchResultItem MapHitToUniqueProviderSearchResultItem(IHit<IApprenticeshipProviderSearchResultsItem> hit)
        {
            return new UniqueProviderApprenticeshipLocationSearchResultItem
            {
                Ukprn = hit.Source.Ukprn,
                Location = hit.InnerHits["trainingLocations"].Hits.Hits.First().Source.As<SFA.DAS.Apprenticeships.Api.Types.V4.TrainingLocation>(),
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
                IsHigherEducationInstitute = hit.Source.IsHigherEducationInstitute,
                HasOtherMatchingLocations = (int)hit.InnerHits[LocationCollapsedInnerHitsGroupName].Hits.Total.Value > 1
            };
        }
    }
}
