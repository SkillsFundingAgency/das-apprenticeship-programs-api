namespace Sfa.Das.ApprenticeshipInfoService.Health
{
    using Microsoft.Azure;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public class HealthSettings : IHealthSettings
    {
        public string Environment => CloudConfigurationManager.GetSetting("EnvironmentName");

        public IEnumerable<Uri> ElasticsearchUrls => GetElasticSearchIps("ElasticServerUrls");

        public IEnumerable<string> RequiredIndexAliases => GetElasticRequiredIndexAliases("RequiredIndexAliases");

        public string LarsSiteRootUrl => ConfigurationManager.AppSettings["LarsSiteRootUrl"];

        public string LarsSiteDownloadsPageUrl => ConfigurationManager.AppSettings["LarsSiteDownloadsPageUrl"];

        public string CourseDirectoryUrl => ConfigurationManager.AppSettings["CourseDirectoryUrl"];

        private IEnumerable<Uri> GetElasticSearchIps(string configString)
        {
            var urlStrings = CloudConfigurationManager.GetSetting(configString).Split(',');
            return urlStrings.Select(url => new Uri(url));
        }

        private IEnumerable<string> GetElasticRequiredIndexAliases(string requiredIndexAliases)
        {
            return ConfigurationManager.AppSettings[requiredIndexAliases].Split(',');
        }
    }
}
