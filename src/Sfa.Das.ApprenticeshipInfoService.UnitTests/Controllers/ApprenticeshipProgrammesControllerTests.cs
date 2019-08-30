using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Api.Controllers;
    using Core.Services;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using SFA.DAS.Apprenticeships.Api.Types;
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class ApprenticeshipProgrammesControllerTests
    {
        private ApprenticeshipProgrammesController _sut;
        private Mock<IGetFrameworks> _mockGetFrameworks;
        private Mock<ILogger<ApprenticeshipProgrammesController>> _mockLogger;
        private Mock<IGetStandards> _mockGetStandards;
        private IApprenticeshipMapping _apprenticeshipMapping;

        [SetUp]
        public void Init()
        {
            _mockGetFrameworks = new Mock<IGetFrameworks>();
            _mockGetStandards = new Mock<IGetStandards>();
            _apprenticeshipMapping = new ApprenticeshipMapping();
            _mockLogger = new Mock<ILogger<ApprenticeshipProgrammesController>>();

            _sut = new ApprenticeshipProgrammesController(_mockGetFrameworks.Object, _mockGetStandards.Object, _apprenticeshipMapping, _mockLogger.Object);
            _sut.Url = new Mock<IUrlHelper>().Object;
        }

        [Test]
        public void ShouldReturnAllActiveApprenticeships()
        {
            _mockGetFrameworks.Setup(m => m.GetAllFrameworks()).Returns(LoadFrameworkSummaryData);
            _mockGetStandards.Setup(m => m.GetAllStandards()).Returns(LoadStandardSummaryData);

            var apprenticeshipSummaries = _sut.Get();

            Assert.NotNull(apprenticeshipSummaries);
            apprenticeshipSummaries.Value.Count().Should().Be(4);
            apprenticeshipSummaries.Value.First().Id.Should().Be("1234");
            apprenticeshipSummaries.Value.Last().Id.Should().Be("1239");
        }

        private IEnumerable<FrameworkSummary> LoadFrameworkSummaryData()
        {
            return new List<FrameworkSummary>
            {
                new FrameworkSummary
                {
                    Id = "1234",
                    Title = "test title",
                    IsActiveFramework = true
                },
                new FrameworkSummary
                {
                    Id = "1235",
                    Title = "test title 2",
                    IsActiveFramework = false
                },
                new FrameworkSummary
                {
                    Id = "1236",
                    Title = "test title 3",
                    IsActiveFramework = true
                }
            };
        }

        private IEnumerable<StandardSummary> LoadStandardSummaryData()
        {
            return new List<StandardSummary>
            {
                new StandardSummary
                {
                    Id = "1237",
                    Title = "test title",
                    IsActiveStandard = true
                },
                new StandardSummary
                {
                    Id = "1238",
                    Title = "test title 2",
                    IsActiveStandard = false
                },
                new StandardSummary
                {
                    Id = "1239",
                    Title = "test title 3",
                    IsActiveStandard = true
                }
            };
        }
    }
}
