﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nest;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
using SFA.DAS.Apprenticeships.Api.Types.V3;
using ApprenticeshipSearchResultsItemV1 = SFA.DAS.Apprenticeships.Api.Types.ApprenticeshipSearchResultsItem;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch.V3
{
    public sealed class ApprenticeshipSearchServiceV3 : IApprenticeshipSearchServiceV3
    {
        private const string LevelAggregateName = "level";
        private readonly IElasticsearchCustomClient _elasticsearchCustomClient;
        private readonly IConfigurationSettings _applicationSettings;
        private readonly IQueryHelper _queryHelper;
        private readonly IApprenticeshipSearchResultsMapping _resultItemMapper;

        public ApprenticeshipSearchServiceV3(
            IElasticsearchCustomClient elasticsearchCustomClient,
            IConfigurationSettings applicationSettings,
            IQueryHelper queryHelper,
            IApprenticeshipSearchResultsMapping resultItemMapper)
        {
            _elasticsearchCustomClient = elasticsearchCustomClient;
            _applicationSettings = applicationSettings;
            _queryHelper = queryHelper;
            _resultItemMapper = resultItemMapper;
        }

        public ApprenticeshipSearchResults SearchApprenticeships(string keywords, int pageNumber, int pageSize = 20, int sortOrder = 0, IEnumerable<int> selectedLevels = null)
        {
            var formattedKeywords = _queryHelper.FormatKeywords(keywords);

            var searchDescriptor = GetSearchDescriptor(pageNumber, pageSize, formattedKeywords, sortOrder, selectedLevels ?? Enumerable.Empty<int>());

            var results = _elasticsearchCustomClient.Search<ApprenticeshipSearchResultsItemV1>(s => searchDescriptor);

            var levelAggregation = BuildLevelAggregationResult(results);

            return MapToApprenticeshipSearchResults(pageNumber, pageSize, results, levelAggregation);
        }

        public ApprenticeshipAutocompleteSearchResults GetCompletions(string searchString)
        {
            var searchDescriptor = new SearchDescriptor<ApprenticeshipSearchResultsItemV1>()
                .Index(_applicationSettings.ApprenticeshipIndexAlias)
                .Query(q => q
                    .Bool(b => b
                        .Filter(
                            AllTypesOfApprenticeship(),
                            PublishedApprenticeship(),
                            MustBeStartedApprenticeship(),
                            MustBeNonExpiredApprenticceship(),
                            MustBeNotPastLastDateForNewStartsApprenticceship())
                        .Must(m => m
                            .MultiMatch(mm => mm
                                    .Type(TextQueryType.MostFields)
                                    .Fields(fi => fi
                                        .Field("title.auto", 3)
                                        .Field("keywords.auto")
                                        .Field("jobRoles.auto")
                                        .Field("jobRoleItems.title.auto"))
                                    .Query(searchString)))));

            var results = _elasticsearchCustomClient.Search<ApprenticeshipSearchResultsItemV1>(s => searchDescriptor);
            return new ApprenticeshipAutocompleteSearchResults
            {
                Results = results.Documents.Select(doc => new ApprenticeshipAutocompleteSearchResultsItem { Title = doc.Title })
            };
        }

        private ApprenticeshipSearchResults MapToApprenticeshipSearchResults(
            int requestedPageNumber,
            int pageSize,
            ISearchResponse<ApprenticeshipSearchResultsItemV1> results,
            Dictionary<int, long?> levelAggregation)
        {
            var totalHits = results.HitsMetadata?.Total.Value ?? 0;

            return new ApprenticeshipSearchResults
            {
                TotalResults = totalHits,
                PageNumber = requestedPageNumber,
                PageSize = pageSize,
                Results = results.Documents.Select(_resultItemMapper.MapToApprenticeshipSearchResult),
                LevelAggregation = levelAggregation
            };
        }

        private static Dictionary<int, long?> BuildLevelAggregationResult(ISearchResponse<ApprenticeshipSearchResultsItemV1> results)
        {
            var levelAggregation = new Dictionary<int, long?>();

            if (results.Aggregations.Terms(LevelAggregateName) != null)
            {
                foreach (var item in results.Aggregations.Terms(LevelAggregateName).Buckets)
                {
                    int iKey;
                    if (int.TryParse(item.Key, out iKey))
                    {
                        levelAggregation.Add(iKey, item.DocCount);
                    }
                }
            }

            return levelAggregation;
        }

        private static QueryContainer FilterBySelectedLevels(
            QueryContainerDescriptor<ApprenticeshipSearchResultsItemV1> descriptor,
            IList<int> selectedLevels)
        {
            if (selectedLevels == null || selectedLevels.Count == 0)
            {
                return descriptor;
            }

            return descriptor
                .Terms(t => t
                    .Field(s => s.Level)
                    .Terms(selectedLevels));
        }

        private static Func<QueryContainerDescriptor<ApprenticeshipSearchResultsItemV1>, QueryContainer> MustBeNotPastLastDateForNewStartsApprenticceship()
        {
            return m1 => m1
                .Bool(mb1 => mb1
                    .Should(
                        bs1 => bs1
                                   .DateRange(r => r
                                       .GreaterThanOrEquals(DateTime.Today)
                                       .Field(f => f.LastDateForNewStarts))
                               || bs1
                                   .Bool(bsb1 => bsb1
                                       .MustNot(mn => mn
                                           .Exists(e => e
                                               .Field(f => f.LastDateForNewStarts))))));
        }

        private static Func<QueryContainerDescriptor<ApprenticeshipSearchResultsItemV1>, QueryContainer> AllTypesOfApprenticeship()
        {
            return f => f.Terms(t => t.Field("documentType").Terms<string>("frameworkdocument", "standarddocument"));
        }

        private static Func<QueryContainerDescriptor<ApprenticeshipSearchResultsItemV1>, QueryContainer> PublishedApprenticeship()
        {
            return f => f
                .Term(t => t
                    .Field(fi => fi.Published).Value(true));
        }

        private static Func<QueryContainerDescriptor<ApprenticeshipSearchResultsItemV1>, QueryContainer> MustBeNonExpiredApprenticceship()
        {
            return m1 => m1
                .Bool(mb1 => mb1
                    .Should(
                        bs1 => bs1
                                   .DateRange(r => r
                                       .GreaterThanOrEquals(DateTime.Now)
                                       .Field(f => f.EffectiveTo))
                               || bs1
                                   .Bool(bsb1 => bsb1
                                       .MustNot(mn => mn
                                           .Exists(e => e
                                               .Field(f => f.EffectiveTo))))));
        }

        private static Func<QueryContainerDescriptor<ApprenticeshipSearchResultsItemV1>, QueryContainer> MustBeStartedApprenticeship()
        {
            return m0 => m0
                .Bool(mb0 => mb0
                    .Should(
                        s0 => s0
                                  .DateRange(r => r
                                      .LessThanOrEquals(DateTime.Now)
                                      .Field(f => f.EffectiveFrom))
                              || s0
                                  .Bool(b2 => b2
                                      .MustNot(mn => mn
                                          .Exists(e => e
                                              .Field(f => f.EffectiveFrom))))));
        }

        private SearchDescriptor<ApprenticeshipSearchResultsItemV1> GetSearchDescriptor(
            int page, int take, string formattedKeywords, int order, IEnumerable<int> selectedLevels)
        {
            return formattedKeywords == "*"
                ? GetAllSearchDescriptor(page, take, formattedKeywords, order, selectedLevels)
                : GetKeywordSearchDescriptor(page, take, formattedKeywords, order, selectedLevels);
        }

        private SearchDescriptor<ApprenticeshipSearchResultsItemV1> GetAllSearchDescriptor(
            int page, int take, string formattedKeywords, int order, IEnumerable<int> selectedLevels)
        {
            var skip = (page - 1) * take;

            var searchDescriptor = new SearchDescriptor<ApprenticeshipSearchResultsItemV1>()
                .Index(_applicationSettings.ApprenticeshipIndexAlias)
                .Skip(skip)
                .Take(take)
                .Query(q => q
                    .Bool(b => b
                    .Filter(PublishedApprenticeship())
                    .Must(
                        MustBeStartedApprenticeship(),
                        MustBeNonExpiredApprenticceship(),
                        MustBeNotPastLastDateForNewStartsApprenticceship(),
                        m => m
                            .QueryString(qs => qs
                                .Fields(fs => fs
                                    .Field(f => f.Title)
                                    .Field(p => p.JobRoles)
                                    .Field(p => p.Keywords)
                                    .Field(p => p.FrameworkName)
                                    .Field(p => p.PathwayName)
                                    .Field(p => p.JobRoleItems.First().Title)
                                    .Field(p => p.JobRoleItems.First().Description))
                                .Query(formattedKeywords)))))
                .PostFilter(GetPostFilter(selectedLevels))
                .Aggregations(agg => agg
                    .Terms(LevelAggregateName, t => t
                        .Field(f => f.Level).MinimumDocumentCount(0)));

            GetSortingOrder(searchDescriptor, order);

            return searchDescriptor;
        }

        private Func<QueryContainerDescriptor<ApprenticeshipSearchResultsItemV1>, QueryContainer> GetPostFilter(IEnumerable<int> selectedLevels)
        {
            return m => FilterBySelectedLevels(m, selectedLevels.ToList());
        }

        private SearchDescriptor<ApprenticeshipSearchResultsItemV1> GetKeywordSearchDescriptor(
            int page, int take, string formattedKeywords, int order, IEnumerable<int> selectedLevels)
        {
            var skip = (page - 1) * take;
            var searchDescriptor = new SearchDescriptor<ApprenticeshipSearchResultsItemV1>()
                    .Index(_applicationSettings.ApprenticeshipIndexAlias)
                    .Skip(skip)
                    .Take(take)
                    .Query(q => q
                        .Bool(b => b
                            .Filter(PublishedApprenticeship())
                            .Must(
                                MustBeStartedApprenticeship(),
                                MustBeNonExpiredApprenticceship(),
                                MustBeNotPastLastDateForNewStartsApprenticceship(),
                                m => m
                                    .Bool(mb => mb
                                        .Should(
                                            bs1 => bs1
                                                .MultiMatch(mm => mm
                                                    .Type(TextQueryType.BestFields)
                                                    .Fields(fi => fi
                                                        .Field(f => f.Title)
                                                        .Field(f => f.Keywords)
                                                        .Field(f => f.JobRoles)
                                                        .Field(f => f.JobRoleItems.First().Title)
                                                        .Field(f => f.JobRoleItems.First().Description))
                                                    .Query(formattedKeywords)
                                                    .MinimumShouldMatch("2<70%")
                                                    .PrefixLength(3)),
                                            bs2 => bs2
                                                .MultiMatch(mm => mm
                                                    .Type(TextQueryType.CrossFields)
                                                    .Fields(fi => fi
                                                        .Field(f => f.Title, 2)
                                                        .Field(f => f.Keywords))
                                                    .Query(formattedKeywords)
                                                    .PrefixLength(3)))))))
                       .PostFilter(GetPostFilter(selectedLevels))
                        .Aggregations(agg => agg
                        .Terms(LevelAggregateName, t => t
                            .Field(f => f.Level).MinimumDocumentCount(0)));

            GetSortingOrder(searchDescriptor, order);

            return searchDescriptor;
        }

        private void GetSortingOrder(SearchDescriptor<ApprenticeshipSearchResultsItemV1> searchDescriptor, int order)
        {
            if (order == 0 || order == 1)
            {
                searchDescriptor.Sort(s => s
                    .Descending(SortSpecialField.Score)
                    .Descending(f => f.TitleKeyword)
                    .Descending(f => f.Level));
            }

            if (order == 2)
            {
                searchDescriptor.Sort(s => s.Descending(p => p.Level));
            }

            if (order == 3)
            {
                searchDescriptor.Sort(s => s.Ascending(p => p.Level));
            }
        }
    }
}
