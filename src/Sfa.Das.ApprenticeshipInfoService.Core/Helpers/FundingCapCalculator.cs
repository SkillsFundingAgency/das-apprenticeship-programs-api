using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Helpers
{
    public class FundingCapCalculator : IFundingCapCalculator
    {
        private readonly IActiveApprenticeshipChecker _activeApprenticeshipChecker;

        public FundingCapCalculator(IActiveApprenticeshipChecker activeApprenticeshipChecker)
        {
            _activeApprenticeshipChecker = activeApprenticeshipChecker;
        }

        public int CalculateCurrentFundingBand(StandardSearchResultsItem standard)
        {
            return _activeApprenticeshipChecker.CheckActiveStandard(standard.EffectiveFrom, standard.EffectiveTo) ? GetFundingCapFromPeriods(standard.FundingPeriods) : 0;
        }

        public int CalculateCurrentFundingBand(FrameworkSearchResultsItem framework)
        {
            return _activeApprenticeshipChecker.CheckActiveFramework(framework.EffectiveFrom, framework.EffectiveTo) ? GetFundingCapFromPeriods(framework.FundingPeriods) : 0;
        }

        private int GetFundingCapFromPeriods(List<FundingPeriod> fundingPeriods)
        {
            if (fundingPeriods == null)
            {
                return 0;
            }

            foreach (var fundingPeriod in fundingPeriods)
            {
                if (fundingPeriod.EffectiveFrom != null && fundingPeriod.EffectiveFrom <= DateTime.Today &&
                    fundingPeriod.EffectiveTo != null && fundingPeriod.EffectiveTo >= DateTime.Today)
                {
                    return fundingPeriod.FundingCap;
                }
            }

            return fundingPeriods.Last().FundingCap;
        }
    }
}
