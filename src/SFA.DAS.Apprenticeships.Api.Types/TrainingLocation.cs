using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;

namespace SFA.DAS.Apprenticeships.Api.Types
{
    public class TrainingLocation
    {
        public int LocationId { get; set; }

        public string LocationName { get; set; }

        public Address Address { get; set; }

        public object Location { get; set; }

        public object LocationPoint { get; set; }
    }
}
