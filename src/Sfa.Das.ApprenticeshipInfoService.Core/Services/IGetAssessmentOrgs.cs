using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Services
{
    using System.Collections.Generic;
    using SFA.DAS.Apprenticeships.Api.Types;

    public interface IGetAssessmentOrgs
    {
        IEnumerable<OrganisationSummary> GetAllOrganisations();

        Organisation GetOrganisationById(string organisationId);

        IEnumerable<Organisation> GetOrganisationsByStandardId(string standardId);

        IEnumerable<StandardOrganisationSummary> GetStandardsByOrganisationIdentifier(string organisationId);
    }
}
