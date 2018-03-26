﻿using FeatureToggle.Core;
using FeatureToggle.Toggles;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.FeatureToggles
{
    public sealed class Elk5Feature : SimpleFeatureToggle
    {
        public override IBooleanToggleValueProvider ToggleValueProvider { get => new CloudConfigToggleValueProvider(); set => base.ToggleValueProvider = value; }
    }
}
