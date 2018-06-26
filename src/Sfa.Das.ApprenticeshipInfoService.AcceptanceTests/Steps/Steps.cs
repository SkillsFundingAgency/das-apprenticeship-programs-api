using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.AcceptanceTests.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Sfa.Das.ApprenticeshipInfoService.AcceptanceTests.Steps
{
    [Binding]
    public class Steps
    {
        private HttpClientReqHelper getRequests;

        [Given(@"I send request to (.*)")]
        public void GivenISendRequestAssessment_OrganisationsToGetAllTheAssessmentOrganisations(string uri)
        {
            getRequests = new HttpClientReqHelper(uri);
            getRequests.ExecuteHttpGetRequest(getRequests.requestMessage).Wait();
        }

        [Then(@"I get response code (.*) is returned")]
        public void ThenIGetResponseCodeIsReturned(string code)
        {
            getRequests.EnsureAppropriateResponseCode(code);
        }

        [Then(@"I expect the amount of refult will be at least (.*)")]
        public async Task ThenIExpectResultWillBeReturned(int expectedCount)
        {
            var res = await getRequests.GetBody<IEnumerable<object>>();

            Assert.GreaterOrEqual(res.Count(), expectedCount);
        }
    }
}
