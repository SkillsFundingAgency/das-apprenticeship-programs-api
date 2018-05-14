﻿using System.Collections.Generic;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Services
{
    public interface IApprenticeshipSearchService
    {
        List<ApprenticeshipSearchResultsItem> SearchApprenticeships(string keywords, int page);
    }
}
