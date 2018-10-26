using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Services;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.PostCodeIo
{
    public class PostCodeIoLocator
    {
        private readonly IConfigurationSettings _configurationSettings;
        private readonly IHttpClient _httpClient;

        public PostCodeIoLocator(IConfigurationSettings configurationSettings, IHttpClient httpClient)
        {
            _configurationSettings = configurationSettings;
            _httpClient = httpClient;
        }

        public async Task<PostCodeResponse> GetLatLongFromPostcode(string postCode)
        {
            var uri = _configurationSettings.PostCodeUrl;

            var response = await _httpClient.GetAsync(new Uri(uri, postCode));

            return JsonConvert.DeserializeObject<PostCodeResponse>(response);
        }
    }
}
