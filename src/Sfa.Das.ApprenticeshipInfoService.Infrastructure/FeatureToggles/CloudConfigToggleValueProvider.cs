using FeatureToggle.Core;
using System.Configuration;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.FeatureToggles
{
    public class CloudConfigToggleValueProvider : IBooleanToggleValueProvider
    {
        public bool EvaluateBooleanToggleValue(IFeatureToggle toggle)
        {
	        var setting = ConfigurationManager.AppSettings[$"FeatureToggle.{toggle.GetType().Name}"];

	        return !string.IsNullOrEmpty(setting) && bool.Parse(setting);
        }
    }
}