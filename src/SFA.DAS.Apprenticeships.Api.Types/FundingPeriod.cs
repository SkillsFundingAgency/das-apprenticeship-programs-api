using System;

namespace SFA.DAS.Apprenticeships.Api.Types
{
    public interface IFundingPeriod
    {
        DateTime? EffectiveFrom { get; }
        DateTime? EffectiveTo { get; }
        int FundingCap { get; }
    }

    public class FundingPeriod : IFundingPeriod
    {
        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public int FundingCap { get; set; }
    }
}
