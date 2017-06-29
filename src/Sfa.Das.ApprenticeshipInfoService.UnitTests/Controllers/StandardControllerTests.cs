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

        [SetUp]
        public void Init()
        {
            _mockGetStandards = new Mock<IGetStandards>();
            _mockLogger = new Mock<ILog>();
            _mockGetStandards.Setup(m => m.GetStandardById("42")).Returns(new Standard { StandardId = "42", Title = "test title" });
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
        public void ShouldReturnStandardkNotFound()
        {
            ActualValueDelegate<object> test = () => _sut.Get("-2");

            Assert.That(test, Throws.TypeOf<HttpResponseException>());
        }

        [Test]
        public void ShouldReturnStandard()
        {
            var standard = _sut.Get("42");

            Assert.NotNull(standard);
            standard.StandardId.Should().Be("42");
            standard.Title.Should().Be("test title");
            standard.Uri.ToLower().Should().Be("http://localhost/standards/42");
        }

        [Test]
        public void ShouldthrowExceptionWhenServiceisDown()
        {
            _mockGetStandards.Setup(
               x =>
                   x.GetAllStandards()).Throws<ApplicationException>();

            Assert.Throws<ApplicationException>(() => _sut.Head());
        }

        [Test]
        public void ShouldNotthrowExceptionWhenServiceisUp()
        {
            _mockGetStandards.Setup(
               x =>
                   x.GetAllStandards()).Returns(new List<StandardSummary> { new StandardSummary { Id = "401" }, new StandardSummary { Id = "52" } });

            Assert.DoesNotThrow(() => _sut.Head());
        }
    }
}
