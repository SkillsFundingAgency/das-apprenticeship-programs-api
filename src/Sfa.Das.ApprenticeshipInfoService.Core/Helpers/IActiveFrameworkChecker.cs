using System;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Helpers
{
    public interface IActiveFrameworkChecker
    {
        bool CheckActiveFramework(string frameworkId, DateTime? effectiveFrom, DateTime? effectiveTo);
    }
}