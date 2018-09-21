using System;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Helpers
{
    public interface IActiveApprenticeshipChecker
    {
        bool CheckActiveFramework(DateTime? effectiveFrom, DateTime? effectiveTo);
        bool CheckActiveStandard(DateTime? effectiveFrom, DateTime? effectiveTo);
    }
}