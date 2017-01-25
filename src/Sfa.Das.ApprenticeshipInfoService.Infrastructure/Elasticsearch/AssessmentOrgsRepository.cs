﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nest;
using Sfa.Das.ApprenticeshipInfoService.Application.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch
{
    public sealed class AssessmentOrgsRepository : IGetAssessmentOrgs
    {
        private readonly IElasticsearchCustomClient _elasticsearchCustomClient;
        private readonly IConfigurationSettings _applicationSettings;
        private readonly IAssessmentOrgsMapping _assessmentOrgsMapping;

        public AssessmentOrgsRepository(
            IElasticsearchCustomClient elasticsearchCustomClient,
            IConfigurationSettings applicationSettings, 
            IAssessmentOrgsMapping assessmentOrgsMapping)
        {
            _elasticsearchCustomClient = elasticsearchCustomClient;
            _applicationSettings = applicationSettings;
            _assessmentOrgsMapping = assessmentOrgsMapping;
        }

        public IEnumerable<OrganisationSummary> GetAllOrganisations()
        {
            var take = GetOrganisationsTotalAmount();
            var results =
                _elasticsearchCustomClient.Search<OrganisationDocument>(
                    s =>
                    s.Index(_applicationSettings.AssessmentOrgsIndexAlias)
                        .Type(Types.Parse("organisationdocument"))
                        .From(0)
                        .Sort(sort => sort.Ascending(f => f.EpaOrganisationIdentifier))
                        .Take(take)
                        .MatchAll());

            if (results.ApiCall.HttpStatusCode != 200)
            {
                throw new ApplicationException($"Failed query all organisations");
            }

            return results.Documents.Select(organisation => _assessmentOrgsMapping.MapToOrganisationDto(organisation)).ToList();
        }

        public Organisation GetOrganisationById(string organisationId)
        {
            var results =
                _elasticsearchCustomClient.Search<OrganisationDocument>(
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
            var take = GetOrganisationsAmountByStandardId(standardId);

            var results =
                _elasticsearchCustomClient.Search<StandardOrganisationDocument>(
                    s =>
                    s.Index(_applicationSettings.AssessmentOrgsIndexAlias)
                        .Type(Types.Parse("standardorganisationdocument"))
                        .From(0)
                        .Take(take)
                        .Query(q => q
                            .Match(m => m
                                .Field(f => f.StandardCode)
                                .Query(standardId))));

            if (results.ApiCall.HttpStatusCode != 200)
            {
                throw new ApplicationException($"Failed query organisations by standard id");
            }

            var organisations = results.Documents.Where(x => x.EffectiveFrom.Date <= DateTime.UtcNow.Date);

            return _assessmentOrgsMapping.MapToOrganisationsDetailsDto(organisations);
        }

        private int GetOrganisationsAmountByStandardId(string standardId)
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

        private int GetOrganisationsTotalAmount()
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
    }
}
