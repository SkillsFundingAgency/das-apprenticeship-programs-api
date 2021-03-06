﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.Apprenticeships.Api.Client.UnitTests
{
    [TestFixture]
    public class TrainingProgrammeApiClientTests
    {
        [Test]
        public void Constructor_ValidCall_ShouldThrowException()
        {
            var fixtures = new TrainingProgrammeApiClientTestFixtures();
            fixtures.CreateClient();
        }

        [Test]
        public async Task GetTrainingProgramme_WithIdDefinedForStandard_ShouldReturnStandard()
        {
            // Arrange
            const string id = "123";
            var fixtures = new TrainingProgrammeApiClientTestFixtures().WithStandard(id);
            var client = fixtures.CreateClient();

            // Act
            var trainingProgramme = await client.GetTrainingProgramme(id);

            // Assert
            Assert.IsNotNull(trainingProgramme);
            Assert.AreEqual(id, trainingProgramme.Id);
            Assert.AreEqual(ProgrammeType.Standard, trainingProgramme.ProgrammeType);
        }

        [Test]
        public async Task GetTrainingProgramme_WithIdDefinedForFramework_ShouldReturnFramework()
        {
            // Arrange
            const string id = "123";
            var fixtures = new TrainingProgrammeApiClientTestFixtures().WithFramework(id);
            var client = fixtures.CreateClient();

            // Act
            var trainingProgramme = await client.GetTrainingProgramme(id);

            // Assert
            Assert.IsNotNull(trainingProgramme);
            Assert.AreEqual(id, trainingProgramme.Id);
            Assert.AreEqual(ProgrammeType.Framework, trainingProgramme.ProgrammeType);
        }

        [Test]
        public async Task GetTrainingProgrammes_WithNoProgrammes_ShouldReturnEmptyCollection()
        {
            // Arrange
            var fixtures = new TrainingProgrammeApiClientTestFixtures();
            var client = fixtures.CreateClient();

            // Act
            var trainingProgrammes = await client.GetTrainingProgrammes();

            // Assert
            Assert.IsNotNull(trainingProgrammes);
            const int expectedCount = 0;
            Assert.AreEqual(expectedCount, trainingProgrammes.Count);
        }

        [Test]
        public async Task GetTrainingProgrammes_WithOneStandardAndOneFramework_ShouldReturnBoth()
        {
            // Arrange
            var fixtures = new TrainingProgrammeApiClientTestFixtures()
                .WithFramework("123")
                .WithStandard("456");

            var client = fixtures.CreateClient();

            // Act
            var trainingProgrammes = await client.GetTrainingProgrammes();

            // Assert
            const int expectedCount = 2;
            Assert.AreEqual(expectedCount, trainingProgrammes.Count);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public Task GetAllTrainingProgrammes_WithOnlySpecifiedTypesAvailable_ShouldReturnWhateverHasBeenAdded(bool addFramework, bool addStandard)
        {
            // For all we expect all programmes that have been added to be returned
            return CheckCallReturnsExpectedTypes(client => client.GetAllTrainingProgrammes(), addFramework, addStandard, addFramework, addStandard);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public Task GetStandardTrainingProgrammes_WithOnlySpecifiedTypesAvailable_ShouldOnlyEverReturnStandard(bool addFramework, bool addStandard)
        {
            return CheckCallReturnsExpectedTypes(client => client.GetStandardTrainingProgrammes(), addFramework, addStandard, false, addStandard);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public Task GetFrameworkTrainingProgrammes_WithOnlySpecifiedTypesAvailable_ShouldOnlyEverReturnFramework(bool addFramework, bool addStandard)
        {
            return CheckCallReturnsExpectedTypes(client => client.GetFrameworkTrainingProgrammes(), addFramework, addStandard, addFramework, false);
        }

        private async Task CheckCallReturnsExpectedTypes(Func<TrainingProgrammeApiClient, Task<IReadOnlyList<ITrainingProgramme>>> getter, bool addFramework, bool addStandard, bool expectFramework, bool expectStandard)
        {
            // Arrange
            var fixtures = new TrainingProgrammeApiClientTestFixtures();
            
            if(addFramework) fixtures.WithFramework("123");
            if(addStandard) fixtures.WithStandard("456");

            var client = fixtures.CreateClient();

            // Act
            var trainingProgrammes = await getter(client);

            var actualFramework = trainingProgrammes.Any(tp => tp.ProgrammeType == ProgrammeType.Framework);
            var actualStandard = trainingProgrammes.Any(tp => tp.ProgrammeType == ProgrammeType.Standard);

            // Assert
            Assert.AreEqual(expectFramework, actualFramework, $"Framework result not correct: expected={expectFramework} actual:{actualFramework}");
            Assert.AreEqual(expectStandard, actualStandard, $"Standard result not correct: expected={expectStandard} actual:{actualStandard}");
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task GetTrainingProgrammes_WithMultipleStandardAndOneFrameworks_ShouldReturnAll(bool populateCache)
        {
            // Arrange
            var fixtures = new TrainingProgrammeApiClientTestFixtures()
                .WithFramework("123")
                .WithFramework("124")
                .WithFramework("125")
                .WithStandard("456")
                .WithStandard("457")
                .WithStandard("458");

            if (populateCache)
            {
                fixtures.WithPopulatedCache();
            }

            var client = fixtures.CreateClient();

            // Act
            var trainingProgrammes = await client.GetTrainingProgrammes();

            // Assert
            const int expectedCount = 6;
            Assert.AreEqual(expectedCount, trainingProgrammes.Count);
        }
    }

    internal class TrainingProgrammeApiClientTestFixtures
    {
        private readonly List<Standard> _standards;
        private readonly List<Framework> _frameworks;

        public TrainingProgrammeApiClientTestFixtures()
        {
            _standards = new List<Standard>();
            _frameworks = new List<Framework>();

            StandardApiClientMock = new Mock<IStandardApiClient>();
            FrameworkApiClientMock = new Mock<IFrameworkApiClient>();
            MemoryCacheMock = new Mock<IMemoryCache>();
            CacheEntryMock = new Mock<ICacheEntry>();

            StandardApiClientMock
                .Setup(client => client.GetAsync(It.IsAny<string>()))
                .Returns<string>(GetStandardAsync);

            FrameworkApiClientMock
                .Setup(client => client.GetAsync(It.IsAny<string>()))
                .Returns<string>(GetFrameworkAsync);

            StandardApiClientMock
                .Setup(client => client.GetAllAsync())
                .ReturnsAsync(() => _standards.Select(s => new StandardSummary {Id = s.Id, Title = s.Title}));

            FrameworkApiClientMock
                .Setup(client => client.GetAllAsync())
                .ReturnsAsync(() => _frameworks.Select(f => new FrameworkSummary {Id = f.Id, Title = f.Title}));

            MemoryCacheMock
                .Setup(mc => mc.CreateEntry(It.IsAny<string>()))
                .Returns<string>(key => CacheEntry);
        }

        public Mock<IStandardApiClient> StandardApiClientMock { get; }
        public IStandardApiClient StandardApiClient => StandardApiClientMock.Object;

        public Mock<IFrameworkApiClient> FrameworkApiClientMock { get; }
        public IFrameworkApiClient FrameworkApiClient => FrameworkApiClientMock.Object;

        public Mock<IMemoryCache> MemoryCacheMock { get; }
        public IMemoryCache MemoryCache => MemoryCacheMock.Object;

        public Mock<ICacheEntry> CacheEntryMock { get; }
        public ICacheEntry CacheEntry => CacheEntryMock.Object;

        public TrainingProgrammeApiClient CreateClient()
        {
            TrainingProgrammeApiClient.ProgrammeLists programmes = null;

            if (PopulateCache)
            {
                programmes = new TrainingProgrammeApiClient.ProgrammeLists(_frameworks, _standards);
            }

            object tempProgrammes = programmes;

            MemoryCacheMock
                .Setup(mc => mc.TryGetValue(It.IsAny<object>(), out tempProgrammes))
                .Returns(PopulateCache);

            return new TrainingProgrammeApiClient(MemoryCache, FrameworkApiClient, StandardApiClient);
        }

        public TrainingProgrammeApiClientTestFixtures WithStandard(string id)
        {
            _standards.Add(new Standard{StandardId =  id, Title = $"Title {id}"});
            return this;
        }

        public TrainingProgrammeApiClientTestFixtures WithFramework(string id)
        {
            _frameworks.Add(new Framework {FrameworkId = id, Title = $"Title {id}", FrameworkName = $"Framework{id}" , PathwayName = $"Pathway{id}"});
            return this;
        }

        public TrainingProgrammeApiClientTestFixtures WithPopulatedCache()
        {
            PopulateCache = true;
            return this;
        }

        public bool PopulateCache { get; private set; }

        private Task<Standard> GetStandardAsync(string id)
        {
            return Task.FromResult(_standards.FirstOrDefault(s => s.StandardId == id));
        }

        private Task<Framework> GetFrameworkAsync(string id)
        {
            return Task.FromResult(_frameworks.FirstOrDefault(f => f.FrameworkId == id));
        }
    }
}
