namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using Sfa.Das.ApprenticeshipInfoService.Api.Controllers;
    using Sfa.Das.ApprenticeshipInfoService.Core.Services;
    using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;
    using Assert = NUnit.Framework.Assert;
    using Sfa.Das.ApprenticeshipInfoService.UnitTests.Helpers;

    [TestFixture]
    public class AssessmentOrgsControllerTests
    {
        private AssessmentOrgsController _sut;
        private Mock<IGetAssessmentOrgs> _mockGetAssessmentOrgs;
        private Mock<ILogger<AssessmentOrgsController>> _mockLogger;
        private Mock<IUrlHelper> _mockUrlHelper;

        [SetUp]
        public void Init()
        {
            _mockGetAssessmentOrgs = new Mock<IGetAssessmentOrgs>();
            _mockLogger = new Mock<ILogger<AssessmentOrgsController>>();

            _mockUrlHelper = new Mock<IUrlHelper>();
            _mockUrlHelper.Setup(x => x.Link("GetAssessmentOrgById", It.IsAny<object>())).Returns<string, dynamic>((a, b) => { var o = DynamicObjectHelper.ToExpandoObject(b); return $"http://localhost/assessment-organisations/{o.id}"; });
            _mockUrlHelper.Setup(x => x.Link("GetStandardById", It.IsAny<object>())).Returns<string, dynamic>((a, b) => { var o = DynamicObjectHelper.ToExpandoObject(b); return $"http://localhost/Standards/{o.id}"; });
            _mockUrlHelper.Setup(x => x.Link("GetStandardsByOrganisationId", It.IsAny<object>())).Returns<string, dynamic>((a, b) => { var o = DynamicObjectHelper.ToExpandoObject(b); return $"http://localhost/assessment-organisations/{o.organisationId}/standards"; });

            _sut = new AssessmentOrgsController(
                _mockGetAssessmentOrgs.Object,
                _mockLogger.Object);

            _sut.Url = _mockUrlHelper.Object;
        }

        [Test]
        public void ShouldNotThrowErrorIfOrganisationNotFound()
        {
            _mockGetAssessmentOrgs.Setup(x => x.GetStandardsByOrganisationIdentifier(It.IsAny<string>())).Returns(new List<StandardOrganisationSummary>() { });
            Assert.DoesNotThrow(() => _sut.GetStandardsByOrganisationId("EPA0001x"));
        }

        [Test]
        public void ShouldNotThrowErrorIfStandardNotFound()
        {
            _mockGetAssessmentOrgs.Setup(x => x.GetOrganisationsByStandardId(It.IsAny<string>())).Returns(new List<Organisation>() { });
            Assert.DoesNotThrow(() => _sut.GetByStandardId("111"));
        }

        [Test]
        public void ShouldReturnAllStandardsForAnOrganisation()
        {
            var data = new List<StandardOrganisationSummary>()
            {
                new StandardOrganisationSummary
                {
                    StandardCode = "1",
                    Periods = new List<Period>()
                    {
                        new Period
                        {
                            EffectiveFrom = DateTime.Now.AddDays(-4),
                            EffectiveTo = null
                        }
                    }
                }
            };

            _mockGetAssessmentOrgs.Setup(x => x.GetStandardsByOrganisationIdentifier("EPA0005")).Returns(data);

            var result = _sut.GetStandardsByOrganisationId("EPA0005");

            result.Value.Should().BeEquivalentTo(data);
            result.Value.First().Uri.Should().Be("http://localhost/Standards/1");
        }

        [Test]
        public void ShouldReturnAllOrganisationForAStandard()
        {
            var data = new List<Organisation>()
            {
                new Organisation
                {
                    Id = "EPA123456"
                }
            };

            _mockGetAssessmentOrgs.Setup(x => x.GetOrganisationsByStandardId("5")).Returns(data);

            var result = _sut.GetByStandardId("5");

            result.Value.Should().BeEquivalentTo(data);
            result.Value.First().Uri.Should().Be("http://localhost/assessment-organisations/EPA123456");
            result.Value.First().Links.First().Title.Should().Be("Standards");
            result.Value.First().Links.First().Href.Should().Be("http://localhost/assessment-organisations/EPA123456/standards");
        }

        [Test]
        public void ShouldReturnAssessmentOrganisation()
        {
            var expected = new Organisation { Id = "EPA0001" };

            _mockGetAssessmentOrgs.Setup(
                x =>
                    x.GetOrganisationById("EPA0001")).Returns(expected);

            var actual = _sut.Get("EPA0001");

            actual.Value.Should().BeEquivalentTo(expected);
            actual.Value.Uri.Should().Be("http://localhost/assessment-organisations/EPA0001");
        }

        [Test]
        public void ShouldReturnAssessmentOrganisationNotFound()
        {
            var expected = new Organisation();

            _mockGetAssessmentOrgs.Setup(
                x =>
                    x.GetOrganisationById("EPA0001")).Returns(expected);

            var result = _sut.Get("EPA0001x");
            result.Result.Should().BeAssignableTo<NotFoundObjectResult>();
        }

        [Test]
        public void ShouldReturnValidListOfAssessmentOrganisations()
        {
            var ukprn = 10002000;
            var element = new OrganisationSummary { Id = "EPA0001", Ukprn = ukprn };
            var element2 = new OrganisationSummary { Id = "EPA0002"};

            var expected = new List<OrganisationSummary> { element, element2 };

            _mockGetAssessmentOrgs.Setup(
                x =>
                    x.GetAllOrganisations()).Returns(expected);

            var response = _sut.Get();

            response.Value.Should().NotBeNull();
            response.Value.Should().BeOfType<List<OrganisationSummary>>();
            response.Value.Should().NotBeEmpty();
            response.Value.Should().BeEquivalentTo(expected);
            response.Value.First().Should().NotBe(null);
            response.Value.First().Should().Be(element);
            response.Value.First().Id.Should().Be(element.Id);
            response.Value.First().Uri.Should().Be("http://localhost/assessment-organisations/EPA0001");
            response.Value.First().Ukprn.Should().Be(ukprn);
            response.Value.ToList()[1].Id.Should().Be(element2.Id);
            response.Value.ToList()[1].Ukprn.Should().Be(null);
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