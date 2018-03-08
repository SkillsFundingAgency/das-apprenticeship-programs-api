namespace Sfa.Das.ApprenticeshipInfoService.Application.Models
{
    using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;

    public class OrganisationDocument
    {
        public string EpaOrganisationIdentifier { get; set; }

        public string EpaOrganisation { get; set; }

        public string OrganisationType { get; set; }

        public string WebsiteLink { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public Address Address { get; set; }

        public string EpaOrganisationIdentifierKeyword { get; set; }

        public int? Ukprn { get; set; }
    }
}
