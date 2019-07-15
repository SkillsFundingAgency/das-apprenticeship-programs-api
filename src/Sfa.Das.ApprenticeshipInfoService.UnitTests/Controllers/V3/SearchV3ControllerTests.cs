using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Api.Controllers.V3;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using SFA.DAS.Apprenticeships.Api.Types.V3;
using SFA.DAS.NLog.Logger;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Controllers.V3
{
    [TestFixture]
    public class SearchV3ControllerTests
    {
        private SearchV3Controller _sut;
        private Mock<IApprenticeshipSearchServiceV3> _apprenticeshipSearchServiceV3;

        [SetUp]
        public void Init()
        {
            _apprenticeshipSearchServiceV3 = new Mock<IApprenticeshipSearchServiceV3>();
            _sut = new SearchV3Controller(
                _apprenticeshipSearchServiceV3.Object,
                Mock.Of<ILog>());
        }

        [Test]
        public void SearchV3Return200StatusCodeOnSuccessfulSearch()
        {
            _apprenticeshipSearchServiceV3.Setup(x => x.SearchApprenticeships(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<int>>())).Returns(TestSearchResult());

            var result = _sut.SearchApprenticeships("admin", 2, 30, 3);

            result.Should().BeOfType<OkNegotiatedContentResult<ApprenticeshipSearchResults>>();
        }

        [Test]
        public void SearchV3PassesRequestParametersToSearchService()
        {
            _sut.SearchApprenticeships("admin", 2, 30, 3);

            _apprenticeshipSearchServiceV3.Verify(x => x.SearchApprenticeships("admin", 2, 30, 3, It.IsAny<List<int>>()));
        }

        [Test]
        public void SearchV3ExtractLevelsFromRequest()
        {
           _sut.SearchApprenticeships("admin", levels: "1,2,3,4");

           _apprenticeshipSearchServiceV3.Verify(x => x.SearchApprenticeships(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.Is<List<int>>(l => ContainsCorrectLevels(l))));
        }

        [TestCase("abc")]
        [TestCase("a,b,c")]
        [TestCase("1,3,a,4")]
        [TestCase(",3,4")]
        public void SearchV3ReturnsBadRequestWhenLevelFormatIsInvalid(string levels)
        {
            var result = _sut.SearchApprenticeships("admin", levels: levels);

            result.Should().BeOfType<BadRequestResult>();
        }

        [Test]
        public void ApprenticeshipsAutocompleteV3Return200StatusCodeOnSuccessfulSearch()
        {
            _apprenticeshipSearchServiceV3.Setup(x => x.SearchApprenticeships(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<int>>())).Returns(TestSearchResult());

            var result = _sut.ApprenticeshipsAutocomplete("admin");

            result.Should().BeOfType<OkNegotiatedContentResult<ApprenticeshipAutocompleteSearchResults>>();
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
