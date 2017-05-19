﻿using System.Collections.Generic;
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

        public Task<Framework> GetAsync(int frameworkCode, int pathwayCode, int programmeType)
        {
            return GetAsync(ConvertToCompositeId(frameworkCode, pathwayCode, programmeType));
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

        public Task<IEnumerable<FrameworkSummary>> FindAllAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/frameworks"))
            {
                return RequestAndDeserialiseAsync<IEnumerable<FrameworkSummary>>(request);
            }
        }

        public bool Exists(string frameworkId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Head, $"/frameworks/{frameworkId}"))
            {
                using (var response = _httpClient.SendAsync(request))
                {
                    var result = response.Result;
                    if (result.StatusCode == HttpStatusCode.NoContent)
                    {
                        return true;
                    }
                    if (result.StatusCode == HttpStatusCode.NotFound)
                    {
                        return false;
                    }

                    RaiseResponseError(request, result);
                }

                return false;
            }
        }

        public async Task<bool> ExistsAsync(string frameworkId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Head, $"/frameworks/{frameworkId}"))
            {
                using (var response = await _httpClient.SendAsync(request))
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        return true;
                    }
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return false;
                    }

                    RaiseResponseError(request, response);
                }

                return false;
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
