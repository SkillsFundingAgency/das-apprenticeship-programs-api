﻿using System;
using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch
{
    using System.Linq;
    using Core.Configuration;
    using Core.Models;
    using Core.Services;
    using Mapping;
    using Microsoft.Extensions.Logging;
    using Nest;

    public sealed class FrameworkRepository : IGetFrameworks
    {
        private readonly IElasticsearchCustomClient _elasticsearchCustomClient;
        private readonly ILogger<FrameworkRepository> _applicationLogger;
        private readonly IConfigurationSettings _applicationSettings;
        private readonly IFrameworkMapping _frameworkMapping;
        private readonly IQueryHelper _queryHelper;

        public FrameworkRepository(
            IElasticsearchCustomClient elasticsearchCustomClient,
            ILogger<FrameworkRepository> applicationLogger,
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

            var searchDescriptor = GetAllFrameworksSearchDescriptor(take);

            var results =
                _elasticsearchCustomClient.Search<FrameworkSearchResultsItem>(s => searchDescriptor);

            if (results.ApiCall.HttpStatusCode != 200)
            {
                throw new ApplicationException($"Failed query all frameworks");
            }

            var resultList = results.Documents.Select(frameworkSearchResultsItem => _frameworkMapping.MapToFrameworkSummary(frameworkSearchResultsItem));

            return resultList;
        }

        public Framework GetFrameworkById(string id)
        {
            var results =
                _elasticsearchCustomClient.Search<FrameworkSearchResultsItem>(
                    s =>
                    s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                        .From(0)
                        .Size(1)
                        .Query(q => +q.Term("documentType", "frameworkdocument") && +q.Term("frameworkId", id)));

            var document = results.Documents.Any() ? results.Documents.First() : null;

            return document != null ? _frameworkMapping.MapToFramework(document) : null;
        }

        public IEnumerable<FrameworkCodeSummary> GetAllFrameworkCodes()
        {
            var frameworks = GetAllFrameworks();

            return frameworks.GroupBy(x => x.FrameworkCode).Select(frameworkSummaries => _frameworkMapping.MapToFrameworkCodeSummaryFromList(frameworkSummaries.ToList())).ToList();
        }

        public FrameworkCodeSummary GetFrameworkByCode(int frameworkCode)
        {
            var results =
                _elasticsearchCustomClient.Search<FrameworkSearchResultsItem>(
                    s =>
                        s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                            .From(0)
                            .Size(1)
                            .Query(q => +q.Term("documentType", "frameworkdocument") && +q.Term(t => t.FrameworkCode, frameworkCode)));

	        return results.Documents.Any() ? _frameworkMapping.MapToFrameworkCodeSummaryFromList(results.Documents.ToList()) : null;
        }

        private ISearchRequest GetAllFrameworksSearchDescriptor(int take)
        {
            return new SearchDescriptor<FrameworkSearchResultsItem>()
                .Index(_applicationSettings.ApprenticeshipIndexAlias)
                .From(0)
                .Sort(sort => sort.Ascending(f => f.FrameworkIdKeyword))
                .Take(take)
                .Query(q => +q.Term("documentType", "frameworkdocument"));
        }
    }
}
