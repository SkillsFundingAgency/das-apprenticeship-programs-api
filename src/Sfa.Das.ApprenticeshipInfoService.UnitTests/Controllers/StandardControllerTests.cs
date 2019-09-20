using System.Linq;
using System.Net;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
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
    public class StandardControllerTests
    {
        private StandardsController _sut;
        private Mock<IGetStandards> _mockGetStandards;
        private Mock<ILogger<StandardsController>> _mockLogger;
        private Mock<IUrlHelper> _mockUrlHelper;
        private DateTime? _lastDateForNewStarts;

        [SetUp]
        public void Init()
        {
            _lastDateForNewStarts = DateTime.Today.AddDays(100);
            _mockGetStandards = new Mock<IGetStandards>();
            _mockLogger = new Mock<ILogger<StandardsController>>();
            _mockUrlHelper = new Mock<IUrlHelper>();
            _mockUrlHelper.Setup(x => x.Link("GetStandardById", It.IsAny<object>())).Returns<string, dynamic>((a, b) => { var o = DynamicObjectHelper.ToExpandoObject(b); return $"http://localhost/standards/{o.id}"; });


            _sut = new StandardsController(_mockGetStandards.Object, _mockLogger.Object);
            _sut.Url = _mockUrlHelper.Object;
        }

        [Test]
        public void ShouldReturnAllActiveStandards()
        {
            _mockGetStandards.Setup(m => m.GetAllStandards()).Returns(LoadStandardSummaryData());

            var standards = _sut.Get();

            Assert.NotNull(standards.Value);
            standards.Value.Count().Should().Be(2);
            standards.Value.First().Id.Should().Be("2");
            standards.Value.Last().Id.Should().Be("3");
            standards.Value.First().LastDateForNewStarts.Should().Be(_lastDateForNewStarts);
        }

        private IEnumerable<StandardSummary> LoadStandardSummaryData()
        {
            return new List<StandardSummary>
            {
                new StandardSummary
                {
                    Id = "1",
                    Title = "test title",
                    IsActiveStandard = false
                },
                new StandardSummary
                {
                    Id = "2",
                    Title = "test title 2",
                    IsActiveStandard = true,
                    LastDateForNewStarts = _lastDateForNewStarts
                },
                new StandardSummary
                {
                    Id = "3",
                    Title = "test title 3",
                    IsActiveStandard = true
                }
            };
        }

        [Test]
        public void ShouldReturnStandardNotFound()
        {
            _mockGetStandards.Setup(m => m.GetStandardById("42")).Returns(new Standard { StandardId = "42", Title = "test title" });

            var result = _sut.Get("-2");
            result.Result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Test]
        public void ShouldReturnStandard()
        {
            var todaysDate = DateTime.Today;
            _mockGetStandards.Setup(m => m.GetStandardById("42")).Returns(new Standard { StandardId = "42", Title = "test title", IsActiveStandard = true, LastDateForNewStarts = todaysDate});
            var standard = _sut.Get("42");

            Assert.NotNull(standard.Value);
            standard.Value.StandardId.Should().Be("42");
            standard.Value.Title.Should().Be("test title");
            standard.Value.Uri.ToLower().Should().Be("http://localhost/standards/42");
            standard.Value.LastDateForNewStarts.Should().Be(todaysDate);
        }

        [Test]
        public void ShouldThrowExceptionWhenServiceisDown()
        {
            _mockGetStandards.Setup(
               x =>
                   x.GetAllStandards()).Throws<ApplicationException>();

            Assert.Throws<ApplicationException>(() => _sut.Head());
        }

        [Test]
        public void ShouldNotThrowExceptionWhenServiceisUp()
        {
            _mockGetStandards.Setup(
               x =>
                   x.GetAllStandards()).Returns(new List<StandardSummary> { new StandardSummary { Id = "401" }, new StandardSummary { Id = "52" } });

            Assert.DoesNotThrow(() => _sut.Head());
        }
    }
}
