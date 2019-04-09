using System.Collections.Generic;
using System.Linq;
using Bogus;
using FluentAssertions;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
using SFA.DAS.Apprenticeships.Api.Types.V3;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Infrastructure.Mapping
{
    [TestFixture]
    public class ProviderSearchResultItemMappingTests
    {
        private IProviderSearchResultItemMapping _sut;

        [SetUp]
        public void Init()
        {
            _sut = new ProviderSearchResultItemMapping();
        }

        [Test]
        public void ShouldPopulatePropertiesForStandard()
        {
            StandardProviderSearchResultsItem source = CreateTestSourceItem();

            var result = _sut.MapToApprenticeshipSearchResult(source);

            result.Ukprn.Should().Be(source.Ukprn);
            result.Location.ShouldBeEquivalentTo(source.TrainingLocations.First());
            result.ProviderName.Should().Be(source.ProviderName);
            result.OverallAchievementRate.Should().Be(source.OverallAchievementRate);
            result.NationalProvider.Should().Be(source.NationalProvider);
            result.DeliveryModes.Should().BeEquivalentTo(source.DeliveryModes);
            result.Distance.Should().Be(source.Distance);
            result.EmployerSatisfaction.Should().Be(source.EmployerSatisfaction);
            result.LearnerSatisfaction.Should().Be(source.LearnerSatisfaction);
            result.NationalOverallAchievementRate.Should().Be(source.NationalOverallAchievementRate);
            result.OverallCohort.Should().Be(source.OverallCohort);
            result.HasNonLevyContract.Should().Be(source.HasNonLevyContract);
            result.IsLevyPayerOnly.Should().Be(source.IsLevyPayerOnly);
            result.CurrentlyNotStartingNewApprentices.Should().Be(source.CurrentlyNotStartingNewApprentices);
        }

        private StandardProviderSearchResultsItem CreateTestSourceItem()
        {
            var fakeResult = new Faker<StandardProviderSearchResultsItem>()
                .StrictMode(false)
                .Rules((f, o) =>
                {
                    o.ApprenticeshipInfoUrl = f.Internet.Email();
                    o.ApprenticeshipMarketingInfo = f.Lorem.Sentences(2);
                    o.ContactUsUrl = f.Internet.Url();
                    o.CurrentlyNotStartingNewApprentices = f.Random.Bool();
                    o.DeliveryModes = f.Random.ListItems(new[] { "deliveryMode1", "deliveryMode2", "deliveryMode3" });
                    o.Distance = f.Random.Double(0, 40);
                    o.Email = f.Internet.Email();
                    o.EmployerSatisfaction = f.Random.Double(0, 100);
                    o.HasNonLevyContract = f.Random.Bool();
                    o.HasParentCompanyGuarantee = f.Random.Bool();
                    o.IsHigherEducationInstitute = f.Random.Bool();
                    o.IsLevyPayerOnly = f.Random.Bool();
                    o.IsNew = f.Random.Bool();
                    o.LearnerSatisfaction = f.Random.Double(0, 100);
                    o.LegalName = f.Company.CompanyName();
                    o.MarketingName = f.Company.CompanyName();
                    o.NationalOverallAchievementRate = f.Random.Double(0, 100);
                    o.NationalProvider = f.Random.Bool();
                    o.OverallAchievementRate = f.Random.Double(0, 100);
                    o.OverallCohort = f.Random.Int(10, 500).ToString();
                    o.Phone = f.Phone.PhoneNumber();
                    o.ProviderFeedback = new SFA.DAS.Apprenticeships.Api.Types.Providers.Feedback
                    {
                        Strengths = new[] { new SFA.DAS.Apprenticeships.Api.Types.Providers.ProviderAttribute { Name = "Communication", Count = 10 } },
                        Weaknesses = new[] { new SFA.DAS.Apprenticeships.Api.Types.Providers.ProviderAttribute { Name = "Timekeeping", Count = 20 } },
                        ExcellentFeedbackCount = f.Random.Int(),
                        GoodFeedbackCount = f.Random.Int(),
                        PoorFeedbackCount = f.Random.Int(),
                        VeryPoorFeedbackCount = f.Random.Int()

                    };
                    o.ProviderMarketingInfo = f.Lorem.Sentences(2);
                    o.ProviderName = f.Company.CompanyName();
                    o.RegulatedStandard = f.Random.Bool();
                    o.StandardCode = f.Random.Int(1, 500);
                    o.TrainingLocations = new List<Core.Models.TrainingLocation>
                    {
                        new Core.Models.TrainingLocation()
                    };
                    o.Ukprn = f.Random.Int(10000000, 999999999);
                    o.Website = f.Internet.Url();
                });

            return fakeResult.Generate();
        }
    }
}