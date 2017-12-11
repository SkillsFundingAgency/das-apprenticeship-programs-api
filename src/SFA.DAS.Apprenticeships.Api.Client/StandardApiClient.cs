using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.Apprenticeships.Api.Client
{
    public class StandardApiClient : ApiClientBase, IStandardApiClient
    {
        public StandardApiClient(string baseUri = null) : base(baseUri)
        {
        }

        public Standard Get(int standardCode)
        {
            return Get(standardCode.ToString());
        }

        public async Task<Standard> GetAsync(int standardCode)
        {
            return await GetAsync(standardCode.ToString());
        }

        public Standard Get(string standardCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/standards/{standardCode}"))
            {
                return RequestAndDeserialise<Standard>(request, $"Could not find the standard {standardCode}");
            }
        }

        public async Task<Standard> GetAsync(string standardCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/standards/{standardCode}"))
            {
                return await RequestAndDeserialiseAsync<Standard>(request, $"Could not find the standard {standardCode}");
            }
        }

        public bool Exists(int standardCode)
        {
            return Exists(standardCode.ToString());
        }

        public async Task<bool> ExistsAsync(int standardCode)
        {
            return await ExistsAsync(standardCode.ToString());
        }

        public bool Exists(string standardCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Head, $"/standards/{standardCode}"))
            {
                return Exists(request);
            }
        }

        public async Task<bool> ExistsAsync(string standardCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Head, $"/standards/{standardCode}"))
            {
                return await ExistsAsync(request);
            }
        }

        /// <summary>
        /// Get a collection of standards
        /// GET /standards
        /// </summary>
        /// <returns>a collection of standard summaries</returns>
        public IEnumerable<StandardSummary> FindAll()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, "/standards"))
            {
                return RequestAndDeserialise<IEnumerable<StandardSummary>>(request);
            }
        }

        public async Task<IEnumerable<StandardSummary>> FindAllAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, "/standards"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<StandardSummary>>(request);
            }
        }
    }
}