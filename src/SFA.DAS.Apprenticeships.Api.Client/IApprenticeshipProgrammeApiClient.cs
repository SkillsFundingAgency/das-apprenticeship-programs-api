using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.Apprenticeships.Api.Client
{
    public interface IApprenticeshipProgrammeApiClient : IDisposable
    {
	    /// <summary>
	    /// Get all apprenticeship programmes
	    /// GET /apprenticeship-programmes
	    /// </summary>
	    /// <returns>all apprenticeship programmes</returns>
	    IEnumerable<ApprenticeshipSummary> Get();

	    /// <summary>
	    /// Get all apprenticeship programmes
	    /// GET /apprenticeship-programmes
	    /// </summary>
	    /// <returns>all apprenticeship programmes</returns>
	    Task<IEnumerable<ApprenticeshipSummary>> GetAsync();

	    /// <summary>
	    /// Get all apprenticeship programmes
	    /// GET /apprenticeship-programmes
	    /// </summary>
	    /// <returns>all apprenticeship programmes</returns>
	    IEnumerable<ApprenticeshipSearchResultsItem> Search(string keywords, int page = 1);

	    /// <summary>
	    /// Get all apprenticeship programmes
	    /// GET /apprenticeship-programmes
	    /// </summary>
	    /// <returns>all apprenticeship programmes</returns>
	    Task<IEnumerable<ApprenticeshipSearchResultsItem>> SearchAsync(string keywords, int page = 1);
	}
}