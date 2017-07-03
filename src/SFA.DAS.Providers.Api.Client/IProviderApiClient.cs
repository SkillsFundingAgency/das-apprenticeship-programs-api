using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.Providers;

namespace SFA.DAS.Providers.Api.Client
{
    public interface IProviderApiClient : IDisposable
    {
        /// <summary>
        /// Get a provider details
        /// GET /providers/{provider-ukprn}
        /// </summary>
        /// <param name="providerUkprn">provider registration number (8 numbers long)</param>
        /// <returns>a bool whether the provider exists</returns>
        Provider Get(long providerUkprn);

        /// <summary>
        /// Get a provider details
        /// GET /providers/{provider-ukprn}
        /// </summary>
        /// <param name="providerUkprn">provider registration number (8 numbers long)</param>
        /// <returns>a bool whether the provider exists</returns>
        Task<Provider> GetAsync(long providerUkprn);

        /// <summary>
        /// Get a provider details
        /// GET /providers/{provider-ukprn}
        /// </summary>
        /// <param name="providerUkprn">provider registration number (8 numbers long)</param>
        /// <returns>a bool whether the provider exists</returns>
        Provider Get(int providerUkprn);

        /// <summary>
        /// Get a provider details
        /// GET /providers/{provider-ukprn}
        /// </summary>
        /// <param name="providerUkprn">provider registration number (8 numbers long)</param>
        /// <returns>a bool whether the provider exists</returns>
        Task<Provider> GetAsync(int providerUkprn);

        /// <summary>
        /// Get a provider details
        /// GET /providers/{provider-ukprn}
        /// </summary>
        /// <param name="providerUkprn">provider registration number (8 numbers long)</param>
        /// <returns>a bool whether the provider exists</returns>
        Provider Get(string providerUkprn);

        /// <summary>
        /// Get a provider details
        /// GET /providers/{provider-ukprn}
        /// </summary>
        /// <param name="providerUkprn">provider registration number (8 numbers long)</param>
        /// <returns>a bool whether the provider exists</returns>
        Task<Provider> GetAsync(string providerUkprn);

        /// <summary>
        /// Check if a provider exists
        /// HEAD /frameworks/{provider-ukprn}
        /// </summary>
        /// <param name="providerUkprn">provider registration number (8 numbers long)</param>
        /// <returns>bool</returns>
        bool Exists(long providerUkprn);

        /// <summary>
        /// Check if a provider exists
        /// HEAD /frameworks/{provider-ukprn}
        /// </summary>
        /// <param name="providerUkprn">provider registration number (8 numbers long)</param>
        /// <returns>bool</returns>
        Task<bool> ExistsAsync(long providerUkprn);

        /// <summary>
        /// Check if a provider exists
        /// HEAD /frameworks/{provider-ukprn}
        /// </summary>
        /// <param name="providerUkprn">provider registration number (8 numbers long)</param>
        /// <returns>bool</returns>
        bool Exists(int providerUkprn);

        /// <summary>
        /// Check if a provider exists
        /// HEAD /frameworks/{provider-ukprn}
        /// </summary>
        /// <param name="providerUkprn">provider registration number (8 numbers long)</param>
        /// <returns>bool</returns>
        Task<bool> ExistsAsync(int providerUkprn);

        /// <summary>
        /// Check if a provider exists
        /// HEAD /frameworks/{provider-ukprn}
        /// </summary>
        /// <param name="providerUkprn">provider registration number (8 numbers long)</param>
        /// <returns>bool</returns>
        bool Exists(string providerUkprn);

        /// <summary>
        /// Check if a provider exists
        /// HEAD /frameworks/{provider-ukprn}
        /// </summary>
        /// <param name="providerUkprn">provider registration number (8 numbers long)</param>
        /// <returns>bool</returns>
        Task<bool> ExistsAsync(string providerUkprn);

        /// <summary>
        /// Get all the active providers
        /// GET /providers/
        /// </summary>
        /// <returns>a collection of Providers</returns>
        IEnumerable<ProviderSummary> FindAll();

        /// <summary>
        /// Get all the active providers
        /// GET /providers/
        /// </summary>
        /// <returns>a collection of Providers</returns>
        Task<IEnumerable<ProviderSummary>> FindAllAsync();

        /// <summary>
        /// Get all the active provider locations for a specific framework
        /// GET /providers/frameworks/{frameworkId}
        /// </summary>
        /// <returns>a collection of Framework Providers</returns>
        IEnumerable<FrameworkProvider> GetFrameworkProviders(string frameworkId);

        /// <summary>
        /// Get all the active provider locations for a specific framework
        /// GET /providers/frameworks/{frameworkId}
        /// </summary>
        /// <returns>a collection of Framework Providers</returns>
        Task<IEnumerable<FrameworkProvider>> GetFrameworkProvidersAsync(string frameworkId);

        /// <summary>
        /// Get all the active provider locations for a specific standard
        /// GET /providers/frameworks/{frameworkId}
        /// </summary>
        /// <returns>a collection of Standard Providers</returns>
        IEnumerable<StandardProvider> GetStandardProviders(string standardId);

        /// <summary>
        /// Get all the active provider locations for a specific standard
        /// GET /providers/frameworks/{frameworkId}
        /// </summary>
        /// <returns>a collection of Standard Providers</returns>
        IEnumerable<StandardProvider> GetStandardProviders(int standardId);

        /// <summary>
        /// Get all the active provider locations for a specific standard
        /// GET /providers/frameworks/{frameworkId}
        /// </summary>
        /// <returns>a collection of Standard Providers</returns>
        Task<IEnumerable<StandardProvider>> GetStandardProvidersAsync(string standardId);

        /// <summary>
        /// Get all the active provider locations for a specific standard
        /// GET /providers/frameworks/{frameworkId}
        /// </summary>
        /// <returns>a collection of Standard Providers</returns>
        Task<IEnumerable<StandardProvider>> GetStandardProvidersAsync(int standardId);
    }
}