using System;
using System.Linq;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Helpers
{
    public class ActiveApprenticceshipChecker : IActiveApprenticceshipChecker
    {
        private readonly IConfigurationSettings _configurationSettings;

        public ActiveApprenticceshipChecker(IConfigurationSettings configurationSettings)
        {
            _configurationSettings = configurationSettings;
        }

        public bool CheckActiveFramework(string frameworkId, DateTime? effectiveFrom, DateTime? effectiveTo)
        {
            return DateHelper.CheckEffectiveDates(effectiveFrom, effectiveTo) || IsSpecialLapsedFramework(frameworkId);
        }

        public bool CheckActiveStandard(string standardId, DateTime? effectiveFrom, DateTime? effectiveTo)
        {
            return DateHelper.CheckEffectiveDates(effectiveFrom, effectiveTo) || IsSpecialLapsedStandard(standardId);
        }

        private bool IsSpecialLapsedFramework(string frameworkId)
        {
            var lapsedFrameworks = _configurationSettings.FrameworksExpiredRequired;

            if (lapsedFrameworks == null || lapsedFrameworks.Count < 1)
            {
                return false;
            }

            return lapsedFrameworks.Any(lapsedFramework => lapsedFramework == frameworkId);
        }

        private bool IsSpecialLapsedStandard(string standardId)
        {
            var lapsedStandards = _configurationSettings.StandardsExpiredRequired;

            if (lapsedStandards == null || lapsedStandards.Count < 1)
            {
                return false;
            }

            return lapsedStandards.Any(lapsedStandard => lapsedStandard == standardId);
        }
    }
}
