using System.Collections.Generic;
using Sfa.Das.ApprenticeshipInfoService.Application.Models;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    public interface IAssessmentOrgsMapping
    {
        OrganisationSummary MapToOrganisationDto(OrganisationDocument organisation);
        Organisation MapToOrganisationDetailsDto(OrganisationDocument organisation);
        IEnumerable<Organisation> MapToOrganisationsDetailsDto(IEnumerable<OrganisationDocument> organisation);
        IEnumerable<StandardOrganisationSummary> MapToStandardOrganisationsSummary(IEnumerable<StandardOrganisationDocument> standardOrganisations);
    }
}