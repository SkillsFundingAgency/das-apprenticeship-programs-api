using System;
using System.Collections.Generic;
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

            return CheckValueIntoList(frameworkId, lapsedFrameworks);
        }

        private bool IsSpecialLapsedStandard(string standardId)
        {
            var lapsedStandards = _configurationSettings.StandardsExpiredRequired;

            return CheckValueIntoList(standardId, lapsedStandards);
        }

        private static bool CheckValueIntoList(string apprenticeshipId, List<string> lapsedApprenticeships)
        {
            if (lapsedApprenticeships == null || lapsedApprenticeships.Count < 1)
            {
                return false;
            }

            return lapsedApprenticeships.Any(lapsedFramework => lapsedFramework == apprenticeshipId);
        }
    }
}
