using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types.Providers
{
    public class Feedback
    {
        public ICollection<ProviderAttribute> Strengths { get; set; }
        public ICollection<ProviderAttribute> Weaknesses { get; set; }
        public int ExcellentFeedbackCount { get; set; }
        public int GoodFeedbackCount { get; set; }
        public int PoorFeedbackCount { get; set; }
        public int VeryPoorFeedbackCount { get; set; }
    }
}
