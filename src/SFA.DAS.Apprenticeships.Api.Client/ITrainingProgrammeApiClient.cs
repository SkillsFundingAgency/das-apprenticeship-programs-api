using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.Apprenticeships.Api.Client
{
    /// <summary>
    ///     Provides an abstraction on top of <see cref="IStandardApiClient"/> and <see cref="IFrameworkApiClient"/>
    ///     and allows querying across both as a single data source.
    /// </summary>
    public interface ITrainingProgrammeApiClient
    {
        Task<ITrainingProgramme> GetTrainingProgramme(string id);
        Task<IReadOnlyList<ITrainingProgramme>> GetTrainingProgrammes();
    }
}