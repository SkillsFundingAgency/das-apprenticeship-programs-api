using System;
using System.Linq;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Helpers
{
    public class ActiveFrameworkChecker : IActiveFrameworkChecker
    {
        private readonly IConfigurationSettings _configurationSettings;

        public ActiveFrameworkChecker(IConfigurationSettings configurationSettings)
        {
            _configurationSettings = configurationSettings;
        }

        public bool CheckActiveFramework(string frameworkId, DateTime? effectiveFrom, DateTime? effectiveTo)
        {
            return DateHelper.CheckEffectiveDates(effectiveFrom, effectiveTo) || IsSpecialLapsedFramework(frameworkId);
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
    }
}
