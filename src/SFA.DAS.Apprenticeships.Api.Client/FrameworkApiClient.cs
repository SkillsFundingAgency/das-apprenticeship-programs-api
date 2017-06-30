using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.Apprenticeships.Api.Client
{
    public class FrameworkApiClient : ApiClientBase, IFrameworkApiClient
    {
        public FrameworkApiClient(string baseUri = null) : base(baseUri)
        {
        }

        public async Task<Framework> GetAsync(string frameworkId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/frameworks/{frameworkId}"))
            {
                return await RequestAndDeserialiseAsync<Framework>(request, $"Could not find the framework {frameworkId}");
            }
        }

        public Framework Get(string frameworkId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/frameworks/{frameworkId}"))
            {
                return RequestAndDeserialise<Framework>(request, $"Could not find the framework {frameworkId}");
            }
        }

        public async Task<Framework> GetAsync(int frameworkCode, int pathwayCode, int programmeType)
        {
            return await GetAsync(ConvertToCompositeId(frameworkCode, pathwayCode, programmeType));
        }

        public Framework Get(int frameworkCode, int pathwayCode, int programmeType)
        {
            return Get(ConvertToCompositeId(frameworkCode, pathwayCode, programmeType));
        }

        public IEnumerable<FrameworkSummary> FindAll()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/frameworks"))
            {
                return RequestAndDeserialise<IEnumerable<FrameworkSummary>>(request);
            }
        }

        public async Task<IEnumerable<FrameworkSummary>> FindAllAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/frameworks"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<FrameworkSummary>>(request);
            }
        }

        public IEnumerable<FrameworkCodeSummary> FindAllFrameworkCodes()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/frameworks/codes"))
            {
                return RequestAndDeserialise<IEnumerable<FrameworkCodeSummary>>(request);
            }
        }

        public async Task<IEnumerable<FrameworkCodeSummary>> FindAllFrameworkCodesAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/frameworks/codes"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<FrameworkCodeSummary>>(request);
            }
        }

        public async Task<FrameworkCodeSummary> GetFrameworkCodeAsync(string frameworkCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/frameworks/codes/{frameworkCode}"))
            {
                return await RequestAndDeserialiseAsync<FrameworkCodeSummary>(request, $"Could not find the framework code {frameworkCode}");
            }
        }

        public FrameworkCodeSummary GetFrameworkCode(string frameworkCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/frameworks/codes/{frameworkCode}"))
            {
                return RequestAndDeserialise<FrameworkCodeSummary>(request, $"Could not find the framework code {frameworkCode}");
            }
        }

        public bool Exists(string frameworkId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Head, $"/frameworks/{frameworkId}"))
            {
                return Exists(request);
            }
        }

        public async Task<bool> ExistsAsync(string frameworkId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Head, $"/frameworks/{frameworkId}"))
            {
                return await ExistsAsync(request);
            }
        }


        public bool Exists(int frameworkCode, int pathwayCode, int progamType)
        {
            return Exists(ConvertToCompositeId(frameworkCode, pathwayCode, progamType));
        }

        public Task<bool> ExistsAsync(int frameworkCode, int pathwayCode, int progamType)
        {
            return ExistsAsync(ConvertToCompositeId(frameworkCode, pathwayCode, progamType));
        }

        private static string ConvertToCompositeId(int frameworkCode, int pathwayCode, int progamType)
        {
            return $"{frameworkCode}-{progamType}-{pathwayCode}";
        }
    }
}
