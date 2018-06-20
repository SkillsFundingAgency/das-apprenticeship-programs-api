using System.Linq;
using System.Net;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Routing;
    using Api.Controllers;
    using Core.Services;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;
    using SFA.DAS.Apprenticeships.Api.Types;
    using SFA.DAS.NLog.Logger;
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class StandardControllerTests
    {
        private StandardsController _sut;
        private Mock<IGetStandards> _mockGetStandards;
        private Mock<ILog> _mockLogger;
        private DateTime? _lastDateForNewStarts;

        [SetUp]
        public void Init()
        {
            _lastDateForNewStarts = DateTime.Today.AddDays(100);
            _mockGetStandards = new Mock<IGetStandards>();
            _mockLogger = new Mock<ILog>();
            _sut = new StandardsController(_mockGetStandards.Object, _mockLogger.Object);
            _sut.Request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/standards")
            };
            _sut.Configuration = new HttpConfiguration();
            _sut.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
            _sut.Configuration.Routes.MapHttpRoute(
                name: "GetStandardProviders",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
            _sut.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary { { "controller", "standards" } });
        }

        [Test]
        public void ShouldReturnAllActiveStandards()
        {
            _mockGetStandards.Setup(m => m.GetAllStandards()).Returns(LoadStandardSummaryData());

            var standards = _sut.Get();

            Assert.NotNull(standards);
            standards.Count().Should().Be(2);
            standards.First().Id.Should().Be("2");
            standards.Last().Id.Should().Be("3");
            standards.First().LastDateForNewStarts.Should().Be(_lastDateForNewStarts);
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

            var ex = Assert.Throws<HttpResponseException>(() => _sut.Get("-2"));
            Assert.That(ex.Response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public void ShouldReturnStandard()
        {
            var todaysDate = DateTime.Today;
            _mockGetStandards.Setup(m => m.GetStandardById("42")).Returns(new Standard { StandardId = "42", Title = "test title", IsActiveStandard = true, LastDateForNewStarts = todaysDate});
            var standard = _sut.Get("42");

            Assert.NotNull(standard);
            standard.StandardId.Should().Be("42");
            standard.Title.Should().Be("test title");
            standard.Uri.ToLower().Should().Be("http://localhost/standards/42");
            standard.LastDateForNewStarts.Should().Be(todaysDate);
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
