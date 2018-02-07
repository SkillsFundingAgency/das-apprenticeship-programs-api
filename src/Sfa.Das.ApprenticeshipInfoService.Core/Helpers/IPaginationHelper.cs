using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using SFA.DAS.Apprenticeships.Api.Types.Pagination;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Helpers
{
        public interface IPaginationHelper
        {
            PaginationDetails GeneratePaginationDetails(int page, int pageSize, int totalCount);
        }
}
