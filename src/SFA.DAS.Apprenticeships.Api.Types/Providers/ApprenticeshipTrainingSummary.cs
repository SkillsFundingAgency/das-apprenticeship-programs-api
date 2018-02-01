using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types.Providers
{
    public class ApprenticeshipTrainingSummary
    {
        public long Ukprn { get; set; }

        public int TotalCount { get; set; }

        public int NumberPerPage { get; set; }
        public int NumberReturned { get; set; }

        public int Page { get; set; }
        public IEnumerable<ApprenticeshipTraining> ApprenticeshipTrainingItems { get; set; }
    }
}
