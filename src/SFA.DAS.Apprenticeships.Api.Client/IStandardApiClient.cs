using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.Apprenticeships.Api.Client
{
    public interface IStandardApiClient : IDisposable
    {
        /// <summary>
        /// Get a single standard details
        /// GET /standards/{standard-code}
        /// </summary>
        /// <param name="standardCode">An integer for the standard id (LARS code) ie: 12</param>
        /// <returns>a standard</returns>
        Standard Get(int standardCode);

        /// <summary>
        /// Get a single standard details
        /// GET /standards/{standard-code}
        /// </summary>
        /// <param name="standardCode">An integer for the standard id (LARS code) ie: 12</param>
        /// <returns>a standard</returns>
        Task<Standard> GetAsync(int standardCode);

        /// <summary>
        /// Get a single standard details
        /// GET /standards/{standard-code}
        /// </summary>
        /// <param name="standardCode">An integer for the standard id (LARS code) ie: 12</param>
        /// <returns>a standard</returns>
        Standard Get(string standardCode);

        /// <summary>
        /// Get a single standard details
        /// GET /standards/{standard-code}
        /// </summary>
        /// <param name="standardCode">An integer for the standard id (LARS code) ie: 12</param>
        /// <returns>a standard</returns>
        Task<Standard> GetAsync(string standardCode);

        /// <summary>
        /// Find all the active standards
        /// </summary>
        /// <returns>Collection of standard summaries</returns>
        IEnumerable<StandardSummary> FindAll();

        /// <summary>
        /// Find all the active standards
        /// </summary>
        /// <returns>Collection of standard summaries</returns>
        Task<IEnumerable<StandardSummary>> FindAllAsync();

        /// <summary>
        /// Find all the active standards
        /// </summary>
        /// <returns>Collection of standard summaries</returns>
        IEnumerable<StandardSummary> GetAll();

        /// <summary>
        /// Find all the active standards
        /// </summary>
        /// <returns>Collection of standard summaries</returns>
        Task<IEnumerable<StandardSummary>> GetAllAsync();

        /// <summary>
        /// Check if a standard exists
        /// HEAD /standards/{standard-code}
        /// </summary>
        /// <param name="standardCode">An integer for the standard id (LARS code) ie: 12</param>
        /// <returns>bool</returns>
        bool Exists(int standardCode);

        /// <summary>
        /// Check if a standard exists
        /// HEAD /standards/{standard-code}
        /// </summary>
        /// <param name="standardCode">An integer for the standard id (LARS code) ie: 12</param>
        /// <returns>bool</returns>
        Task<bool> ExistsAsync(int standardCode);

        /// <summary>
        /// Check if a standard exists
        /// HEAD /standards/{standard-code}
        /// </summary>
        /// <param name="standardCode">An integer for the standard id (LARS code) ie: 12</param>
        /// <returns>bool</returns>
        bool Exists(string standardCode);

        /// <summary>
        /// Check if a standard exists
        /// HEAD /standards/{standard-code}
        /// </summary>
        /// <param name="standardCode">An integer for the standard id (LARS code) ie: 12</param>
        /// <returns>bool</returns>
        Task<bool> ExistsAsync(string standardCode);
    }
}