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
        private readonly IQueryHelper _queryHelper;

        public FrameworkRepository(
            IElasticsearchCustomClient elasticsearchCustomClient,
            ILog applicationLogger,
            IConfigurationSettings applicationSettings,
            IFrameworkMapping frameworkMapping,
            IQueryHelper queryHelper)
        {
            _elasticsearchCustomClient = elasticsearchCustomClient;
            _applicationLogger = applicationLogger;
            _applicationSettings = applicationSettings;
            _frameworkMapping = frameworkMapping;
            _queryHelper = queryHelper;
        }

        public IEnumerable<FrameworkSummary> GetAllFrameworks()
        {
            var take = _queryHelper.GetFrameworksTotalAmount();

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
                        .Bool(b => b
                            .Must(m => m
                                .Term("frameworkId", id)))));

            var document = results.Documents.Any() ? results.Documents.First() : null;

            return document != null ? _frameworkMapping.MapToFramework(document) : null;
        }

        public IEnumerable<FrameworkCodeSummary> GetAllFrameworkCodes()
        {
            var frameworks = GetAllFrameworks();

            return frameworks.GroupBy(x => x.FrameworkCode).Select(frameworkSummary => _frameworkMapping.MapToFrameworkCodeSummary(frameworkSummary.First())).ToList();
        }

        public FrameworkCodeSummary GetFrameworkByCode(int frameworkCode)
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
                                    .Query(frameworkCode.ToString()))));

            var document = results.Documents.Any() ? results.Documents.First() : null;

            return document != null ? _frameworkMapping.MapToFrameworkCodeSummary(document) : null;
        }
    }
}
