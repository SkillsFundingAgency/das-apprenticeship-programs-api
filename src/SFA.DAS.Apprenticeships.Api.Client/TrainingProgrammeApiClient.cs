using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.Apprenticeships.Api.Client
{
    public class TrainingProgrammeApiClient : ITrainingProgrammeApiClient
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IFrameworkApiClient _frameworkApiClient;
        private readonly IStandardApiClient _standardApiClient;

        public TrainingProgrammeApiClient(
            IMemoryCache memoryCache,
            IFrameworkApiClient frameworkApiClient,
            IStandardApiClient standardApiClient)
        {
            _frameworkApiClient = frameworkApiClient;
            _standardApiClient = standardApiClient;
            _memoryCache = memoryCache;
        }

        private readonly string ProgrammesCacheKey = $"{nameof(TrainingProgrammeApiClient)}.programmes";

        public Task<IReadOnlyList<ITrainingProgramme>> GetTrainingProgrammes()
        {
            return GetTrainingProgrammes(RequiredProgrammeTypes.All);
        }

        public async Task<IReadOnlyList<ITrainingProgramme>> GetTrainingProgrammes(RequiredProgrammeTypes requiredProgrammeTypes)
        {
            var programmeLists = await _memoryCache.GetOrCreateAsync(ProgrammesCacheKey, LoadProgrammes);

            switch (requiredProgrammeTypes)
            {
                case RequiredProgrammeTypes.Framework: return programmeLists.Frameworks;
                case RequiredProgrammeTypes.Standard: return programmeLists.Standards;
                case RequiredProgrammeTypes.All: return programmeLists.AllProgrammes;
                default:
                    throw new InvalidOperationException($"{nameof(GetTrainingProgrammes)} does not support {requiredProgrammeTypes}");
            }
        }

        public async Task<ITrainingProgramme> GetTrainingProgramme(string id)
        {
            var programmes = await GetTrainingProgrammes();

            return programmes.FirstOrDefault(p => p.Id == id);
        }

        private async Task<ProgrammeLists> LoadProgrammes(ICacheEntry cacheEntry)
        {
            var frameworkTask = _frameworkApiClient.GetAllAsync()
                .ContinueWith(t => t.Result as IEnumerable<ITrainingProgramme>);

            var standardsTask = _standardApiClient.GetAllAsync()
                .ContinueWith(t => t.Result as IEnumerable<ITrainingProgramme>);
 
            await Task.WhenAll(frameworkTask, standardsTask);

            return new ProgrammeLists(frameworkTask.Result, standardsTask.Result);
        }

        public class ProgrammeLists
        {
            public ProgrammeLists(IEnumerable<ITrainingProgramme> frameworks, IEnumerable<ITrainingProgramme> standards)
            {
                Frameworks = ToListByTitle(frameworks);
                Standards = ToListByTitle(standards);

                AllProgrammes = ToListByTitle(Frameworks.Union(Standards));
            }

            public IReadOnlyList<ITrainingProgramme> AllProgrammes { get; }
            public IReadOnlyList<ITrainingProgramme> Frameworks { get; }
            public IReadOnlyList<ITrainingProgramme> Standards { get; }

            private List<ITrainingProgramme> ToListByTitle(IEnumerable<ITrainingProgramme> trainingProgrammes)
            {
                return trainingProgrammes.OrderBy(fw => fw.Title).ToList();
            }
        }
    }
}