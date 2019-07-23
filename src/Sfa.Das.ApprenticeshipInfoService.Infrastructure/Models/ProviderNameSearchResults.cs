﻿using System.Collections.Generic;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Models.Responses;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models
{
    public class ProviderNameSearchResults
    {
        public long TotalResults { get; set; }

        public int ResultsToTake { get; set; }

        public int ActualPage { get; set; }

        public int LastPage { get; set; }

        public string SearchTerm { get; set; }

        public bool HasError { get; set; }

        public IEnumerable<ProviderNameSearchResult> Results { get; set; }

        public ProviderNameSearchResponseCodes ResponseCode { get; set; }
    }
}
