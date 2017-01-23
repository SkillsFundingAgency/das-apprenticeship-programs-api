using System.Collections.Generic;
using Sfa.Das.ApprenticeshipInfoService.Application.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Logging;
using Sfa.Das.ApprenticeshipInfoService.Core.Models.Responses;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;
using Organisation = SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs.Organisation;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch
{
    using System;
    using System.Linq;
    using Nest;
    using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
    using Sfa.Das.ApprenticeshipInfoService.Core.Models;
    using Sfa.Das.ApprenticeshipInfoService.Core.Services;
    using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;

    public sealed class AssessmentOrgsRepository : IGetAssessmentOrgs
    {
        private readonly IElasticsearchCustomClient _elasticsearchCustomClient;
        private readonly ILog _applicationLogger;
        private readonly IConfigurationSettings _applicationSettings;
        private readonly IProviderLocationSearchProvider _providerLocationSearchProvider;
        private readonly IAssessmentOrgsMapping _assessmentOrgsMapping;

        public AssessmentOrgsRepository(
            IElasticsearchCustomClient elasticsearchCustomClient,
            ILog applicationLogger,
            IConfigurationSettings applicationSettings,
            IProviderLocationSearchProvider providerLocationSearchProvider, 
            IAssessmentOrgsMapping assessmentOrgsMapping)
        {
            _elasticsearchCustomClient = elasticsearchCustomClient;
            _applicationLogger = applicationLogger;
            _applicationSettings = applicationSettings;
            _providerLocationSearchProvider = providerLocationSearchProvider;
            _assessmentOrgsMapping = assessmentOrgsMapping;
        }

        public IEnumerable<OrganisationSummary> GetAllOrganisations()
        {
            var take = GetOrganisationsTotalAmount();
            var results =
                _elasticsearchCustomClient.Search<Application.Models.Organisation>(
                    s =>
                    s.Index(_applicationSettings.AssessmentOrgsIndexAlias)
                        .Type(Types.Parse("organisationdocument"))
                        .From(0)
                        .Take(take)
                        .MatchAll());

            if (results.ApiCall.HttpStatusCode != 200)
            {
                throw new ApplicationException($"Failed query all organisations");
            }

            return results.Documents.OrderBy(x => x.EpaOrganisationIdentifier).Select(organisation => _assessmentOrgsMapping.MapToOrganisationDto(organisation)).ToList();
        }

        public Organisation GetOrganisationById(string organisationId)
        {
            var results =
                _elasticsearchCustomClient.Search<Application.Models.Organisation>(
                    s =>
                    s.Index(_applicationSettings.AssessmentOrgsIndexAlias)
                        .Type(Types.Parse("organisationdocument"))
                        .From(0)
                        .Take(1)
                        .Query(q => q
                            .Match(m => m
                                .Field(f => f.EpaOrganisationIdentifier)
                                .Query(organisationId))));

            if (results.ApiCall.HttpStatusCode != 200)
            {
                throw new ApplicationException($"Failed query organisation by id");
            }

            return _assessmentOrgsMapping.MapToOrganisationDetailsDto(results.Documents.FirstOrDefault());
        }

        public IEnumerable<Organisation> GetOrganisationsByStandardId(string standardId)
        {
            var ids = GetOrganisationIdsByStandardId(standardId);

            return ids.Select(GetOrganisationById).ToList();
        }

        private IEnumerable<string> GetOrganisationIdsByStandardId(string standardId)
        {
            var take = GetOrganisationsAmountByStandardId(standardId);

            var results =
                _elasticsearchCustomClient.Search<StandardOrganisation>(
                    s =>
                    s.Index(_applicationSettings.AssessmentOrgsIndexAlias)
                        .Type(Types.Parse("standardorganisationdocument"))
                        .From(0)
                        .Take(take)
                        .Query(q => q
                            .Match(m => m
                                .Field(f => f.StandardCode)
                                .Query(standardId))));

            return results.Documents.Select(result => result.EpaOrganisationIdentifier).ToList();
        }

        private int GetOrganisationsAmountByStandardId(string standardId)
        {
            var results =
                _elasticsearchCustomClient.Search<StandardOrganisation>(
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

        private int GetOrganisationsTotalAmount()
        {
            var results =
                _elasticsearchCustomClient.Search<Application.Models.Organisation>(
                    s =>
                    s.Index(_applicationSettings.AssessmentOrgsIndexAlias)
                        .Type(Types.Parse("organisationdocument"))
                        .From(0)
                        .MatchAll());
            return (int)results.HitsMetaData.Total;
        }
    }
}
