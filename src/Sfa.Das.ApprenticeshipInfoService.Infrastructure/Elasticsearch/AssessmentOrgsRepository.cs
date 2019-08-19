﻿using System;
using System.Collections.Generic;
using System.Linq;
using FeatureToggle.Core.Fluent;
using Nest;
using Sfa.Das.ApprenticeshipInfoService.Application.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.FeatureToggles;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch
{
    public sealed class AssessmentOrgsRepository : IGetAssessmentOrgs
    {
        private readonly IElasticsearchCustomClient _elasticsearchCustomClient;
        private readonly IConfigurationSettings _applicationSettings;
        private readonly IAssessmentOrgsMapping _assessmentOrgsMapping;
        private readonly IQueryHelper _queryHelper;

        public AssessmentOrgsRepository(
            IElasticsearchCustomClient elasticsearchCustomClient,
            IConfigurationSettings applicationSettings,
            IAssessmentOrgsMapping assessmentOrgsMapping,
            IQueryHelper queryHelper)
        {
            _elasticsearchCustomClient = elasticsearchCustomClient;
            _applicationSettings = applicationSettings;
            _assessmentOrgsMapping = assessmentOrgsMapping;
            _queryHelper = queryHelper;
        }

        public IEnumerable<OrganisationSummary> GetAllOrganisations()
        {
            var take = _queryHelper.GetOrganisationsTotalAmount();

            var searchDescriptor = GetAllAssessmentOrgsSearchDescriptor(take);

            var results =
                _elasticsearchCustomClient.Search<OrganisationDocument>(
                    s =>
                    searchDescriptor);

            if (results.ApiCall.HttpStatusCode != 200)
            {
                throw new ApplicationException("Failed query all organisations");
            }

            return results.Documents.Select(organisation => _assessmentOrgsMapping.MapToOrganisationDto(organisation)).ToList();
        }

        public Organisation GetOrganisationById(string organisationId)
        {
            var results =
                _elasticsearchCustomClient.Search<OrganisationDocument>(
                    s =>
                    s.Index(_applicationSettings.AssessmentOrgsIndexAlias)
                        .From(0)
                        .Take(1)
                        .Query(q => q
                            .Bool(b => b
                                .Must(mu => mu
                                    .Match(m => m
                                        .Field("documentType")
                                        .Query("OrganisationDocument")) && q
                                    .Match(m => m
                                        .Field(f => f.EpaOrganisationIdentifier)
                                        .Query(organisationId))))));

            if (results.ApiCall.HttpStatusCode != 200)
            {
                throw new ApplicationException("Failed query organisation by id");
            }

            return _assessmentOrgsMapping.MapToOrganisationDetailsDto(results.Documents.FirstOrDefault());
        }

        public IEnumerable<Organisation> GetOrganisationsByStandardId(string standardId)
        {
            var take = _queryHelper.GetOrganisationsAmountByStandardId(standardId);

            var results =
                _elasticsearchCustomClient.Search<StandardOrganisationDocument>(
                    s =>
                    s.Index(_applicationSettings.AssessmentOrgsIndexAlias)
                        .From(0)
                        .Take(take)
                        .Query(q => q
                            .Bool(b => b
                                .Must(mu => mu
                                    .Match(m => m
                                        .Field(f => f.StandardCode)
                                        .Query(standardId)), mu => mu
                                    .Match(m => m
                                        .Field("documentType")
                                        .Query("StandardOrganisationDocument"))))));

            if (results.ApiCall.HttpStatusCode != 200)
            {
                throw new ApplicationException("Failed query organisations by standard id");
            }

            var organisations = results.Documents.Where(x => x.EffectiveFrom.Date <= DateTime.UtcNow.Date && (x.EffectiveTo == null || x.EffectiveTo.Value.Date >= DateTime.UtcNow.Date)).ToList();

            return _assessmentOrgsMapping.MapToOrganisationsDetailsDto(organisations);
        }

        private ISearchRequest GetAllAssessmentOrgsSearchDescriptor(int take)
        {
            return new SearchDescriptor<OrganisationDocument>()
                .Index(_applicationSettings.AssessmentOrgsIndexAlias)
                .From(0)
                .Sort(sort => sort.Ascending(f => f.EpaOrganisationIdentifierKeyword))
                .Take(take)
                .Query(q => q
                    .Match(m => m
                        .Field("documentType")
                        .Query("OrganisationDocument")));
        }

        public IEnumerable<StandardOrganisationSummary> GetStandardsByOrganisationIdentifier(string organisationId)
        {
            var take = _queryHelper.GetStandardsByOrganisationIdentifierAmount(organisationId);
            var results =
                _elasticsearchCustomClient.Search<StandardOrganisationDocument>(
                    s =>
                        s.Index(_applicationSettings.AssessmentOrgsIndexAlias)
                            .From(0)
                            .Take(take)
                            .Query(q => q
                                .Bool(b => b
                                    .Must(mu => mu
                                        .Match(m => m
                                            .Field(f => f.EpaOrganisationIdentifier)
                                            .Query(organisationId)), mu => mu
                                        .Match(m => m
                                            .Field("documentType")
                                            .Query("StandardOrganisationDocument"))))));

            if (results.ApiCall.HttpStatusCode != 200)
            {
                throw new ApplicationException("Failed query standards by organisation id");
            }

            return _assessmentOrgsMapping.MapToStandardOrganisationsSummary(results.Documents).OrderBy(x => x.StandardCode);
        }
    }
}