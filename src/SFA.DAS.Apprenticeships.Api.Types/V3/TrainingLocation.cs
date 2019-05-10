namespace SFA.DAS.Apprenticeships.Api.Types.V3
{
    public class TrainingLocation
    {
        public int LocationId { get; set; }

        public string LocationName { get; set; }

        public Address Address { get; set; }
    }
}
