using FluentAssertions;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
using SFA.DAS.Apprenticeships.Api.Types.Pagination;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Helpers
{
    [TestFixture]
    public class PaginationHelperTests
    {
        [TestCase(1, 20, 50, 1, 3, 0)]
        [TestCase(2, 20, 50, 2, 3, 20)]
        [TestCase(3, 20, 50, 3, 3, 40)]
        [TestCase(4, 20, 50, 3, 3, 40)]
        [TestCase(4, 20, 60, 3, 3, 40)]
        [TestCase(4, 20, 61, 4, 4, 60)]
        [TestCase(50, 20, 61, 4, 4, 60)]
        [TestCase(50, 20, 59, 3, 3, 40)]
        [TestCase(0, 20, 19, 1, 1, 0)]
        [TestCase(0, 20, 20, 1, 1, 0)]
        [TestCase(1, 20, 21, 1, 2, 0)]
        [TestCase(2, 20, 21, 2, 2, 20)]
        public void ShouldCheckPaginationProcessesAreCorrect(int page, int pageSize, int totalCount, int expectedPage, int lastPage, int noOfRecordsToSkip)
        {

            var result = new PaginationHelper().GeneratePaginationDetails(page, pageSize,totalCount);
            var expectedResult = new PaginationDetails
            {
                Page = expectedPage,
                TotalCount = totalCount,
                LastPage = lastPage,
                NumberOfRecordsToSkip = noOfRecordsToSkip,
                NumberPerPage = pageSize
            };

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
