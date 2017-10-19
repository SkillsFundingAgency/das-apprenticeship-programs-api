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

    public sealed class StandardRepository : IGetStandards
    {
        private readonly IElasticsearchCustomClient _elasticsearchCustomClient;
        private readonly IConfigurationSettings _applicationSettings;
        private readonly IStandardMapping _standardMapping;
        private readonly IQueryHelper _queryHelper;

        public StandardRepository(
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

        public IEnumerable<StandardSummary> GetAllStandards()
        {
            var take = _queryHelper.GetStandardsTotalAmount();

            var searchDescriptor = GetAllStandardsSeachDescriptor(take);

            var results =
                _elasticsearchCustomClient.Search<StandardSearchResultsItem>(s => searchDescriptor);

            if (results.ApiCall.HttpStatusCode != 200)
            {
                throw new ApplicationException($"Failed query all standards");
            }

            var resultList = results.Documents.Select(standardSearchResultsItem => _standardMapping.MapToStandardSummary(standardSearchResultsItem)).ToList();
            return resultList;
        }

        public Standard GetStandardById(string id)
        {
            var results = _elasticsearchCustomClient.Search<StandardSearchResultsItem>(
                s =>
                s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                .Type(Types.Parse("standarddocument"))
                .From(0)
                .Size(1)
                .Query(q => q
                    .Term(t => t
                        .Field(fi => fi.StandardId).Value(id))));

            var document = results.Documents.Any() ? results.Documents.First() : null;

            return document != null ? _standardMapping.MapToStandard(document) : null;
        }

        private ISearchRequest GetAllStandardsSeachDescriptor(int take)
        {
            if (Is<Elk5Feature>.Enabled)
            {
                return new SearchDescriptor<StandardSearchResultsItem>()
                    .Index(_applicationSettings.ApprenticeshipIndexAlias)
                    .Type(Types.Parse("standarddocument"))
                    .From(0)
                    .Sort(sort => sort.Ascending(f => f.StandardIdKeyword))
                    .Take(take)
                    .MatchAll();
            }

            return new SearchDescriptor<StandardSearchResultsItem>()
                .Index(_applicationSettings.ApprenticeshipIndexAlias)
                .Type(Types.Parse("standarddocument"))
                .From(0)
                .Sort(sort => sort.Ascending(f => f.StandardId))
                .Take(take)
                .MatchAll();
        }
    }
}
