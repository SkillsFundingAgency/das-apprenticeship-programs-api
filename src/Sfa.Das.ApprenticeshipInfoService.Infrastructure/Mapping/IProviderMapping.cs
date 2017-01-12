﻿using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Models.Responses;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    public interface IProviderMapping
    {
        ProviderDTO MapToProviderDto(Provider provider);

        ApprenticeshipDetails MapToProvider(StandardProviderSearchResultsItem item, int selectedLocationId);

        ApprenticeshipDetails MapToProvider(FrameworkProviderSearchResultsItem item, int selectedLocationId);

        StandardProviderSearchResultsItemResponse MapToStandardProviderResponse(StandardProviderSearchResultsItem item);

        FrameworkProviderSearchResultsItemResponse MapToFrameworkProviderResponse(FrameworkProviderSearchResultsItem item);
    }
}
