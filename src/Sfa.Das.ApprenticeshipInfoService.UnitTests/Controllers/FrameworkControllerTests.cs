using System.Net;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Api.Controllers;
    using Core.Services;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using Sfa.Das.ApprenticeshipInfoService.UnitTests.Helpers;
    using SFA.DAS.Apprenticeships.Api.Types;
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class FrameworkControllerTests
    {
        private FrameworksController _sut;
        private Mock<IGetFrameworks> _mockGetFrameworks;
        private Mock<ILogger<FrameworksController>> _mockLogger;
        private Mock<IUrlHelper> _mockUrlHelper;

        [SetUp]
        public void Init()
        {
            _mockGetFrameworks = new Mock<IGetFrameworks>();
            _mockLogger = new Mock<ILogger<FrameworksController>>();
            _mockUrlHelper = new Mock<IUrlHelper>();
            _mockUrlHelper.Setup(x => x.Link("GetFrameworkById", It.IsAny<object>())).Returns<string, dynamic>((a, b) => { var o = DynamicObjectHelper.ToExpandoObject(b); return $"http://localhost/frameworks/{o.id}"; });
            _mockUrlHelper.Setup(x => x.Link("GetByFrameworkCode", It.IsAny<object>())).Returns<string, dynamic>((a, b) => { var o = DynamicObjectHelper.ToExpandoObject(b); return $"http://localhost/frameworks/codes/{o.frameworkCode}"; });

            _sut = new FrameworksController(_mockGetFrameworks.Object, _mockLogger.Object);
            _sut.Url = _mockUrlHelper.Object;
        }

        [Test]
        public void ShouldReturnAllActiveFrameworks()
        {
            _mockGetFrameworks.Setup(m => m.GetAllFrameworks()).Returns(LoadFrameworkSummaryData());

            var frameworks = _sut.Get();

            Assert.NotNull(frameworks);
            frameworks.Value.Count().Should().Be(2);
            frameworks.Value.First().FrameworkCode.Should().Be(1234);
            frameworks.Value.Last().FrameworkCode.Should().Be(1236);
        }

        [Test]
        public void ShouldReturnFrameworkNotFound()
        {
            _mockGetFrameworks.Setup(m => m.GetFrameworkById("1234")).Returns(new Framework { FrameworkId = "1234", Title = "test title" });

            var result = _sut.Get("-2");
            result.Result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Test]
        public void ShouldReturnFramework()
        {
            _mockGetFrameworks.Setup(m => m.GetFrameworkById("1234")).Returns(new Framework { FrameworkId = "1234", Title = "test title",FrameworkName = "Framework Title", PathwayName = "Pathway Name", IsActiveFramework = true});

            var framework = _sut.Get("1234");

            Assert.NotNull(framework.Value);
            framework.Value.FrameworkId.Should().Be("1234");
            framework.Value.Title.Should().Be("test title");
            framework.Value.Uri.ToLower().Should().Be("http://localhost/frameworks/1234");
        }

        [Test]
        public void ShouldReturnFrameworkCodes()
        {
            _mockGetFrameworks.Setup(m => m
                .GetAllFrameworkCodes())
                .Returns(
                    new List<FrameworkCodeSummary>
                    {
                        new FrameworkCodeSummary
                        {
                            FrameworkCode = 1234,
                            Title = "test title"
                        }
                    });

            var frameworks = _sut.GetAllFrameworkCodes();

            Assert.NotNull(frameworks.Value);
            frameworks.Value.First().FrameworkCode.Should().Be(1234);
            frameworks.Value.First().Title.Should().Be("test title");
            frameworks.Value.First().Uri.ToLower().Should().Be("http://localhost/frameworks/codes/1234");
        }

        [Test]
        public void ShouldReturnFrameworkCode()
        {
            _mockGetFrameworks.Setup(m => m.GetFrameworkByCode(1234)).Returns(new FrameworkCodeSummary { FrameworkCode = 1234, Title = "test title" });

            var frameworkCodeSummary = _sut.GetByFrameworkCode(1234);

            Assert.NotNull(frameworkCodeSummary.Value);
            frameworkCodeSummary.Value.FrameworkCode.Should().Be(1234);
            frameworkCodeSummary.Value.Title.Should().Be("test title");
            frameworkCodeSummary.Value.Uri.ToLower().Should().Be("http://localhost/frameworks/codes/1234");
        }

        [Test]
        public void ShouldReturnFrameworkCodeNotFound()
        {
            _mockGetFrameworks.Setup(m => m.GetFrameworkByCode(1234)).Returns(new FrameworkCodeSummary() { FrameworkCode = 1234, Title = "test title" });

            var result =_sut.GetByFrameworkCode(-2);
            result.Result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Test]
        public void ShouldThrowExceptionWhenServiceIsDown()
        {
            _mockGetFrameworks.Setup(
               x =>
                   x.GetAllFrameworks()).Throws<ApplicationException>();

            Assert.Throws<ApplicationException>(() => _sut.Head());
        }

        [Test]
        public void ShouldNotThrowExceptionWhenServiceIsUp()
        {
            _mockGetFrameworks.Setup(
               x =>
                   x.GetAllFrameworks()).Returns(new List<FrameworkSummary> { new FrameworkSummary { Id = "0001" }, new FrameworkSummary { Id = "0002" } });

            Assert.DoesNotThrow(() => _sut.Head());
        }

        private IEnumerable<FrameworkSummary> LoadFrameworkSummaryData()
        {
            return new List<FrameworkSummary>
            {
                new FrameworkSummary
                {
                    FrameworkCode = 1234,
                    Title = "test title",
                    IsActiveFramework = true
                },
                new FrameworkSummary
                {
                    FrameworkCode = 1235,
                    Title = "test title 2",
                    IsActiveFramework = false
                },
                new FrameworkSummary
                {
                    FrameworkCode = 1236,
                    Title = "test title 3",
                    IsActiveFramework = true
                }
            };
        }
    }
}
