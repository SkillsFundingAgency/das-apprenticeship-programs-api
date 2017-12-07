using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types.Providers
{
    public class ApprenticeshipTrainingSummary
    {
        public long Ukprn { get; set; }

        public int Count { get; set; }

        public IEnumerable<ApprenticeshipTraining> ApprenticeshipTrainingItems { get; set; }
    }
}
