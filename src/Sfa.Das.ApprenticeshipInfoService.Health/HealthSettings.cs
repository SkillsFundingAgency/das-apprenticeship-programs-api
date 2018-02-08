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

        public string LarsSiteRootUrl => CloudConfigurationManager.GetSetting("LarsSiteRootUrl");

        public string LarsSiteDownloadsPageUrl => CloudConfigurationManager.GetSetting("LarsSiteDownloadsPageUrl");

        public string CourseDirectoryUrl => CloudConfigurationManager.GetSetting("CourseDirectoryUrl");

        public string UkrlpUrl => CloudConfigurationManager.GetSetting("UKRLP_EndpointUri");

        private IEnumerable<Uri> GetElasticSearchIps(string configString)
        {
            var urlStrings = CloudConfigurationManager.GetSetting(configString).Split(',');
            return urlStrings.Select(url => new Uri(url));
        }

        private IEnumerable<string> GetElasticRequiredIndexAliases(string requiredIndexAliases)
        {
            return CloudConfigurationManager.GetSetting(requiredIndexAliases).Split(',');
        }
    }
}
