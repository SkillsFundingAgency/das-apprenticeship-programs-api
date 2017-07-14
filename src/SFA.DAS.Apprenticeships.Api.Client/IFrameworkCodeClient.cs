using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.Apprenticeships.Api.Client
{
    public interface IFrameworkCodeClient
    {
        /// <summary>
        /// Check if a Framework Code exists
        /// HEAD /frameworks/codes/{frameworkCode}
        /// </summary>
        /// <param name="frameworkCode">An integer for the framework code (LARS code) ie: 403</param>
        /// <returns>bool</returns>
        bool Exists(int frameworkCode);

        /// <summary>
        /// Check if a Framework Code exists
        /// HEAD /frameworks/codes/{frameworkCode}
        /// </summary>
        /// <param name="frameworkCode">An integer for the framework code (LARS code) ie: 403</param>
        /// <returns>bool</returns>
        Task<bool> ExistsAsync(int frameworkCode);

        /// <summary>
        /// Get a single Framework Code details
        /// GET /frameworks/codes/{frameworkCode}
        /// </summary>
        /// <param name="frameworkCode">An integer for the framework code (LARS code) ie: 403</param>
        /// <returns>a Framework Code Summary</returns>
        FrameworkCodeSummary Get(int frameworkCode);

        /// <summary>
        /// Get a single Framework Code details
        /// GET /frameworks/codes/{frameworkCode}
        /// </summary>
        /// <param name="frameworkCode">An integer for the framework code (LARS code) ie: 403</param>
        /// <returns>a Framework Code Summary</returns>
        Task<FrameworkCodeSummary> GetAsync(int frameworkCode);

        /// <summary>
        /// Get all framework Code details
        /// GET /frameworks/codes
        /// </summary>
        /// <returns>a collection of Framework Code Summary</returns>
        IEnumerable<FrameworkCodeSummary> FindAll();

        /// <summary>
        /// Get all framework Code details
        /// GET /frameworks/codes
        /// </summary>
        /// <returns>a collection of Framework Code Summary</returns>
        Task<IEnumerable<FrameworkCodeSummary>> FindAllAsync();

    }
}