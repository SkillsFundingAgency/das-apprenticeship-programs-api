using System;
using System.Collections.Generic;
using System.Diagnostics;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.NLog.Logger;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch
{
    using System.Linq;
    using Core.Configuration;
    using Core.Models;
    using Core.Services;
    using Mapping;
    using Nest;

    public sealed class FrameworkRepository : IGetFrameworks
    {
        private readonly IElasticsearchCustomClient _elasticsearchCustomClient;
        private readonly ILog _applicationLogger;
        private readonly IConfigurationSettings _applicationSettings;
        private readonly IFrameworkMapping _frameworkMapping;

        public FrameworkRepository(
            IElasticsearchCustomClient elasticsearchCustomClient,
            ILog applicationLogger,
            IConfigurationSettings applicationSettings,
            IFrameworkMapping frameworkMapping)
        {
            _elasticsearchCustomClient = elasticsearchCustomClient;
            _applicationLogger = applicationLogger;
            _applicationSettings = applicationSettings;
            _frameworkMapping = frameworkMapping;
        }

        public IEnumerable<FrameworkSummary> GetAllFrameworks()
        {
            var take = GetFrameworksTotalAmount();

            var results =
                _elasticsearchCustomClient.Search<FrameworkSearchResultsItem>(
                    s =>
                    s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                        .Type(Types.Parse("frameworkdocument"))
                        .From(0)
                        .Sort(sort => sort.Ascending(f => f.FrameworkId))
                        .Take(take)
                        .MatchAll());

            if (results.ApiCall.HttpStatusCode != 200)
            {
                throw new ApplicationException($"Failed query all frameworks");
            }

            var resultList = results.Documents.Select(frameworkSearchResultsItem => _frameworkMapping.MapToFrameworkSummary(frameworkSearchResultsItem)).ToList();

            return resultList;
        }

        private int GetFrameworksTotalAmount()
        {
            var results =
                _elasticsearchCustomClient.Search<FrameworkSearchResultsItem>(
                    s =>
                    s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                        .Type(Types.Parse("frameworkdocument"))
                        .From(0)
                        .MatchAll());
            return (int) results.HitsMetaData.Total;
        }

        public Framework GetFrameworkById(string id)
        {
            var results =
                _elasticsearchCustomClient.Search<FrameworkSearchResultsItem>(
                    s =>
                    s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                        .Type(Types.Parse("frameworkdocument"))
                        .From(0)
                        .Size(1)
                        .Query(q => q
                        .MultiMatch(m => m
                            .Type(TextQueryType.Phrase)
                            .Fields(fs => fs
                                .Field(e => e.FrameworkId))
                            .Query(id))));

            var document = results.Documents.Any() ? results.Documents.First() : null;

            return document != null ? _frameworkMapping.MapToFramework(document) : null;
        }

        public IEnumerable<FrameworkResume> GetAllFrameworkCodes()
        {
            var frameworks = GetAllFrameworks();

            return frameworks.GroupBy(x => x.FrameworkCode).Select(frameworkSummary => _frameworkMapping.MapToFrameworkResume(frameworkSummary.First())).ToList();
        }

        public FrameworkResume GetFrameworkByCode(string frameworkCode)
        {
            var results =
                _elasticsearchCustomClient.Search<FrameworkSearchResultsItem>(
                    s =>
                        s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                            .Type(Types.Parse("frameworkdocument"))
                            .From(0)
                            .Size(1)
                            .Query(q => q
                                .MultiMatch(m => m
                                    .Type(TextQueryType.Phrase)
                                    .Fields(fs => fs
                                        .Field(e => e.FrameworkCode))
                                    .Query(frameworkCode))));

            var document = results.Documents.Any() ? results.Documents.First() : null;

            return document != null ? _frameworkMapping.MapToFrameworkResume(document) : null;
        }
    }
}
