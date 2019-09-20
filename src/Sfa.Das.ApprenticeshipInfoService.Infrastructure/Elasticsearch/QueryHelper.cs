using System.Text.RegularExpressions;
using Nest;
using Sfa.Das.ApprenticeshipInfoService.Application.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using SFA.DAS.Apprenticeships.Api.Types.Providers;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch
{
    public class QueryHelper : IQueryHelper
    {
        private readonly IElasticsearchCustomClient _elasticsearchCustomClient;
        private readonly IConfigurationSettings _applicationSettings;

        public QueryHelper(IElasticsearchCustomClient elasticsearchCustomClient, IConfigurationSettings applicationSettings)
        {
            _elasticsearchCustomClient = elasticsearchCustomClient;
            _applicationSettings = applicationSettings;
        }

        public int GetOrganisationsTotalAmount()
        {
            var results =
                _elasticsearchCustomClient.Search<OrganisationDocument>(
                    s =>
                        s.Index(_applicationSettings.AssessmentOrgsIndexAlias)
                            .From(0)
                            .Query(q => q.Term("documentType", "organisationdocument")));

            return (int)results.HitsMetadata.Total.Value;
        }

        public int GetOrganisationsAmountByStandardId(string standardId)
        {
            var results =
                _elasticsearchCustomClient.Search<StandardOrganisationDocument>(
                    s =>
                        s.Index(_applicationSettings.AssessmentOrgsIndexAlias)
                            .From(0)
                            .Query(q => +q.Term("documentType", "standardorganisationdocument") && +q
                                .Term(t => t.StandardCode.Suffix("keyword"), standardId)));

            return (int)results.HitsMetadata.Total.Value;
        }

        public int GetStandardsByOrganisationIdentifierAmount(string organisationId)
        {
            var results =
                _elasticsearchCustomClient.Search<StandardOrganisationDocument>(
                    s =>
                        s.Index(_applicationSettings.AssessmentOrgsIndexAlias)
                            .From(0)
                            .Query(q => +q.Term("documentType", "standardorganisationdocument") && +q
                                .Term(t => t.EpaOrganisationIdentifier.Suffix("keyword"), organisationId)));

            return (int)results.HitsMetadata.Total.Value;
        }

	    public string FormatKeywords(string query)
	    {
			if (string.IsNullOrEmpty(query))
		    {
			    return "*";
		    }

		    var queryformated = Regex.Replace(query, @"[+\-&|!(){}\[\]^""~?:\\/]", @" ");

		    return queryformated.ToLowerInvariant();
		}

	    public int GetFrameworksTotalAmount()
        {
            var results =
                _elasticsearchCustomClient.Search<FrameworkSearchResultsItem>(
                    s =>
                        s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                            .From(0)
                            .Query(q => +q.Term("documentType", "frameworkdocument")));

            return (int)results.HitsMetadata.Total.Value;
        }

        public int GetProvidersTotalAmount()
        {
            var results =
                _elasticsearchCustomClient.Search<Provider>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .From(0)
                            .Query(q => +q.Term("documentType", "providerapidocument")));

            return (int)results.HitsMetadata.Total.Value;
        }

        public int GetStandardsTotalAmount()
        {
            var results =
                _elasticsearchCustomClient.Search<StandardSearchResultsItem>(
                    s =>
                        s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                            .Size(0)
                            .Query(q => +q.Term("documentType", "standarddocument")));

            return (int)results.HitsMetadata.Total.Value;
        }

        public int GetProvidersByFrameworkTotalAmount(string frameworkId)
        {
            var results =
                _elasticsearchCustomClient.Search<FrameworkProviderSearchResultsItem>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .From(0)
                            .Sort(sort => sort.Ascending(f => f.Ukprn))
                            .Take(100)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.FrameworkId)
                                    .Terms(frameworkId))));

            return (int)results.HitsMetadata.Total.Value;
        }

        public int GetProvidersByStandardTotalAmount(string standardId)
        {
            var results =
                _elasticsearchCustomClient.Search<StandardProviderSearchResultsItem>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .From(0)
                            .Sort(sort => sort.Ascending(f => f.Ukprn))
                            .Take(100)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.StandardCode)
                                    .Terms(int.Parse(standardId)))));

            return (int)results.HitsMetadata.Total.Value;
        }
    }
}
