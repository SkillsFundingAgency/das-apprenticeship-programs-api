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
                            .Query(q => q
                                .Match(m => m
                                    .Field("documentType")
                                    .Query("OrganisationDocument"))));
                                
            return (int)results.HitsMetadata.Total.Value;
        }

        public int GetOrganisationsAmountByStandardId(string standardId)
        {
            var results =
                _elasticsearchCustomClient.Count<StandardOrganisationDocument>(
                    s =>
                        s.Index(_applicationSettings.AssessmentOrgsIndexAlias)
                            .Query(q => q
                                .Bool(b => b
                                    .Must(mu => mu
                                        .Match(m => m
                                            .Field(f => f.StandardCode)
                                            .Query(standardId)), mu => mu
                                        .Match(m => m
                                            .Field("documentType")
                                            .Query("StandardOrganisationDocument"))))));
            return (int)results.Count;
        }

        public int GetStandardsByOrganisationIdentifierAmount(string organisationId)
        {
            var results =
                _elasticsearchCustomClient.Count<StandardOrganisationDocument>(
                    s =>
                        s.Index(_applicationSettings.AssessmentOrgsIndexAlias)
                            .Query(q => q
                                .Bool(b => b
                                    .Must(mu => mu
                                        .Match(m => m
                                            .Field(f => f.EpaOrganisationIdentifier)
                                            .Query(organisationId)), mu => mu
                                        .Match(m => m
                                            .Field("documentType")
                                            .Query("StandardOrganisationDocument"))))));
            return (int)results.Count;
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
                _elasticsearchCustomClient.Count<FrameworkSearchResultsItem>(
                    s =>
                        s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                            .Query(q => q
                                .Match(m => m
                                    .Field("documentType")
                                    .Query("FrameworkDocument"))));
            return (int)results.Count;
        }

        public int GetProvidersTotalAmount()
        {
            var results =
                _elasticsearchCustomClient.Count<Provider>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .Query(q => q
                                .Match(m => m
                                    .Field("documentType")
                                    .Query("ProviderApiDocument"))));
            return (int)results.Count;
        }

        public int GetStandardsTotalAmount()
        {
            var results =
                _elasticsearchCustomClient.Count<StandardSearchResultsItem>(
                    s =>
                        s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                            .Query(q => q
                                .Match(m => m
                                    .Field("documentType")
                                    .Query("StandardDocument"))));
            return (int)results.Count;
        }

        public int GetProvidersByFrameworkTotalAmount(string frameworkId)
        {
            var results =
                _elasticsearchCustomClient.Count<FrameworkProviderSearchResultsItem>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.FrameworkId)
                                    .Terms(frameworkId))));

            return (int)results.Count;
        }

        public int GetProvidersByStandardTotalAmount(string standardId)
        {
            var results =
                _elasticsearchCustomClient.Count<StandardProviderSearchResultsItem>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .Query(q => q
                                .Terms(t => t
                                    .Field(f => f.StandardCode)
                                    .Terms(int.Parse(standardId)))));

            return (int)results.Count;
        }
    }
}
