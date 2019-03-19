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
            return _memoryCache.GetOrCreateAsync(ProgrammesCacheKey, LoadProgrammes);
        }

        public async Task<ITrainingProgramme> GetTrainingProgramme(string id)
        {
            var programmes = await GetTrainingProgrammes();

            return programmes.FirstOrDefault(p => p.Id == id);
        }

        private async Task<IReadOnlyList<ITrainingProgramme>> LoadProgrammes(ICacheEntry cachEntry)
        {
            var tasks = new List<Task<IEnumerable<ITrainingProgramme>>>
            {
                _frameworkApiClient.GetAllAsync().ContinueWith(t => t.Result as IEnumerable<ITrainingProgramme>),
                _standardApiClient.GetAllAsync().ContinueWith(t => t.Result as IEnumerable<ITrainingProgramme>)
            };

            await Task.WhenAll(tasks);

            var combined = tasks.SelectMany(t => t.Result).OrderBy(tp => tp.Title).ToList();

            return combined;
        }
    }
}