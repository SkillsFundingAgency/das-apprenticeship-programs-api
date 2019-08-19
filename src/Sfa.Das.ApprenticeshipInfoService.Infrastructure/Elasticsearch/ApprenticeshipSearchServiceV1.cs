using System;
using System.Collections.Generic;
using FeatureToggle.Core.Fluent;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.FeatureToggles;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch
{
    using System.Linq;
    using Core.Configuration;
    using Core.Models;
    using Core.Services;
    using Mapping;
    using Nest;

    public sealed class ApprenticeshipSearchServiceV1 : IApprenticeshipSearchServiceV1
    {
        private readonly IElasticsearchCustomClient _elasticsearchCustomClient;
        private readonly IConfigurationSettings _applicationSettings;
        private readonly IStandardMapping _standardMapping;
        private readonly IQueryHelper _queryHelper;

        public ApprenticeshipSearchServiceV1(
            IElasticsearchCustomClient elasticsearchCustomClient,
            IConfigurationSettings applicationSettings,
            IStandardMapping standardMapping,
            IQueryHelper queryHelper)
        {
            _elasticsearchCustomClient = elasticsearchCustomClient;
            _applicationSettings = applicationSettings;
            _standardMapping = standardMapping;
            _queryHelper = queryHelper;
        }

        public List<ApprenticeshipSearchResultsItem> SearchApprenticeships(string keywords, int page)
        {
            var takeElements = 20;

            var formattedKeywords = _queryHelper.FormatKeywords(keywords);

            var searchDescriptor = GetSearchDescriptor(page, takeElements, formattedKeywords);

            var results = _elasticsearchCustomClient.Search<ApprenticeshipSearchResultsItem>(s => searchDescriptor);

            return results.Documents.ToList();
        }

        private SearchDescriptor<ApprenticeshipSearchResultsItem> GetSearchDescriptor(
            int page, int take, string formattedKeywords)
        {
            return formattedKeywords == "*"
                ? GetAllSearchDescriptor(page, take, formattedKeywords)
                : GetKeywordSearchDescriptor(page, take, formattedKeywords);
        }

        private SearchDescriptor<ApprenticeshipSearchResultsItem> GetAllSearchDescriptor(
            int page, int take, string formattedKeywords)
        {
            var skip = (page - 1) * take;

            var searchDescriptor = new SearchDescriptor<ApprenticeshipSearchResultsItem>()
                .Index(_applicationSettings.ApprenticeshipIndexAlias)
                .Skip(skip)
                .Take(take)
                .Query(q => q
                    .Bool(b => b
                    .Filter(PublishedApprenticeship())
                    .Must(
                        MustBeStartedApprenticeship(),
                        MustBeNonExpiredApprenticceship(),
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
                                .Query(formattedKeywords)))));

            GetSortingOrder(searchDescriptor);

            return searchDescriptor;
        }

        private SearchDescriptor<ApprenticeshipSearchResultsItem> GetKeywordSearchDescriptor(
            int page, int take, string formattedKeywords)
        {
            var skip = (page - 1) * take;
            var searchDescriptor = new SearchDescriptor<ApprenticeshipSearchResultsItem>()
                    .Index(_applicationSettings.ApprenticeshipIndexAlias)
                    .Skip(skip)
                    .Take(take)
                    .Query(q => q
                        .Bool(b => b
                            .Filter(PublishedApprenticeship())
                            .Must(
                                MustBeStartedApprenticeship(),
                                MustBeNonExpiredApprenticceship(),
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
                                                    .PrefixLength(3)))))));

            GetSortingOrder(searchDescriptor);

            return searchDescriptor;
        }

        private void GetSortingOrder(SearchDescriptor<ApprenticeshipSearchResultsItem> searchDescriptor)
        {            
            searchDescriptor.Sort(s => s
                .Descending(SortSpecialField.Score)
                .Descending(f => f.TitleKeyword)
                .Descending(f => f.Level));
        }

        private static Func<QueryContainerDescriptor<ApprenticeshipSearchResultsItem>, QueryContainer> PublishedApprenticeship()
        {
            return f => f
                .Term(t => t
                    .Field(fi => fi.Published).Value(true));
        }

        private static Func<QueryContainerDescriptor<ApprenticeshipSearchResultsItem>, QueryContainer> MustBeNonExpiredApprenticceship()
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

        private static Func<QueryContainerDescriptor<ApprenticeshipSearchResultsItem>, QueryContainer> MustBeStartedApprenticeship()
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
    }
}
