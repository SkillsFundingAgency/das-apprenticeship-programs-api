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
		/// <returns>an apprenticeship programme</returns>
		ApprenticeshipSummary Get();

		/// <summary>
		/// Get a single standard details
		/// GET /apprenticeship-programmes
		/// </summary>
		/// <returns>an apprenticeship programme</returns>
		Task<ApprenticeshipSummary> GetAsync();
    }
}