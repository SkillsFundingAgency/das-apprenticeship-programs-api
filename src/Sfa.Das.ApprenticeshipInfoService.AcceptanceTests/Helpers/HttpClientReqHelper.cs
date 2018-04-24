using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Configuration;
using NUnit.Framework;

namespace Sfa.Das.ApprenticeshipInfoService.AcceptanceTests.Helpers
{
    public class HttpClientReqHelper
    {
        HttpClient client;
        private HttpResponseMessage response;
        public HttpRequestMessage requestMessage;

        public HttpClientReqHelper(string uri)
        {
            RunAsync(uri);
        }

        private void RunAsync(string uri)
        {
            var url = ConfigurationManager.AppSettings.Get("ApprenticeshipInfoService");
            client = new HttpClient { BaseAddress = new Uri(url) };
            requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            requestMessage.Headers.Add("Accept", "application/json");
        }

        public async Task<HttpResponseMessage> ExecuteHttpGetRequest(HttpRequestMessage request)
        {
            response = await client.SendAsync(request);
            return response;
        }

        public void EnsureAppropriateResponseCode(string code)
        {
            String StatusCode = response.StatusCode.ToString();
            Assert.AreEqual(code, StatusCode);
        }

        public async Task<T> GetBody<T>()
        {
            var s = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(s);
        }
    }
}