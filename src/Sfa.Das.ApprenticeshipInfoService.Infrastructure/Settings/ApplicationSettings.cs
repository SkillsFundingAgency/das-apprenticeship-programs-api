namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Microsoft.Azure;
    using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;

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
        public List<string> FrameworksExpiredRequired => GetFrameworksExpiredList();

        private List<string> GetFrameworksExpiredList()
        {
            return CloudConfigurationManager.GetSetting("FrameworksExpiredRequired").Split(',').Where(s => s != string.Empty).Select(x => x.Trim()).ToList();
        }

        private IEnumerable<Uri> GetElasticSearchIps()
        {
            var urlStrings = CloudConfigurationManager.GetSetting("ElasticServerUrls").Split(',');
            return urlStrings.Select(url => new Uri(url));
        }
    }
}
