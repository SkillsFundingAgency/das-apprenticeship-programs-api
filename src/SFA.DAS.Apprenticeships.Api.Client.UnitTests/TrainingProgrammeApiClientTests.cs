using System;
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
        private const string FrameworkCode = "123";
        private const string StandardCode = "456";

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
            const string id = FrameworkCode;
            var fixtures = new TrainingProgrammeApiClientTestFixtures()
                .WithStandard(id);
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
            const string id = FrameworkCode;
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
                .WithFramework(FrameworkCode)
                .WithStandard(StandardCode);

            var client = fixtures.CreateClient();

            // Act
            var trainingProgrammes = await client.GetTrainingProgrammes();

            // Assert
            const int expectedCount = 2;
            Assert.AreEqual(expectedCount, trainingProgrammes.Count);
        }

        [Test]
        public async Task GetAllTrainingProgrammes_WithOnlyStandard_ShouldReturnStandard()
        {
            // Arrange
            var fixtures = new TrainingProgrammeApiClientTestFixtures()
                .WithStandard(StandardCode);

            var client = fixtures.CreateClient();

            // Act
            var trainingProgrammes = await client.GetAllTrainingProgrammes();

            // Assert
            Assert.AreEqual(1, trainingProgrammes.Count, "Incorrect number of training programmes returned");
            Assert.AreEqual(ProgrammeType.Standard, trainingProgrammes[0].ProgrammeType);
            Assert.AreEqual(StandardCode, trainingProgrammes[0].Id);
        }

        [Test]
        public async Task GetAllTrainingProgrammes_WithOnlyFramework_ShouldReturnFramework()
        {
            // Arrange
            var fixtures = new TrainingProgrammeApiClientTestFixtures()
                .WithFramework(FrameworkCode);

            var client = fixtures.CreateClient();

            // Act
            var trainingProgrammes = await client.GetAllTrainingProgrammes();

            // Assert
            Assert.AreEqual(1, trainingProgrammes.Count, "Incorrect number of training programmes returned");
            Assert.AreEqual(ProgrammeType.Framework, trainingProgrammes[0].ProgrammeType);
            Assert.AreEqual(FrameworkCode, trainingProgrammes[0].Id);
        }

        [Test]
        public async Task GetAllTrainingProgrammes_WithStandardAndFramework_ShouldReturnStandardAndFramework()
        {
            // Arrange
            var fixtures = new TrainingProgrammeApiClientTestFixtures()
                .WithFramework(FrameworkCode)
                .WithStandard(StandardCode);

            var client = fixtures.CreateClient();

            // Act
            var trainingProgrammes = await client.GetAllTrainingProgrammes();

            // Assert
            Assert.AreEqual(2, trainingProgrammes.Count, "Incorrect number of training programmes returned");
            Assert.IsTrue(trainingProgrammes.Any(tp => tp.Id== FrameworkCode && tp.ProgrammeType==ProgrammeType.Framework));
            Assert.IsTrue(trainingProgrammes.Any(tp => tp.Id == StandardCode && tp.ProgrammeType == ProgrammeType.Standard));
        }

        [Test]
        public async Task GetStandardTrainingProgrammes_WithOnlyStandard_ShouldReturnStandard()
        {
            // Arrange
            var fixtures = new TrainingProgrammeApiClientTestFixtures()
                .WithStandard(StandardCode);

            var client = fixtures.CreateClient();

            // Act
            var trainingProgrammes = await client.GetStandardTrainingProgrammes();

            // Assert
            Assert.AreEqual(1, trainingProgrammes.Count, "Incorrect number of training programmes returned");
            Assert.AreEqual(ProgrammeType.Standard, trainingProgrammes[0].ProgrammeType);
            Assert.AreEqual(StandardCode, trainingProgrammes[0].Id);
        }

        [Test]
        public async Task GetStandardTrainingProgrammes_WithOnlyFramework_ShouldReturnEmptyList()
        {
            // Arrange
            var fixtures = new TrainingProgrammeApiClientTestFixtures()
                .WithFramework(FrameworkCode);

            var client = fixtures.CreateClient();

            // Act
            var trainingProgrammes = await client.GetStandardTrainingProgrammes();

            // Assert
            Assert.AreEqual(0, trainingProgrammes.Count, "Incorrect number of training programmes returned");
        }

        [Test]
        public async Task GetStandardTrainingProgrammes_WithFrameworkAndStandard_ShouldReturnJustStandard()
        {
            // Arrange
            var fixtures = new TrainingProgrammeApiClientTestFixtures()
                .WithFramework(FrameworkCode)
                .WithStandard(StandardCode);

            var client = fixtures.CreateClient();

            // Act
            var trainingProgrammes = await client.GetStandardTrainingProgrammes();

            // Assert
            Assert.AreEqual(1, trainingProgrammes.Count, "Incorrect number of training programmes returned");
            Assert.AreEqual(ProgrammeType.Standard, trainingProgrammes[0].ProgrammeType);
            Assert.AreEqual(StandardCode, trainingProgrammes[0].Id);
        }

        [Test]
        public async Task GetFrameworkTrainingProgrammes_WithOnlyStandard_ShouldReturnEmptyList()
        {
            // Arrange
            var fixtures = new TrainingProgrammeApiClientTestFixtures()
                .WithStandard(StandardCode);

            var client = fixtures.CreateClient();

            // Act
            var trainingProgrammes = await client.GetFrameworkTrainingProgrammes();

            // Assert
            Assert.AreEqual(0, trainingProgrammes.Count, "Incorrect number of training programmes returned");
        }

        [Test]
        public async Task GetFrameworkTrainingProgrammes_WithOnlyFramework_ShouldReturnFramework()
        {
            // Arrange
            var fixtures = new TrainingProgrammeApiClientTestFixtures()
                .WithFramework(FrameworkCode);

            var client = fixtures.CreateClient();

            // Act
            var trainingProgrammes = await client.GetFrameworkTrainingProgrammes();

            // Assert
            // Assert
            Assert.AreEqual(1, trainingProgrammes.Count, "Incorrect number of training programmes returned");
            Assert.AreEqual(ProgrammeType.Framework, trainingProgrammes[0].ProgrammeType);
            Assert.AreEqual(FrameworkCode, trainingProgrammes[0].Id);
        }

        [Test]
        public async Task GetFrameworkTrainingProgrammes_WithFrameworkAndStandard_ShouldReturnJustFramework()
        {
            // Arrange
            var fixtures = new TrainingProgrammeApiClientTestFixtures()
                .WithFramework(FrameworkCode)
                .WithStandard(StandardCode);

            var client = fixtures.CreateClient();

            // Act
            var trainingProgrammes = await client.GetFrameworkTrainingProgrammes();

            // Assert
            Assert.AreEqual(1, trainingProgrammes.Count, "Incorrect number of training programmes returned");
            Assert.AreEqual(ProgrammeType.Framework, trainingProgrammes[0].ProgrammeType);
            Assert.AreEqual(FrameworkCode, trainingProgrammes[0].Id);
        }
    }

    internal class TrainingProgrammeApiClientTestFixtures
    {
        private readonly List<Standard> _standards;
        private readonly List<Framework> _frameworks;

        IReadOnlyList<ITrainingProgramme> _allCachedItem;
        IReadOnlyList<ITrainingProgramme> _standardCachedItem;
        IReadOnlyList<ITrainingProgramme> _frameworkCachedItem;

        public TrainingProgrammeApiClientTestFixtures()
        {
            _standards = new List<Standard>();
            _frameworks = new List<Framework>();

            StandardApiClientMock = new Mock<IStandardApiClient>();
            FrameworkApiClientMock = new Mock<IFrameworkApiClient>();
            MemoryCacheMock = new Mock<IMemoryCache>();

            AllCacheEntryMock = new Mock<ICacheEntry>();
            StandardCacheEntryMock = new Mock<ICacheEntry>();
            FrameworkCacheEntryMock = new Mock<ICacheEntry>();

            //StandardApiClientMock
            //    .Setup(client => client.GetAsync(It.IsAny<string>()))
            //    .Returns<string>(GetStandardAsync);

            //FrameworkApiClientMock
            //    .Setup(client => client.GetAsync(It.IsAny<string>()))
            //    .Returns<string>(GetFrameworkAsync);
        }

        public Mock<IStandardApiClient> StandardApiClientMock { get; }
        public IStandardApiClient StandardApiClient => StandardApiClientMock.Object;

        public Mock<IFrameworkApiClient> FrameworkApiClientMock { get; }
        public IFrameworkApiClient FrameworkApiClient => FrameworkApiClientMock.Object;

        public Mock<IMemoryCache> MemoryCacheMock { get; }
        public IMemoryCache MemoryCache => MemoryCacheMock.Object;

        public Mock<ICacheEntry> AllCacheEntryMock { get; }
        public ICacheEntry AllCacheEntry => AllCacheEntryMock.Object;

        public Mock<ICacheEntry> StandardCacheEntryMock { get; }
        public ICacheEntry StandardCacheEntry => StandardCacheEntryMock.Object;

        public Mock<ICacheEntry> FrameworkCacheEntryMock { get; }
        public ICacheEntry FrameworkCacheEntry => FrameworkCacheEntryMock.Object;

        public TrainingProgrammeApiClient CreateClient()
        {
            SetStandard();
            SetFramework();
            SetAll();

            return new TrainingProgrammeApiClient(MemoryCache, FrameworkApiClient, StandardApiClient);
        }

        private void SetStandard()
        {
            // Setup API call
            StandardApiClientMock
                .Setup(client => client.GetAllAsync())
                .ReturnsAsync(() => _standards.Select(s => new StandardSummary { Id = s.Id, Title = s.Title }));

            // Setup call to check cache
            object obj;

            MemoryCacheMock
                .Setup(mc => mc.TryGetValue(TrainingProgrammeApiClient.StandardProgrammesCacheKey, out obj))
                .Returns(_standardCachedItem != null);

            // Setup call to create cache item
            MemoryCacheMock
                .Setup(mc => mc.CreateEntry(TrainingProgrammeApiClient.StandardProgrammesCacheKey))
                .Returns<string>(key => StandardCacheEntry);
        }

        private void SetFramework()
        {
            // Setup API call
            FrameworkApiClientMock
                .Setup(client => client.GetAllAsync())
                .ReturnsAsync(() => _frameworks.Select(s => new FrameworkSummary { Id = s.Id, Title = s.Title }));

            // Setup call to check cache
            object obj;

            MemoryCacheMock
                .Setup(mc => mc.TryGetValue(TrainingProgrammeApiClient.FrameworkProgrammesCacheKey, out obj))
                .Returns(_frameworkCachedItem != null);

            // Setup call to create cache item
            MemoryCacheMock
                .Setup(mc => mc.CreateEntry(TrainingProgrammeApiClient.FrameworkProgrammesCacheKey))
                .Returns<string>(key => FrameworkCacheEntry);
        }

        private void SetAll()
        {
            // Setup call to check cache
            object obj;

            MemoryCacheMock
                .Setup(mc => mc.TryGetValue(TrainingProgrammeApiClient.AllProgrammesCacheKey, out obj))
                .Returns(_allCachedItem != null);

            // Setup call to create cache item
            MemoryCacheMock
                .Setup(mc => mc.CreateEntry(TrainingProgrammeApiClient.AllProgrammesCacheKey))
                .Returns<string>(key => AllCacheEntry);
        }

        /// <summary>
        ///     Adds a response to the simulated call to the apprentice info call
        /// </summary>
        public TrainingProgrammeApiClientTestFixtures WithStandard(string id)
        {
            _standards.Add(new Standard{StandardId =  id, Title = $"Title {id}"});
            return this;
        }

        /// <summary>
        ///     Adds a response to the simulated call to the apprentice info call
        /// </summary>
        public TrainingProgrammeApiClientTestFixtures WithFramework(string id)
        {
            _frameworks.Add(new Framework {FrameworkId = id, Title = $"Title {id}"});
            return this;
        }

        //private Task<Standard> GetStandardAsync(string id)
        //{
        //    return Task.FromResult(_standards.FirstOrDefault(s => s.StandardId == id));
        //}

        //private Task<Framework> GetFrameworkAsync(string id)
        //{
        //    return Task.FromResult(_frameworks.FirstOrDefault(f => f.FrameworkId == id));
        //}
    }
}
