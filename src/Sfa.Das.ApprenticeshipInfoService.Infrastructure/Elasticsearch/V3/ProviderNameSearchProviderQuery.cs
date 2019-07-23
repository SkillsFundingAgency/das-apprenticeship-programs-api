﻿using System;
using Nest;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.Sas.Infrastructure.Elasticsearch;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch.V3
{
    public class ProviderNameSearchProviderQuery : IProviderNameSearchProviderQuery
    {
        private readonly IElasticsearchCustomClient _elasticsearchCustomClient;
        private readonly IConfigurationSettings _applicationSettings;

        public ProviderNameSearchProviderQuery(IElasticsearchCustomClient elasticsearchCustomClient, IConfigurationSettings applicationSettings)
        {
            _elasticsearchCustomClient = elasticsearchCustomClient;
            _applicationSettings = applicationSettings;
        }

        public ISearchResponse<ProviderNameSearchResult> GetResults(string term, int take, int skip)
        {
            return _elasticsearchCustomClient.Search<ProviderNameSearchResult>(s => s
                .Index(_applicationSettings.ProviderIndexAlias)
                .Type("providerapidocument")
                .Skip(skip)
                .Take(take)
                .Query(q => q
                    .Bool(b => b
                        .Filter(IsNotEmployerProvider())
                        .Must(mu => mu
                            .QueryString(qs => qs
                                .Fields(fs => fs
                                    .Field(f => f.ProviderName)
                                    .Field(f => f.Aliases))
                                .Query(term)))
                    )));
        }

        public long GetTotalMatches(string term)
        {
            var initialDetails = _elasticsearchCustomClient.Search<ProviderNameSearchResult>(s => s
                .Index(_applicationSettings.ProviderIndexAlias)
                .Type("providerapidocument")
                .Query(q => q
                    .Bool(b => b
                        .Filter(IsNotEmployerProvider())
                        .Must(mu => mu
                            .QueryString(qs => qs
                                .Fields(fs => fs
                                    .Field(f => f.ProviderName)
                                    .Field(f => f.Aliases))
                                .Query(term)))
                    )));

            return initialDetails.HitsMetaData.Total;
        }

        private static Func<QueryContainerDescriptor<ProviderNameSearchResult>, QueryContainer> IsNotEmployerProvider()
        {
            return f => f
                .Term(t => t
                    .Field(fi => fi.IsEmployerProvider).Value(false));
        }
    }
}