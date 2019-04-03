using System;
using System.Collections.Generic;
using System.Linq;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Helpers
{
    public class ActiveApprenticeshipChecker : IActiveApprenticeshipChecker
    {
        public bool IsActiveFramework(DateTime? effectiveFrom, DateTime? effectiveTo)
        {
            return DateHelper.CheckEffectiveDates(effectiveFrom, effectiveTo);
        }

        public bool IsActiveStandard(DateTime? effectiveFrom, DateTime? effectiveTo)
        {
            return DateHelper.CheckEffectiveDates(effectiveFrom, effectiveTo);
        }
    }
}
