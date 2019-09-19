namespace Sfa.Das.ApprenticeshipInfoService.Core.Configuration
{
    using System;
    using System.Collections.Generic;

    public interface IConfigurationSettings
    {
        string ApprenticeshipIndexAlias { get; }

        string ProviderIndexAlias { get; }

        string AssessmentOrgsIndexAlias { get; }

        List<Uri> ElasticServerUrls { get; }

        int ApprenticeshipProviderElements { get; }

        string ElasticsearchUsername { get; }

        string ElasticsearchPassword { get; }

        List<string> FrameworksExpiredRequired { get; }

        List<string> StandardsExpiredRequired { get; }

        int PageSizeApprenticeshipSummary { get; }

        string IfaStandardApiUrl { get; }
    }
}
