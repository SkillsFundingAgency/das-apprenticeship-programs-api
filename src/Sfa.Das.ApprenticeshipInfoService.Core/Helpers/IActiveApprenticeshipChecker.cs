using System;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Helpers
{
    public interface IActiveApprenticeshipChecker
    {
        bool CheckActiveFramework(string frameworkId, DateTime? effectiveFrom, DateTime? effectiveTo);
        bool CheckActiveStandard(string standardId, DateTime? effectiveFrom, DateTime? effectiveTo);
    }
}