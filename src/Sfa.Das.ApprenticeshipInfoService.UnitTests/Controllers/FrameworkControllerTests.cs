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
    public class FrameworkControllerTests
    {
        private FrameworksController _sut;
        private Mock<IGetFrameworks> _mockGetFrameworks;
        private Mock<ILog> _mockLogger;

        [SetUp]
        public void Init()
        {
            _mockGetFrameworks = new Mock<IGetFrameworks>();
            _mockLogger = new Mock<ILog>();

            _sut = new FrameworksController(_mockGetFrameworks.Object, _mockLogger.Object);
            _sut.Request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/frameworks")
            };
            _sut.Configuration = new HttpConfiguration();
            _sut.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
            _sut.Configuration.Routes.MapHttpRoute(
                name: "GetFrameworkProviders",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
            _sut.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary { { "controller", "frameworks" } });
        }

        [Test]
        public void ShouldReturnFrameworkNotFound()
        {
            _mockGetFrameworks.Setup(m => m.GetFrameworkById("1234")).Returns(new Framework { FrameworkId = "1234", Title = "test title" });

            ActualValueDelegate<object> test = () => _sut.Get("-2");

            Assert.That(test, Throws.TypeOf<HttpResponseException>());
        }

        [Test]
        public void ShouldReturnFramework()
        {
            _mockGetFrameworks.Setup(m => m.GetFrameworkById("1234")).Returns(new Framework { FrameworkId = "1234", Title = "test title" });

            var framework = _sut.Get("1234");

            Assert.NotNull(framework);
            framework.FrameworkId.Should().Be("1234");
            framework.Title.Should().Be("test title");
            framework.Uri.ToLower().Should().Be("http://localhost/frameworks/1234");
        }

        [Test]
        public void ShouldthrowExceptionWhenServiceisDown()
        {
            _mockGetFrameworks.Setup(
               x =>
                   x.GetAllFrameworks()).Throws<ApplicationException>();

            Assert.Throws<ApplicationException>(() => _sut.Head());
        }

        [Test]
        public void ShouldNotthrowExceptionWhenServiceisUp()
        {
            _mockGetFrameworks.Setup(
               x =>
                   x.GetAllFrameworks()).Returns(new List<FrameworkSummary> { new FrameworkSummary { Id = "0001" }, new FrameworkSummary { Id = "0002" } });

            Assert.DoesNotThrow(() => _sut.Head());
        }
    }
}
