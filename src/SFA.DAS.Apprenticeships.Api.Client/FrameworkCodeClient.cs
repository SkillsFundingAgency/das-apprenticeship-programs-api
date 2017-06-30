using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.Apprenticeships.Api.Client
{
    public class FrameworkCodeClient : ApiClientBase, IFrameworkCodeClient
    {
        public FrameworkCodeClient(string baseUri = null) : base(baseUri)
        {
        }

        public IEnumerable<FrameworkCodeSummary> FindAll()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/frameworks/codes"))
            {
                return RequestAndDeserialise<IEnumerable<FrameworkCodeSummary>>(request);
            }
        }

        public async Task<IEnumerable<FrameworkCodeSummary>> FindAllAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/frameworks/codes"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<FrameworkCodeSummary>>(request);
            }
        }

        public FrameworkCodeSummary Get(int frameworkCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/frameworks/codes/{frameworkCode}"))
            {
                return RequestAndDeserialise<FrameworkCodeSummary>(request);
            }
        }

        public FrameworkCodeSummary Get(string frameworkCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/frameworks/codes/{frameworkCode}"))
            {
                return RequestAndDeserialise<FrameworkCodeSummary>(request);
            }
        }

        public async Task<FrameworkCodeSummary> GetAsync(int frameworkCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/frameworks/codes/{frameworkCode}"))
            {
                return await RequestAndDeserialiseAsync<FrameworkCodeSummary>(request);
            }
        }

        public async Task<FrameworkCodeSummary> GetAsync(string frameworkCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/frameworks/codes/{frameworkCode}"))
            {
                return await RequestAndDeserialiseAsync<FrameworkCodeSummary>(request);
            }
        }

        public bool Exists(int frameworkCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Head, $"/frameworks/codes/{frameworkCode}"))
            {
                return Exists(request);
            }
        }

        public bool Exists(string frameworkCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Head, $"/frameworks/codes/{frameworkCode}"))
            {
                return Exists(request);
            }
        }

        public async Task<bool> ExistsAsync(int frameworkCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Head, $"/frameworks/codes/{frameworkCode}"))
            {
                return await ExistsAsync(request);
            }
        }

        public async Task<bool> ExistsAsync(string frameworkCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Head, $"/frameworks/codes/{frameworkCode}"))
            {
                return await ExistsAsync(request);
            }
        }
    }
}
