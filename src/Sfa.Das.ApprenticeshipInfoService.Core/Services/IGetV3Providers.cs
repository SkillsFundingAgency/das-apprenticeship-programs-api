namespace Sfa.Das.ApprenticeshipInfoService.Core.Services
{
    using System.Collections.Generic;
    using SFA.DAS.Apprenticeships.Api.Types.V3;

    public interface IGetV3Providers
    {
        StandardProviderSearchResult GetByStandardIdAndLocation(int id, double lat, double lon, int page, int pageSize, bool showForNonLevyOnly, bool showNationalOnly, List<DeliveryMode> deliverModes);

        //List<FrameworkProviderSearchResultsItemResponse> GetByFrameworkIdAndLocation(int id, double lat, double lon, int page);
    }
}
