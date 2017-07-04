using System.Net;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Routing;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;
    using Sfa.Das.ApprenticeshipInfoService.Api.Controllers;
    using Sfa.Das.ApprenticeshipInfoService.Core.Services;
    using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;
    using SFA.DAS.NLog.Logger;
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class AssessmentOrgsControllerTests
    {
        private AssessmentOrgsController _sut;
        private Mock<IGetAssessmentOrgs> _mockGetAssessmentOrgs;
        private Mock<ILog> _mockLogger;

        [OneTimeSetUp]
        public void Init()
        {
            _mockGetAssessmentOrgs = new Mock<IGetAssessmentOrgs>();
            _mockLogger = new Mock<ILog>();

            _sut = new AssessmentOrgsController(
                _mockGetAssessmentOrgs.Object,
                _mockLogger.Object);

            _sut.Request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/assessment-organisations")
            };
            _sut.Configuration = new HttpConfiguration();
            _sut.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
            _sut.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary { { "controller", "providers" } });
        }

        [Test]
        public void ShouldReturnAssessmentOrganisation()
        {
            var expected = new Organisation { Id = "EPA0001" };

            _mockGetAssessmentOrgs.Setup(
                x =>
                    x.GetOrganisationById("EPA0001")).Returns(expected);

            var actual = _sut.Get("EPA0001");

            actual.ShouldBeEquivalentTo(expected);
            actual.Uri.Should().Be("http://localhost/assessment-organisations/EPA0001");
        }

        [Test]
        public void ShouldReturnAssessmentOrganisationNotFound()
        {
            var expected = new Organisation();

            _mockGetAssessmentOrgs.Setup(
                x =>
                    x.GetOrganisationById("EPA0001")).Returns(expected);

            var ex = Assert.Throws<HttpResponseException>(() => _sut.Get("EPA0001x"));
            Assert.That(ex.Response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public void ShouldReturnValidListOfAssessmentOrganisations()
        {
            var element = new OrganisationSummary { Id = "EPA0001" };
            var expected = new List<OrganisationSummary> { element };

            _mockGetAssessmentOrgs.Setup(
                x =>
                    x.GetAllOrganisations()).Returns(expected);

            var response = _sut.Get();

            response.Should().NotBeNull();
            response.Should().BeOfType<List<OrganisationSummary>>();
            response.Should().NotBeEmpty();
            response.Should().BeEquivalentTo(expected);
            response.First().Should().NotBe(null);
            response.First().Should().Be(element);
            response.First().Id.Should().Be(element.Id);
            response.First().Uri.Should().Be("http://localhost/assessment-organisations/EPA0001");
        }

        [Test]
        public void ShouldThrowExceptionWhenServiceisDown()
        {
            _mockGetAssessmentOrgs.Setup(
               x =>
                   x.GetAllOrganisations()).Throws<ApplicationException>();

            Assert.Throws<ApplicationException>(() => _sut.Head());
        }

        [Test]
        public void ShouldNotThrowExceptionWhenServiceisUp()
        {
            _mockGetAssessmentOrgs.Setup(
               x =>
                   x.GetAllOrganisations()).Returns(new List<OrganisationSummary> { new OrganisationSummary { Id = "EPA0001" }, new OrganisationSummary { Id = "EPA0002" } });

            Assert.DoesNotThrow(() => _sut.Head());
        }
    }
}