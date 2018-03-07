using FeatureToggle.Core;
using Microsoft.Azure;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.FeatureToggles
{
    public class CloudConfigToggleValueProvider : IBooleanToggleValueProvider
    {
        public bool EvaluateBooleanToggleValue(IFeatureToggle toggle)
        {
            return bool.Parse(CloudConfigurationManager.GetSetting($"FeatureToggle.{toggle.GetType().Name}"));
        }
    }
}