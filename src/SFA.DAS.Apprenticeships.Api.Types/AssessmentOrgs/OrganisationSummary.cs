using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs
{
    public class OrganisationSummary
    {
        public string Id { get; set; }

        public string Uri { get; set; }

        public string Name { get; set; }

        public List<Link> Links { get; set; }
    }
}
