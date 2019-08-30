using System;
using System.Collections.Generic;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Settings
{
    public sealed class ApplicationSettings : IConfigurationSettings
    {
        public string ApprenticeshipIndexAlias { get; set; }

        public string ProviderIndexAlias { get; set; }

        public string AssessmentOrgsIndexAlias { get; set; }

        public List<Uri> ElasticServerUrls { get; set; }

        public string EnvironmentName { get; set; }

        public string ApplicationName { get; set; }

        public string GaTrackingCode { get; set; }

        public int ApprenticeshipProviderElements { get; set; }

        public string ElasticsearchUsername { get; set; }

        public string ElasticsearchPassword { get; set; }

        public string IfaStandardApiUrl { get; set; }

        public int PageSizeApprenticeshipSummary { get; set; }

        public List<string> FrameworksExpiredRequired { get; set; }

        public List<string> StandardsExpiredRequired { get; set; }
      }
}
