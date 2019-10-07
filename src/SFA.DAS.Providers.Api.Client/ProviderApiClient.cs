using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.Providers;

namespace SFA.DAS.Providers.Api.Client
{
    public class ProviderApiClient : ApiClientBase, IProviderApiClient
    {
        public ProviderApiClient(string baseUri = null) : base(baseUri)
        {
        }

        /// <summary>
        /// Get a provider details
        /// GET /providers/{provider-ukprn}
        /// </summary>
        /// <param name="providerUkprn">an integer for the provider ukprn</param>
        /// <returns>a provider details based on ukprn</returns>
        public Provider Get(long providerUkprn)
        {
            return Get(providerUkprn.ToString());
        }

        public async Task<Provider> GetAsync(long providerUkprn)
        {
            return await GetAsync(providerUkprn.ToString());
        }

        public Provider Get(int providerUkprn)
        {
            return Get(providerUkprn.ToString());
        }

        public async Task<Provider> GetAsync(int providerUkprn)
        {
            return await GetAsync(providerUkprn.ToString());
        }

		public Provider Get(string providerUkprn)
	    {
		    using (var request = new HttpRequestMessage(HttpMethod.Get, $"/providers/{providerUkprn}"))
		    {
			    return RequestAndDeserialise<Provider>(request, $"The provider {providerUkprn} could not be found");
		    }
	    }

	    public async Task<Provider> GetAsync(string providerUkprn)
	    {
		    using (var request = new HttpRequestMessage(HttpMethod.Get, $"/providers/{providerUkprn}"))
		    {
			    return await RequestAndDeserialiseAsync<Provider>(request, $"The provider {providerUkprn} could not be found");
		    }
	    }

	    public ProviderSearchResponseItem Search(string keywords, int page = 1)
	    {
		    using (var request = new HttpRequestMessage(HttpMethod.Get, $"/providers/search?keywords={keywords}&page={page}"))
		    {
			    return RequestAndDeserialise<ProviderSearchResponseItem>(request);
		    }
	    }

	    public async Task<ProviderSearchResponseItem> SearchAsync(string keywords, int page = 1)
	    {
		    using (var request = new HttpRequestMessage(HttpMethod.Get, $"/providers/search?keywords={keywords}&page={page}"))
		    {
			    return await RequestAndDeserialiseAsync<ProviderSearchResponseItem>(request);
		    }
	    }

		/// <summary>
		/// Check if a provider exists
		/// HEAD /frameworks/{provider-ukprn}
		/// </summary>
		/// <param name="providerUkprn">an integer for the provider ukprn</param>
		/// <returns>bool</returns>
		public bool Exists(long providerUkprn)
        {
            return Exists(providerUkprn.ToString());
        }

        public async Task<bool> ExistsAsync(long providerUkprn)
        {
            return await ExistsAsync(providerUkprn.ToString());
        }

        public bool Exists(int providerUkprn)
        {
            return Exists(providerUkprn.ToString());
        }

        public async Task<bool> ExistsAsync(int providerUkprn)
        {
            return await ExistsAsync(providerUkprn.ToString());
        }

        public bool Exists(string providerUkprn)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Head, $"/providers/{providerUkprn}"))
            {
                return Exists(request);
            }
        }

        public async Task<bool> ExistsAsync(string providerUkprn)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Head, $"/providers/{providerUkprn}"))
            {
                return await ExistsAsync(request);
            }
        }

        public IEnumerable<ProviderSummary> FindAll()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, "/providers"))
            {
                return RequestAndDeserialise<IEnumerable<ProviderSummary>>(request);
            }
        }

        public async Task<IEnumerable<ProviderSummary>> FindAllAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, "/providers"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<ProviderSummary>>(request);
            }
        }

        internal IEnumerable<StandardProvider> GetStandardProviders(string standardId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/providers/standard/{standardId}"))
            {
                return RequestAndDeserialise<IEnumerable<StandardProvider>>(request);
            }
        }

        internal async Task<IEnumerable<StandardProvider>> GetStandardProvidersAsync(string standardId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/providers/standard/{standardId}"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<StandardProvider>>(request);
            }
        }

        internal IEnumerable<StandardProvider> GetStandardProviders(int standardId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/providers/standard/{standardId}"))
            {
                return RequestAndDeserialise<IEnumerable<StandardProvider>>(request);
            }
        }

        internal async Task<IEnumerable<StandardProvider>> GetStandardProvidersAsync(int standardId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/providers/standard/{standardId}"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<StandardProvider>>(request);
            }
        }

        internal IEnumerable<FrameworkProvider> GetFrameworkProviders(string frameworkId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/providers/framework/{frameworkId}"))
            {
                return RequestAndDeserialise<IEnumerable<FrameworkProvider>>(request);
            }
        }

        internal async Task<IEnumerable<FrameworkProvider>> GetFrameworkProvidersAsync(string frameworkId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/providers/framework/{frameworkId}"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<FrameworkProvider>>(request);
            }
        }

        public ApprenticeshipTrainingSummary GetActiveApprenticeshipTrainingByProvider(long ukprn)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"providers/{ukprn}/active-apprenticeship-training"))
            {
                return RequestAndDeserialise<ApprenticeshipTrainingSummary>(request);
            }
        }

        public ApprenticeshipTrainingSummary GetActiveApprenticeshipTrainingByProvider(long ukprn, int pageNumber)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"providers/{ukprn}/active-apprenticeship-training/{pageNumber}"))
            {
                return RequestAndDeserialise<ApprenticeshipTrainingSummary>(request);
            }
        }

        public async Task<ApprenticeshipTrainingSummary> GetActiveApprenticeshipTrainingByProviderAsync(long ukprn)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"providers/{ukprn}/active-apprenticeship-training"))
            {
                return await RequestAndDeserialiseAsync<ApprenticeshipTrainingSummary>(request);
            }
        }

        public async Task<ApprenticeshipTrainingSummary> GetActiveApprenticeshipTrainingByProviderAsync(long ukprn, int pageNumber)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"providers/{ukprn}/active-apprenticeship-training/{pageNumber}"))
            {
                return await RequestAndDeserialiseAsync<ApprenticeshipTrainingSummary>(request);
            }
        }

        public async Task Ping()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"ping"))
            {
                using (var response = await _httpClient.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        RaiseResponseError("Error calling ping endpoint", request, response);
                    }
                }
            }
        }
    }
}