using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types.Pagination;

namespace SFA.DAS.Apprenticeships.Api.Types.Providers
{
    public class ApprenticeshipTrainingSummary
    {
        public long Ukprn { get; set; }        
        public PaginationDetails PaginationDetails { get; set; }
        public IEnumerable<ApprenticeshipTraining> ApprenticeshipTrainingItems { get; set; }

    }
}
