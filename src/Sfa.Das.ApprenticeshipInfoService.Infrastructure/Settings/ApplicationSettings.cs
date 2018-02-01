namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Core.Configuration;
    using Microsoft.Azure;

    public sealed class ApplicationSettings : IConfigurationSettings
    {
        public string ApprenticeshipIndexAlias => ConfigurationManager.AppSettings["ApprenticeshipIndexAlias"];

        public string ProviderIndexAlias => ConfigurationManager.AppSettings["ProviderIndexAlias"];

        public string AssessmentOrgsIndexAlias => ConfigurationManager.AppSettings["AssessmentOrgsIndexAlias"];

        public IEnumerable<Uri> ElasticServerUrls => GetElasticSearchIps();

        public string EnvironmentName => ConfigurationManager.AppSettings["EnvironmentName"];

        public string ApplicationName => ConfigurationManager.AppSettings["ApplicationName"];

        public string GaTrackingCode => ConfigurationManager.AppSettings["ga.trackingid"];

        public int ApprenticeshipProviderElements => int.Parse(ConfigurationManager.AppSettings["ApprenticeshipProviderElements"]);

        public string ElasticsearchUsername => ConfigurationManager.AppSettings["ElasticsearchUsername"];

        public string ElasticsearchPassword => ConfigurationManager.AppSettings["ElasticsearchPassword"];
        public int TakeMaximum => int.Parse(ConfigurationManager.AppSettings["TakeMaximum"]);

        public int PageSizeApprenticeshipSummary => int.Parse(
            ConfigurationManager.AppSettings["PageSizeApprenticeshipSummary"]);

        public List<string> FrameworksExpiredRequired
        {
            get
            {
                return
                    CloudConfigurationManager.GetSetting("FrameworksExpiredRequired")
                        ?.Split(',')
                        .Where(s => s != string.Empty).Select(x => x.Trim()).ToList()
                    ?? new List<string>();
            }
        }

        public List<string> StandardsExpiredRequired
        {
            get
            {
                return
                    CloudConfigurationManager.GetSetting("StandardsExpiredRequired")
                        ?.Split(',')
                        .Where(s => s != string.Empty).Select(x => x.Trim()).ToList()
                    ?? new List<string>();
            }
        }

        private IEnumerable<Uri> GetElasticSearchIps()
        {
            var urlStrings = CloudConfigurationManager.GetSetting("ElasticServerUrls").Split(',');
            return urlStrings.Select(url => new Uri(url));
        }
    }
}
