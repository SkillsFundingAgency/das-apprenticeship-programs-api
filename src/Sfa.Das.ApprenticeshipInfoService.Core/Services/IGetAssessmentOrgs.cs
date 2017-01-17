﻿namespace Sfa.Das.ApprenticeshipInfoService.Core.Services
{
    using System.Collections.Generic;
    using SFA.DAS.Apprenticeships.Api.Types;
    using SFA.DAS.Apprenticeships.Api.Types.DTOs;

    public interface IGetAssessmentOrgs
    {
        IEnumerable<OrganisationDTO> GetAllOrganisations();

        OrganisationDetailsDTO GetOrganisationById(string organisationId);

        IEnumerable<OrganisationDetailsDTO> GetOrganisationsByStandardId(string standardId);
    }
}