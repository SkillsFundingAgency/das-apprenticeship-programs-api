using System;
using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch
{
    using System.Linq;
    using Core.Configuration;
    using Core.Models;
    using Core.Services;
    using Mapping;
    using Nest;

    public sealed class ProviderSearchService : IProviderSearchService
    {
        private readonly IElasticsearchCustomClient _elasticsearchCustomClient;
        private readonly IConfigurationSettings _applicationSettings;
        private readonly IStandardMapping _standardMapping;
        private readonly IQueryHelper _queryHelper;

        public ProviderSearchService(
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

        public List<ProviderSearchResultsItem> SearchProviders(string keywords, int page)
        {
            if (keywords.Length < 3)
            {
                return new List<ProviderSearchResultsItem>();
            }

            const int takeElements = 20;

            var formattedKeywords = _queryHelper.FormatKeywords(keywords);

            var searchDescriptor = GetSearchDescriptor(page, takeElements, formattedKeywords);

            var results = _elasticsearchCustomClient.Search<ProviderSearchResultsItem>(s => searchDescriptor);

            return results.Documents.ToList();
        }

        private SearchDescriptor<ProviderSearchResultsItem> GetSearchDescriptor(
            int page, int take, string formattedKeywords)
        {
            return formattedKeywords == "*"
                ? GetAllSearchDescriptor(page, take, formattedKeywords)
                : GetKeywordSearchDescriptor(page, take, formattedKeywords);
        }

        private SearchDescriptor<ProviderSearchResultsItem> GetAllSearchDescriptor(
            int page, int take, string formattedKeywords)
        {
            var skip = (page - 1) * take;

            var searchDescriptor = new SearchDescriptor<ProviderSearchResultsItem>()
                .Index(_applicationSettings.ProviderIndexAlias)
                .Skip(skip)
                .Take(take)
                .Analyzer(typeof(KeywordAnalyzer).ToString())
                .Query(q => q
                    .QueryString(qs => qs
                        .Fields(fs => fs
                            .Field(f => f.ProviderName)
                            .Field(p => p.Aliases))
                        .Query(formattedKeywords)) && +q.Term("documentType", "providerapidocument"));

            return searchDescriptor;
        }

        private SearchDescriptor<ProviderSearchResultsItem> GetKeywordSearchDescriptor(
            int page, int take, string formattedKeywords)
        {
	        var keywords = AddRequirementsToKeywords(formattedKeywords);
            var skip = (page - 1) * take;
            var searchDescriptor = new SearchDescriptor<ProviderSearchResultsItem>()
                    .Index(_applicationSettings.ProviderIndexAlias)
                    .Skip(skip)
                    .Take(take)
                    .Query(q => +q.Term("documentType", "providerapidocument") && q
                        .QueryString(qs => qs
                            .Fields(fs => fs
                                .Field(f => f.ProviderName)
                                .Field(p => p.Aliases))
                            .Query(keywords)));

			return searchDescriptor;
        }

	    private string AddRequirementsToKeywords(string formattedKeywords)
	    {
		    var keywords = formattedKeywords.Split(' ').Select(keyword => $"*{keyword}*").ToList();
		    return string.Join(" AND ", keywords);
		}
    }
}
