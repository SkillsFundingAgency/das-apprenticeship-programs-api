namespace Sfa.Das.ApprenticeshipInfoService.Core.Configuration
{
    using System;
    using System.Collections.Generic;

    public interface IConfigurationSettings
    {
        string ApprenticeshipIndexAlias { get; }

        string ProviderIndexAlias { get; }

        string AssessmentOrgsIndexAlias { get; }

        IEnumerable<Uri> ElasticServerUrls { get; }

        string EnvironmentName { get; }

        string ApplicationName { get; }

        string GaTrackingCode { get; }

        int ApprenticeshipProviderElements { get; }

        string ElasticsearchUsername { get; }

        string ElasticsearchPassword { get; }

        List<string> FrameworksExpiredRequired { get; }

        List<string> StandardsExpiredRequired { get; }

        int TakeMaximum { get; }
    }
}
