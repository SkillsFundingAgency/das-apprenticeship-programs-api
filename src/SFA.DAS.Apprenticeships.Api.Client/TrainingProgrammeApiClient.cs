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

        // These need to public to support the unit tests
        public static readonly string AllProgrammesCacheKey = $"{nameof(TrainingProgrammeApiClient)}.allProgrammes";
        public static readonly string StandardProgrammesCacheKey = $"{nameof(TrainingProgrammeApiClient)}.standardProgrammes";
        public static readonly string FrameworkProgrammesCacheKey = $"{nameof(TrainingProgrammeApiClient)}.frameworkProgrammes";

        public Task<IReadOnlyList<ITrainingProgramme>> GetTrainingProgrammes()
        {
            return GetAllTrainingProgrammes();
        }

        public Task<IReadOnlyList<ITrainingProgramme>> GetAllTrainingProgrammes()
        {
            return _memoryCache.GetOrCreateAsync(AllProgrammesCacheKey, LoadAllProgrammes);
        }

        public Task<IReadOnlyList<ITrainingProgramme>> GetFrameworkTrainingProgrammes()
        {
            return _memoryCache.GetOrCreateAsync(FrameworkProgrammesCacheKey, LoadFrameworkProgrammes);
        }

        public Task<IReadOnlyList<ITrainingProgramme>> GetStandardTrainingProgrammes()
        {
            return _memoryCache.GetOrCreateAsync(StandardProgrammesCacheKey, LoadStandardProgrammes);
        }

        public async Task<ITrainingProgramme> GetTrainingProgramme(string id)
        {
            var programmes = await GetTrainingProgrammes();

            return programmes.FirstOrDefault(p => p.Id == id);
        }

        private async Task<IReadOnlyList<ITrainingProgramme>> LoadAllProgrammes(ICacheEntry cacheEntry)
        {
            var frameworkTask = GetFrameworkTrainingProgrammes();
            var standardsTask = GetStandardTrainingProgrammes();
 
            await Task.WhenAll(frameworkTask, standardsTask);

            return frameworkTask.Result.Union(standardsTask.Result).OrderBy(tp => tp.Title).ToList();
        }

        private Task<IReadOnlyList<ITrainingProgramme>> LoadStandardProgrammes(ICacheEntry cacheEntry)
        {
            return _standardApiClient.GetAllAsync()
                .ContinueWith(t => t.Result.OrderBy(tp => tp.Title).ToList() as IReadOnlyList<ITrainingProgramme>);
        }

        private Task<IReadOnlyList<ITrainingProgramme>> LoadFrameworkProgrammes(ICacheEntry cacheEntry)
        {
            return _frameworkApiClient.GetAllAsync()
                .ContinueWith(t => t.Result.OrderBy(tp => tp.Title).ToList() as IReadOnlyList<ITrainingProgramme>);
        }
    }
}