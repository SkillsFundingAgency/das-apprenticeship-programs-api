using Bogus;
using FluentAssertions;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models;
using SFA.DAS.Apprenticeships.Api.Types;
using System.Collections.Generic;
using System.Linq;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Infrastructure.Mapping
{
    [TestFixture]
    public class ApprenticeshipSearchResultDocumentMappingTests
    {
        private IApprenticeshipSearchResultDocumentMapping _sut;

        [SetUp]
        public void Init()
        {
            _sut = new ApprenticeshipSearchResultDocumentMapping();
        }

        [Test]
        public void ShouldPopulatePropertiesForStandard()
        {
            ApprenticeshipSearchResultsDocument source = CreateTestDocument();

            var result = _sut.MapToApprenticeshipSearchResultsItem(source);

            result.Duration.Should().Be(source.Duration);
            result.EffectiveFrom.Should().Be(source.EffectiveFrom);
            result.EffectiveTo.Should().Be(source.EffectiveTo);
            result.FrameworkId.Should().Be(source.FrameworkId);
            result.FrameworkName.Should().Be(source.FrameworkName);
            result.JobRoleItems.Should().BeSameAs(source.JobRoleItems);
            result.JobRoles.Should().BeSameAs(source.JobRoles);
            result.Keywords.Should().BeSameAs(source.Keywords);
            result.LastDateForNewStarts.Should().Be(source.LastDateForNewStarts);
            result.Level.Should().Be(source.Level);
            result.PathwayName.Should().Be(source.PathwayName);
            result.Published.Should().Be(source.Published);
            result.StandardId.Should().Be(source.StandardId?.ToString());
            result.Title.Should().Be(source.Title);
            result.TitleKeyword.Should().Be(source.TitleKeyword);
        }

        private ApprenticeshipSearchResultsDocument CreateTestDocument()
        {
            var fakeResult = new Faker<ApprenticeshipSearchResultsDocument>()
                .StrictMode(false)
                .Rules((f, o) =>
                    {
                        o.Duration = f.Random.Number(24);
                        o.EffectiveFrom = f.Date.Past();
                        o.EffectiveTo = f.Date.Future();
                        o.FrameworkId = "420-2-1";
                        o.FrameworkName = f.Commerce.ProductName();
                        o.JobRoleItems = new List<JobRoleItem>();
                        o.JobRoles = f.Lorem.Words(2).ToList();
                        o.Keywords = f.Lorem.Words(2).ToList();
                        o.LastDateForNewStarts = f.Date.Future();
                        o.Level = f.Random.Number(7);
                        o.PathwayName = f.Commerce.ProductName();
                        o.Published = f.Random.Bool();
                        o.StandardId = 1234;
                        o.Title = f.Commerce.ProductName();
                        o.TitleKeyword = f.Commerce.ProductName();
                    });

            return fakeResult.Generate();
        }
    }
}