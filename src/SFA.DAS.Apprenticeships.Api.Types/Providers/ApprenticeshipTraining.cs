namespace SFA.DAS.Apprenticeships.Api.Types.Providers
{
    public class ApprenticeshipTraining
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public ApprenticeshipTrainingType TrainingType { get; set; }
        public int Level { get; set; }
        public string Identifier { get; set; }
    }
}
