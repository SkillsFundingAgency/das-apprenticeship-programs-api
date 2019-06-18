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
        /// <summary>
        ///     Returns a training programme with the specified id. If not found then returns null.
        /// </summary>
        Task<ITrainingProgramme> GetTrainingProgramme(string id);

        /// <summary>
        ///     Returns all training programmes ordered by title.
        ///     This is equivalent to calling <see cref="GetTrainingProgrammes()"/> with
        ///     <see cref="RequiredProgrammeTypes"/> set to <see cref="RequiredProgrammeTypes.All"/>.
        /// </summary>
        Task<IReadOnlyList<ITrainingProgramme>> GetTrainingProgrammes();

        /// <summary>
        ///     Returns training programmes of the specified types ordered by title.
        /// </summary>
        Task<IReadOnlyList<ITrainingProgramme>> GetTrainingProgrammes(RequiredProgrammeTypes programmeType);
    }
}