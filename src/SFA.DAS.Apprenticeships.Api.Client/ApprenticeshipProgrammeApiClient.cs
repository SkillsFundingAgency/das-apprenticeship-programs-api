using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.enums;

namespace SFA.DAS.Apprenticeships.Api.Client
{
    public class ApprenticeshipProgrammeApiClient : ApiClientBase, IApprenticeshipProgrammeApiClient
	{
        public ApprenticeshipProgrammeApiClient(string baseUri = null) : base(baseUri)
        {
        }
		
        public IEnumerable<ApprenticeshipSummary> Get()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/apprenticeship-programmes"))
            {
                return RequestAndDeserialise<IEnumerable<ApprenticeshipSummary>>(request);
            }
        }

		public async Task<IEnumerable<ApprenticeshipSummary>> GetAsync()
		{
			using (var request = new HttpRequestMessage(HttpMethod.Get, $"/apprenticeship-programmes"))
			{
				return await RequestAndDeserialiseAsync<IEnumerable<ApprenticeshipSummary>>(request);
			}
		}

		public IEnumerable<ApprenticeshipSearchResultsItem> Search(string keywords, int page = 1)
		{
			using (var request = new HttpRequestMessage(HttpMethod.Get, $"/apprenticeship-programmes/search?keywords={keywords}&page={page}"))
			{
				return RequestAndDeserialise<IEnumerable<ApprenticeshipSearchResultsItem>>(request);
			}
		}

		public async Task<IEnumerable<ApprenticeshipSearchResultsItem>> SearchAsync(string keywords, int page = 1)
		{
			using (var request = new HttpRequestMessage(HttpMethod.Get, $"/apprenticeship-programmes/search?keywords={keywords}&page={page}"))
			{
				return await RequestAndDeserialiseAsync<IEnumerable<ApprenticeshipSearchResultsItem>>(request);
			}
		}

        private string GetLevelsAsCommaSeparatedString(IEnumerable<int> levels)
        {
            if (levels == null)
                return null;

            return string.Join("%2C%20", levels);
        }
    }
}