﻿using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Models
{
	public sealed class ApprenticeshipSearchResults
	{
		public long TotalResults { get; set; }

		public int ResultsToTake { get; set; }

		public int ActualPage { get; set; }

		public int LastPage { get; set; }

		public string SearchTerm { get; set; }

		public string SortOrder { get; set; }

		public IEnumerable<ApprenticeshipSearchResultsItem> Results { get; set; }

		public bool HasError { get; set; }

		public Dictionary<int, long?> LevelAggregation { get; set; }

		public IEnumerable<int> SelectedLevels { get; set; }
	}
}
