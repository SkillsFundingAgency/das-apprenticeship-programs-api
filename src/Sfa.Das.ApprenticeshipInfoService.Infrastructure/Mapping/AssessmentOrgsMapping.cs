using System;
using System.Collections.Generic;
using System.Linq;
using Sfa.Das.ApprenticeshipInfoService.Application.Models;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    public class AssessmentOrgsMapping : IAssessmentOrgsMapping
    {
        public OrganisationSummary MapToOrganisationDto(OrganisationDocument organisation)
        {
            return new OrganisationSummary
            {
                Id = organisation.EpaOrganisationIdentifier,
                Name = organisation.EpaOrganisation
            };
        }

        public Organisation MapToOrganisationDetailsDto(OrganisationDocument organisation)
        {
            if (organisation == null)
            {
                return null;
            }

            return new Organisation
            {
                Id = organisation.EpaOrganisationIdentifier,
                Name = organisation.EpaOrganisation,
                Email = organisation.Email,
                Phone = organisation.Phone,
                Address = new Address
                {
                    Primary = organisation.Address.Primary,
                    Secondary = organisation.Address.Secondary,
                    Street = organisation.Address.Street,
                    Town = organisation.Address.Town,
                    Postcode = organisation.Address.Postcode
                },
                //Type = organisation.OrganisationType,
                Website = organisation.WebsiteLink
            };
        }

        public IEnumerable<Organisation> MapToOrganisationsDetailsDto(IEnumerable<OrganisationDocument> organisations)
        {
            return organisations.Select(MapToOrganisationDetailsDto);
        }

        public IEnumerable<StandardOrganisationSummary> MapToStandardOrganisationsSummary(IEnumerable<StandardOrganisationDocument> standardOrganisations)
        {
            var groupedStandardOrganisations = standardOrganisations.GroupBy(x => x.StandardCode);

            var result = new List<StandardOrganisationSummary>();

            foreach (var standardOrganisation in groupedStandardOrganisations)
            {
                result.Add(MapToStandardOrganisationSummary(standardOrganisation));
            }

            return result;
        }

        private StandardOrganisationSummary MapToStandardOrganisationSummary(IGrouping<string, StandardOrganisationDocument> standardOrganisation)
        {
            var result = new StandardOrganisationSummary
            {
                StandardCode = standardOrganisation.First().StandardCode
            };

            var periods = new List<Period>();

            foreach (var standardOrganisationDocument in standardOrganisation)
            {
                var period = new Period
                {
                    EffectiveFrom = standardOrganisationDocument.EffectiveFrom
                };

                if (standardOrganisationDocument.EffectiveTo != null && standardOrganisationDocument.EffectiveTo != default(DateTime))
                {
                    period.EffectiveTo = standardOrganisationDocument.EffectiveTo;
                }

                periods.Add(period);
            }

            result.Periods = periods;

            return result;
        }
    }
}
