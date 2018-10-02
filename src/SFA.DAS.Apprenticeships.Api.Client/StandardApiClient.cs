using System.Collections.Generic;
using System.Linq;
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
        /// Get all active standards
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

		/// <summary>
		/// Get all standards
		/// GET /standards/v2
		/// </summary>
		/// <returns>a collection of standard summaries</returns>
		public IEnumerable<StandardSummary> GetAll()
	    {
		    using (var request = new HttpRequestMessage(HttpMethod.Get, "/standards/v2"))
		    {
			    return RequestAndDeserialise<IEnumerable<StandardSummary>>(request);
		    }
	    }

	    public async Task<IEnumerable<StandardSummary>> GetAllAsync()
	    {
		    using (var request = new HttpRequestMessage(HttpMethod.Get, "/standards/v2"))
		    {
			    return await RequestAndDeserialiseAsync<IEnumerable<StandardSummary>>(request);
		    }
	    }

		public IEnumerable<Standard> GetStandardsById(List<int> ids, int page = 1)
	    {
			return GetStandardsById(ids.Select(id => id.ToString()).ToList(), page);
		}

	    public IEnumerable<Standard> GetStandardsById(List<string> ids, int page = 1)
	    {
		    using (var request = new HttpRequestMessage(HttpMethod.Get, $"/standards/getlistbyids?ids={string.Join("%2C%20", ids)}&page={page}"))
		    {
			    return RequestAndDeserialise<IEnumerable<Standard>>(request);
		    }
	    }

		public async Task<IEnumerable<Standard>> GetStandardsByIdAsync(List<int> ids, int page = 1)
		{
			return await GetStandardsByIdAsync(ids.Select(id => id.ToString()).ToList(), page);
		}

	    public async Task<IEnumerable<Standard>> GetStandardsByIdAsync(List<string> ids, int page = 1)
	    {
		    using (var request = new HttpRequestMessage(HttpMethod.Get, $"/standards/getlistbyids?ids={string.Join("%2C%20", ids)}&page={page}"))
		    {
			    return await RequestAndDeserialiseAsync<IEnumerable<Standard>>(request);
		    }
	    }
	}
}