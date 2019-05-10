using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types.V3
{
    public class ProviderSearchResultItem
    {
        public int Ukprn { get; set; }

        public TrainingLocation Location { get; set; }

        public string ProviderName { get; set; }

        public double? OverallAchievementRate { get; set; }

        public bool NationalProvider { get; set; }

        public List<string> DeliveryModes { get; set; }

        public double Distance { get; set; }

        public double? EmployerSatisfaction { get; set; }

        public double? LearnerSatisfaction { get; set; }

        public double? NationalOverallAchievementRate { get; set; }

        public string OverallCohort { get; set; }

        public bool HasNonLevyContract { get; set; }

        public bool IsLevyPayerOnly { get; set; }

        public bool CurrentlyNotStartingNewApprentices { get; set; }
    }
}
