using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
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
            return Get(frameworkCode.ToString());
        }

        public async Task<FrameworkCodeSummary> GetAsync(int frameworkCode)
        {
            return await GetAsync(frameworkCode.ToString());
        }

        public bool Exists(int frameworkCode)
        {
            return Exists(frameworkCode.ToString());
        }

        public async Task<bool> ExistsAsync(int frameworkCode)
        {
            return await ExistsAsync(frameworkCode.ToString());
        }
    }
}
