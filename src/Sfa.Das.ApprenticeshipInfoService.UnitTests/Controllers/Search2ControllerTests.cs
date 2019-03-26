using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Api.Controllers;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.V2;
using SFA.DAS.NLog.Logger;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Controllers
{
    [TestFixture]
    public class Search2ControllerTests
    {
        private Search2Controller _sut;
        private Mock<IApprenticeshipSearchServiceV2> _apprenticeshipSearchServiceV2;

        [SetUp]
        public void Init()
        {
            _apprenticeshipSearchServiceV2 = new Mock<IApprenticeshipSearchServiceV2>();
            _sut = new Search2Controller(
                _apprenticeshipSearchServiceV2.Object,
                Mock.Of<ILog>());
        }

        [Test]
        public void SearchV2Return200StatusCodeOnSuccessfulSearch()
        {
            _apprenticeshipSearchServiceV2.Setup(x => x.SearchApprenticeships(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<int>>())).Returns(TestSearchResult());

            var result = _sut.SearchApprenticeshipsV2("admin", 2, 30, 3);

            result.Should().BeOfType<OkNegotiatedContentResult<ApprenticeshipSearchResults>>();
        }

        [Test]
        public void SearchV2PassesRequestParametersToSearchService()
        {
            _sut.SearchApprenticeshipsV2("admin", 2, 30, 3);

            _apprenticeshipSearchServiceV2.Verify(x => x.SearchApprenticeships("admin", 2, 30, 3, It.IsAny<List<int>>()));
        }

        [Test]
        public void SearchV2ExtractLevelsFromRequest()
        {
           _sut.SearchApprenticeshipsV2("admin", levels: "1,2,3,4");

           _apprenticeshipSearchServiceV2.Verify(x => x.SearchApprenticeships(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.Is<List<int>>(l => ContainsCorrectLevels(l))));
        }

        [TestCase("abc")]
        [TestCase("a,b,c")]
        [TestCase("1,3,a,4")]
        [TestCase(",3,4")]
        public void SearchV2ReturnsBadRequestWhenLevelFormatIsInvalid(string levels)
        {
            var result = _sut.SearchApprenticeshipsV2("admin", levels: levels);

            result.Should().BeOfType<BadRequestResult>();
        }

        private static ApprenticeshipSearchResults TestSearchResult()
        {
            return new ApprenticeshipSearchResults();
        }

        private static bool ContainsCorrectLevels(List<int> l)
        {
            var result = l.Intersect(new List<int> { 1, 2, 3, 4 }).Count() == 4;
            return result;
        }
    }
}
