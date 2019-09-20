using System.Diagnostics;
using System.Linq;
using System.Net;
using Elasticsearch.Net;
using Nest;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch
{
    public sealed class ElasticsearchClientFactory : IElasticsearchClientFactory
    {
        private ElasticClient _client;

        private readonly IConfigurationSettings _applicationSettings;

        public ElasticsearchClientFactory(IConfigurationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        public IElasticClient Create()
        {
            if (_client == null)
            {
                var settings = new ConnectionSettings(new SingleNodeConnectionPool(_applicationSettings.ElasticServerUrls.First()));
                SetDefaultSettings(settings);

                _client = new ElasticClient(settings);
            }

            return _client;
        }

        private void SetDefaultSettings(ConnectionSettings settings)
        {
            if (HasAuthenticationSettings())
            {
                settings.BasicAuthentication(_applicationSettings.ElasticsearchUsername, _applicationSettings.ElasticsearchPassword);
            }

            settings.DisableDirectStreaming();
        }

        private bool HasAuthenticationSettings()
        {
            if (string.IsNullOrWhiteSpace(_applicationSettings.ElasticsearchUsername) && string.IsNullOrWhiteSpace(_applicationSettings.ElasticsearchPassword))
                return false;
            else 
                return true;
        }
    }
}
