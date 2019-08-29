using Bogus;
using FluentAssertions;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
using SFA.DAS.Apprenticeships.Api.Types;
using System.Linq;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Infrastructure.Mapping
{
    [TestFixture]
    public class ApprenticeshipSearchResultsMappingTests
    {
        private IApprenticeshipSearchResultsMapping _sut;

        [SetUp]
        public void Init()
        {
            _sut = new ApprenticeshipSearchResultsMapping();
        }

        [Test]
        public void ShouldPopulatePropertiesForStandard()
        {
            ApprenticeshipSearchResultsItem source = CreateTestStandardSourceItem();

            var result = _sut.MapToApprenticeshipSearchResult(source);

            result.Id.Should().Be(source.StandardId.ToString());
            result.ProgrammeType.Should().Be(ApprenticeshipTrainingType.Standard);
            result.Title.Should().Be(source.Title);
            result.Level.Should().Be(source.Level);
            result.Duration.Should().Be(source.Duration);
            result.EffectiveFrom.Should().Be(source.EffectiveFrom);
            result.EffectiveTo.Should().Be(source.EffectiveTo);
            result.LastDateForNewStarts.Should().Be(source.LastDateForNewStarts);
            result.JobRoles.Should().BeEquivalentTo(source.JobRoles);
            result.Keywords.Should().BeEquivalentTo(source.Keywords);
            result.FrameworkName.Should().BeNullOrEmpty();
            result.PathwayName.Should().BeNullOrEmpty();
            result.Published.Should().Be(source.Published);
        }

        [Test]
        public void ShouldPopulatePropertiesForFrameworkWithoutSubgroups()
        {
            ApprenticeshipSearchResultsItem source = CreateTestFrameworkSourceItem();

            var result = _sut.MapToApprenticeshipSearchResult(source);

            result.Id.Should().Be(source.FrameworkId);
            result.ProgrammeType.Should().Be(ApprenticeshipTrainingType.Framework);
            result.Title.Should().Be(source.FrameworkName);
            result.Level.Should().Be(source.Level);
            result.Duration.Should().Be(source.Duration);
            result.EffectiveFrom.Should().Be(source.EffectiveFrom);
            result.EffectiveTo.Should().Be(source.EffectiveTo);
            result.LastDateForNewStarts.Should().Be(source.LastDateForNewStarts);
            result.JobRoles.Should().BeEquivalentTo(source.JobRoleItems.Select(x => x.Title).ToList());
            result.Keywords.Should().BeEquivalentTo(source.Keywords);
            result.FrameworkName.Should().Be(source.FrameworkName);
            result.PathwayName.Should().Be(source.PathwayName);
            result.Published.Should().Be(source.Published);
        }

        [Test]
        public void ShouldPopulatePropertiesForFrameworkWithSubgroups()
        {
            ApprenticeshipSearchResultsItem source = CreateTestFrameworkSourceItem(true);

            var result = _sut.MapToApprenticeshipSearchResult(source);

            result.Id.Should().Be(source.FrameworkId);
            result.ProgrammeType.Should().Be(ApprenticeshipTrainingType.Framework);
            result.Title.Should().Be(source.Title);
            result.Level.Should().Be(source.Level);
            result.Duration.Should().Be(source.Duration);
            result.EffectiveFrom.Should().Be(source.EffectiveFrom);
            result.EffectiveTo.Should().Be(source.EffectiveTo);
            result.LastDateForNewStarts.Should().Be(source.LastDateForNewStarts);
            result.JobRoles.Should().BeEquivalentTo(source.JobRoleItems.Select(x => x.Title).ToList());
            result.Keywords.Should().BeEquivalentTo(source.Keywords);
            result.FrameworkName.Should().Be(source.FrameworkName);
            result.PathwayName.Should().Be(source.PathwayName);
            result.Published.Should().Be(source.Published);
        }

        [Test]
        public void ShouldNotPopulateJobRolesForFrameworkWhenJobRoleItemsIsNull()
        {
            ApprenticeshipSearchResultsItem source = CreateTestFrameworkSourceItem();
            source.JobRoleItems = null;

            var result = _sut.MapToApprenticeshipSearchResult(source);

            result.JobRoles.Should().BeNullOrEmpty();
        }

        private ApprenticeshipSearchResultsItem CreateTestStandardSourceItem()
        {
            var fakeResult = new Faker<ApprenticeshipSearchResultsItem>()
                .StrictMode(false)
                .Rules((f, o) =>
                    {
                        o.StandardId = 123;
                        o.Duration = f.Random.Number(24);
                        o.EffectiveFrom = f.Date.Past();
                        o.EffectiveTo = f.Date.Future();
                        o.JobRoles = f.Lorem.Words(2).ToList();
                        o.Keywords = f.Lorem.Words(2).ToList();
                        o.LastDateForNewStarts = f.Date.Future();
                        o.Level = f.Random.Number(7);
                        o.Published = f.Random.Bool();
                        o.Title = f.Commerce.ProductName();

                        o.FrameworkId = null;
                        o.FrameworkName = null;
                        o.JobRoleItems = null;
                        o.PathwayName = null;
                    });

            return fakeResult.Generate();
        }

        private ApprenticeshipSearchResultsItem CreateTestFrameworkSourceItem(bool hasSubGroups = false)
        {
            var fakeJobRoleItem = new Faker<JobRoleItem>()
                .Rules((f, o) =>
                {
                    o.Title = string.Join(" ", f.Lorem.Words(3));
                    o.Description = f.Lorem.Sentence();
                });

            var fakeResult = new Faker<ApprenticeshipSearchResultsItem>()
                .StrictMode(false)
                .Rules((f, o) =>
                {
                    if (hasSubGroups)
                    {
                        o.FrameworkName = f.Random.Words(2);
                        o.PathwayName = f.Random.Words(2);
                    }
                    else
                    {
                        o.FrameworkName = f.Commerce.ProductName();
                        o.PathwayName = o.FrameworkName;
                    }

                    o.FrameworkId = f.Random.Replace("###-#-#");
                    o.JobRoleItems = fakeJobRoleItem.Generate(2);

                    o.Duration = f.Random.Number(24);
                    o.EffectiveFrom = f.Date.Past();
                    o.EffectiveTo = f.Date.Future();
                    o.Keywords = f.Lorem.Words(2).ToList();
                    o.LastDateForNewStarts = f.Date.Future();
                    o.Level = f.Random.Number(7);
                    o.Published = f.Random.Bool();
                    o.StandardId = null;
                    o.JobRoles = null;
                });

            return fakeResult.Generate();
        }
    }
}