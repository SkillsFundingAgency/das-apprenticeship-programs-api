using Nest;
using Sfa.Das.ApprenticeshipInfoService.Application.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models;
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
                            .Type(Types.Parse("organisationdocument"))
                            .From(0)
                            .MatchAll());
            return (int)results.HitsMetaData.Total;
        }

        public int GetOrganisationsAmountByStandardId(string standardId)
        {
            var results =
                _elasticsearchCustomClient.Search<StandardOrganisationDocument>(
                    s =>
                        s.Index(_applicationSettings.AssessmentOrgsIndexAlias)
                            .Type(Types.Parse("standardorganisationdocument"))
                            .From(0)
                            .Query(q => q
                                .Match(m => m
                                    .Field(f => f.StandardCode)
                                    .Query(standardId))));
            return (int)results.HitsMetaData.Total;
        }

        public int GetStandardsByOrganisationIdentifierAmount(string organisationId)
        {
            var results =
                _elasticsearchCustomClient.Search<StandardOrganisationDocument>(
                    s =>
                        s.Index(_applicationSettings.AssessmentOrgsIndexAlias)
                            .Type(Types.Parse("standardorganisationdocument"))
                            .From(0)
                            .Query(q => q
                                .Match(m => m
                                    .Field(f => f.EpaOrganisationIdentifier)
                                    .Query(organisationId))));
            return (int)results.HitsMetaData.Total;
        }

        public int GetFrameworksTotalAmount()
        {
            var results =
                _elasticsearchCustomClient.Search<FrameworkSearchResultsItem>(
                    s =>
                        s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                            .Type(Types.Parse("frameworkdocument"))
                            .From(0)
                            .MatchAll());
            return (int)results.HitsMetaData.Total;
        }

        public int GetProvidersTotalAmount()
        {
            var results =
                _elasticsearchCustomClient.Search<Provider>(
                    s =>
                        s.Index(_applicationSettings.ProviderIndexAlias)
                            .Type(Types.Parse("providerapidocument"))
                            .From(0)
                            .MatchAll());
            return (int)results.HitsMetaData.Total;
        }

        public int GetStandardsTotalAmount()
        {
            var results =
                _elasticsearchCustomClient.Search<StandardSearchResultsItem>(
                    s =>
                        s.Index(_applicationSettings.ApprenticeshipIndexAlias)
                            .Type(Types.Parse("standarddocument"))
                            .From(0)
                            .MatchAll());
            return (int)results.HitsMetaData.Total;
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

            return (int)results.HitsMetaData.Total;
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

            return (int)results.HitsMetaData.Total;
        }
    }
}
