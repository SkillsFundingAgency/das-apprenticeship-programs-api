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
        public string ApprenticeshipIndexAlias => CloudConfigurationManager.GetSetting("ApprenticeshipIndexAlias");

        public string ProviderIndexAlias => CloudConfigurationManager.GetSetting("ProviderIndexAlias");

        public string AssessmentOrgsIndexAlias => CloudConfigurationManager.GetSetting("AssessmentOrgsIndexAlias");

        public IEnumerable<Uri> ElasticServerUrls => GetElasticSearchIps();

        public string EnvironmentName => CloudConfigurationManager.GetSetting("EnvironmentName");

        public string ApplicationName => ConfigurationManager.AppSettings["ApplicationName"];

        public string GaTrackingCode => CloudConfigurationManager.GetSetting("ga.trackingid");

        public int ApprenticeshipProviderElements => int.Parse(ConfigurationManager.AppSettings["ApprenticeshipProviderElements"]);

        public string ElasticsearchUsername => CloudConfigurationManager.GetSetting("ElasticsearchUsername");

        public string ElasticsearchPassword => CloudConfigurationManager.GetSetting("ElasticsearchPassword");
        public string IfaStandardApiUrl => ConfigurationManager.AppSettings["IFAStandardsApiUrl"];

        public int PageSizeApprenticeshipSummary => int.Parse(
            ConfigurationManager.AppSettings["PageSizeApprenticeshipSummary"]);
		
        private IEnumerable<Uri> GetElasticSearchIps()
        {
            var urlStrings = CloudConfigurationManager.GetSetting("ElasticServerUrls").Split(',');
            return urlStrings.Select(url => new Uri(url));
        }

      }
}
