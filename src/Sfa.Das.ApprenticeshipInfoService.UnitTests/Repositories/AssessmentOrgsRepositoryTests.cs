using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Nest;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Application.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Repositories
{
    public class AssessmentOrgsRepositoryTests
    {
        private AssessmentOrgsRepository _sut;
        private Mock<IElasticsearchCustomClient> _elasticCustomClientMock;
        private Mock<IConfigurationSettings> _configurationSettingsMock;
        private Mock<IAssessmentOrgsMapping> _assessmentOrgsMappingMock;
        private Mock<IQueryHelper> _queryHelperMock;

        [SetUp]
        public void Init()
        {
            _elasticCustomClientMock = new Mock<IElasticsearchCustomClient>();
            _configurationSettingsMock = new Mock<IConfigurationSettings>();
            _assessmentOrgsMappingMock = new Mock<IAssessmentOrgsMapping>();
            _queryHelperMock = new Mock<IQueryHelper>();

            _sut = new AssessmentOrgsRepository(_elasticCustomClientMock.Object, _configurationSettingsMock.Object, new AssessmentOrgsMapping(), _queryHelperMock.Object);
        }

        [Test]
        public void ShouldReturnActiveStandardOrganisations()
        {
            var standardOrganisationDocuments = new List<StandardOrganisationDocument>
            {
                new StandardOrganisationDocument
                {
                    EpaOrganisation = "EPAorganisation",
                    EpaOrganisationIdentifier = "123456",
                    Email = "qwerty",
                    OrganisationType = "qwertyui",
                    Phone = "qwertyuio",
                    StandardCode = "1",
                    WebsiteLink = "www.abba.co.uk",
                    Address = new Address
                    {
                        Primary = "Primary",
                        Secondary = "Secondary",
                        Street = "Street",
                        Town = "Town",
                        Postcode = "Postcode"
                    },
                    EffectiveFrom = DateTime.Now.AddDays(-1),
                    EffectiveTo = default(DateTime)
                },
                new StandardOrganisationDocument
                {
                    EpaOrganisation = "EPAorganisation",
                    EpaOrganisationIdentifier = "123456",
                    Email = "qwerty",
                    OrganisationType = "qwertyui",
                    Phone = "qwertyuio",
                    StandardCode = "1",
                    WebsiteLink = "www.abba.co.uk",
                    Address = new Address
                    {
                        Primary = "Primary",
                        Secondary = "Secondary",
                        Street = "Street",
                        Town = "Town",
                        Postcode = "Postcode"
                    },
                    EffectiveFrom = DateTime.Now.AddDays(-2),
                    EffectiveTo = DateTime.Now.AddDays(1)
                },
                new StandardOrganisationDocument
                {
                    EpaOrganisation = "EPAorganisation",
                    EpaOrganisationIdentifier = "123456",
                    Email = "qwerty",
                    OrganisationType = "qwertyui",
                    Phone = "qwertyuio",
                    StandardCode = "1",
                    WebsiteLink = "www.abba.co.uk",
                    Address = new Address
                    {
                        Primary = "Primary",
                        Secondary = "Secondary",
                        Street = "Street",
                        Town = "Town",
                        Postcode = "Postcode"
                    },
                    EffectiveFrom = DateTime.Now.AddDays(-3),
                    EffectiveTo = DateTime.Now.AddDays(2)
                },
                new StandardOrganisationDocument
                {
                    EpaOrganisation = "EPAorganisation",
                    EpaOrganisationIdentifier = "123456",
                    Email = "qwerty",
                    OrganisationType = "qwertyui",
                    Phone = "qwertyuio",
                    StandardCode = "1",
                    WebsiteLink = "www.abba.co.uk",
                    Address = new Address
                    {
                        Primary = "Primary",
                        Secondary = "Secondary",
                        Street = "Street",
                        Town = "Town",
                        Postcode = "Postcode"
                    },
                    EffectiveFrom = DateTime.Now.AddDays(-4),
                    EffectiveTo = DateTime.Now.AddDays(-1)
                },
                new StandardOrganisationDocument
                {
                    EpaOrganisation = "EPAorganisation",
                    EpaOrganisationIdentifier = "123456",
                    Email = "qwerty",
                    OrganisationType = "qwertyui",
                    Phone = "qwertyuio",
                    StandardCode = "1",
                    WebsiteLink = "www.abba.co.uk",
                    Address = new Address
                    {
                        Primary = "Primary",
                        Secondary = "Secondary",
                        Street = "Street",
                        Town = "Town",
                        Postcode = "Postcode"
                    },
                    EffectiveFrom = DateTime.Now.AddDays(-4),
                    EffectiveTo = null
                }
            };

            var mockSearchResponse = new Mock<ISearchResponse<StandardOrganisationDocument>>();
            mockSearchResponse.Setup(x => x.Documents).Returns(standardOrganisationDocuments);
            mockSearchResponse.Setup(x => x.ApiCall.HttpStatusCode).Returns(200);

            _elasticCustomClientMock.
                Setup(x => x
                    .Search(It.IsAny<Func<SearchDescriptor<StandardOrganisationDocument>, ISearchRequest>>(), It.IsAny<string>()))
                .Returns(mockSearchResponse.Object);

            _queryHelperMock.Setup(x => x.GetOrganisationsAmountByStandardId(It.IsAny<string>())).Returns(4);

            var result = _sut.GetOrganisationsByStandardId("1").ToList();

            result.Count.Should().Be(3);
            result.Select(x => x.OrganisationType).Should().Contain("qwertyui");

        }
    }
}
