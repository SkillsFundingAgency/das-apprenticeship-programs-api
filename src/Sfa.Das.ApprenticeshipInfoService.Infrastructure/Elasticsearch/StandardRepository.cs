using System;
using System.Collections.Generic;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models;
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
        private readonly IGetIfaStandardsUrlService _getIfaStandardUrlService;

        private const int Take = 20;

        public StandardRepository(
            IElasticsearchCustomClient elasticsearchCustomClient,
            IConfigurationSettings applicationSettings,
            IStandardMapping standardMapping,
            IQueryHelper queryHelper, 
            IGetIfaStandardsUrlService getIfaStandardUrlService)
        {
            _elasticsearchCustomClient = elasticsearchCustomClient;
            _applicationSettings = applicationSettings;
            _standardMapping = standardMapping;
            _queryHelper = queryHelper;
            _getIfaStandardUrlService = getIfaStandardUrlService;
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
                .From(0)
                .Size(1)
                .Query(q => +q.Term(t => t.StandardId, id) && +q.Term("documentType", "standarddocument")));

            var document = results.Documents.Any() ? results.Documents.First() : null;

            var response = document != null ? _standardMapping.MapToStandard(document) : null;

            if (response != null)
            {
                response.StandardPageUri = _getIfaStandardUrlService.GetStandardUrl(response.StandardId);
            }

            return response;
        }

        public List<Standard> GetStandardsById(List<int> ids, int page)
        {
            var skip = GetSkipAmount(page);
            ids.Sort();
            var elements = ids.Skip(skip).Take(Take);
            var results = _elasticsearchCustomClient.Search<StandardSearchResultsItem>(
                s =>
                    s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                        .Take(Take)
                        .Query(q => +q.Term("documentType", "standarddocument") && +q.Terms(t => t.Field(fi => fi.StandardId).Terms(elements))));

            var documents = results.Documents.Any() ? results.Documents : null;

            if (documents == null)
            {
                return null;
            }

            var response = new List<Standard>();

            foreach (var standardSearchResultsItem in documents)
            {
                var standard = _standardMapping.MapToStandard(standardSearchResultsItem);
                standard.StandardPageUri = _getIfaStandardUrlService.GetStandardUrl(standard.StandardId);

                response.Add(standard);
            }

            return response;
        }

        private int GetSkipAmount(int page)
        {
            return (page - 1) * Take;
        }

        private ISearchRequest GetAllStandardsSeachDescriptor(int take)
        {
            return new SearchDescriptor<StandardSearchResultsItem>()
                .Index(_applicationSettings.ApprenticeshipIndexAlias)
                .From(0)
                .Sort(sort => sort.Ascending(f => f.StandardIdKeyword))
                .Take(take)
                .Query(q => +q.Term("documentType", "standarddocument"));
        }
    }
}
