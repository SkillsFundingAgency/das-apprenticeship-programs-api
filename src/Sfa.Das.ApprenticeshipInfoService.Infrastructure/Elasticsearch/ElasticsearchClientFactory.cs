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
        private readonly IConfigurationSettings _applicationSettings;

        public ElasticsearchClientFactory(IConfigurationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        public IElasticClient Create()
        {
            // TODO: LWA - Should this be the list of client/co-ordinating nodes
            using (var settings = new ConnectionSettings(new SingleNodeConnectionPool(_applicationSettings.ElasticServerUrls.First())))
            {
                SetDefaultSettings(settings);

                return new ElasticClient(settings);
            }
        }

        private void SetDefaultSettings(ConnectionSettings settings)
        {
            if (!Debugger.IsAttached)
            {
                settings.BasicAuthentication(_applicationSettings.ElasticsearchUsername, _applicationSettings.ElasticsearchPassword);
            }

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            settings.DisableDirectStreaming();
        }
    }
}
