﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types.Providers;

namespace SFA.DAS.Providers.Api.Client
{
    public interface IProviderApiClient : IDisposable
    {
        /// <summary>
        /// Makes an inexpensive ping request
        /// </summary>
        /// <returns>Task</returns>
        Task Ping();

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
	    /// Search for providers
	    /// </summary>
	    /// <returns>a search result object</returns>
	    ProviderSearchResponseItem Search(string keywords, int page = 1);

	    /// <summary>
	    /// Search for providers
	    /// </summary>
	    /// <returns>a search result object</returns>
		Task<ProviderSearchResponseItem> SearchAsync(string keywords, int page = 1);

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

        ///// <summary>
        ///// Get all the active provider locations for a specific framework
        ///// GET /providers/frameworks/{frameworkId}
        ///// </summary>
        ///// <returns>a collection of Framework Providers</returns>
        //IEnumerable<FrameworkProvider> GetFrameworkProviders(string frameworkId);

        ///// <summary>
        ///// Get all the active provider locations for a specific framework
        ///// GET /providers/frameworks/{frameworkId}
        ///// </summary>
        ///// <returns>a collection of Framework Providers</returns>
        //Task<IEnumerable<FrameworkProvider>> GetFrameworkProvidersAsync(string frameworkId);

        ///// <summary>
        ///// Get all the active provider locations for a specific standard
        ///// GET /providers/standard/{standard code}
        ///// </summary>
        ///// <returns>a collection of Standard Providers</returns>
        //IEnumerable<StandardProvider> GetStandardProviders(string standardId);

        ///// <summary>
        ///// Get all the active provider locations for a specific standard
        ///// GET /providers/standard/{standard code}
        ///// </summary>
        ///// <returns>a collection of Standard Providers</returns>
        //IEnumerable<StandardProvider> GetStandardProviders(int standardId);

        ///// <summary>
        ///// Get all the active provider locations for a specific standard
        ///// GET /providers/standard/{standard code}
        ///// </summary>
        ///// <returns>a collection of Standard Providers</returns>
        //Task<IEnumerable<StandardProvider>> GetStandardProvidersAsync(string standardId);

        ///// <summary>
        ///// Get all the active provider locations for a specific standard
        ///// GET /providers/standard/{standard code}
        ///// </summary>
        ///// <returns>a collection of Standard Providers</returns>
        //Task<IEnumerable<StandardProvider>> GetStandardProvidersAsync(int standardId);

        /// <summary>
        /// Get ApprenticeshipTrainingSummary for a provider for page 1 of results
        /// GET providers/{ukprn}/active-apprenticeship-training
        /// </summary>
        /// <returns>ApprenticeshipTrainingSummary (list of apprenticeshipTraining, ukprn, pagination details</returns>
        ApprenticeshipTrainingSummary GetActiveApprenticeshipTrainingByProvider(long ukprn);


        /// <summary>
        /// Get ApprenticeshipTrainingSummary for a provider for a given page
        /// GET providers/{ukprn}/active-apprenticeship-training/{pageNumber}
        /// </summary>
        /// <returns>ApprenticeshipTrainingSummary (list of apprenticeshipTraining, ukprn, pagination details</returns>
        ApprenticeshipTrainingSummary GetActiveApprenticeshipTrainingByProvider(long ukprn, int pageNumber);

        /// <summary>
        /// Get ApprenticeshipTrainingSummary for a provider for page 1 of results
        /// GET providers/{ukprn}/active-apprenticeship-training
        /// </summary>
        /// <returns>ApprenticeshipTrainingSummary (list of apprenticeshipTraining, ukprn, total count of active items</returns>
        Task<ApprenticeshipTrainingSummary> GetActiveApprenticeshipTrainingByProviderAsync(long ukprn);


        /// <summary>
        /// Get ApprenticeshipTrainingSummary for a provider for a given page
        /// GET providers/{ukprn}/active-apprenticeship-training/{pageNumber}
        /// </summary>
        /// <returns>ApprenticeshipTrainingSummary (list of apprenticeshipTraining, ukprn, total count of active items</returns>
        Task<ApprenticeshipTrainingSummary> GetActiveApprenticeshipTrainingByProviderAsync(long ukprn, int pageNumber);



    }
}