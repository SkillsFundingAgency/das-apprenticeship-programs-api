using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;

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
    }
}