using System;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Helpers
{
    public interface IActiveApprenticeshipChecker
    {
        bool IsActiveFramework(DateTime? effectiveFrom, DateTime? effectiveTo);
        bool IsActiveStandard(DateTime? effectiveFrom, DateTime? effectiveTo);
    }
}