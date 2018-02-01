using Sfa.Das.ApprenticeshipInfoService.Core.Models;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Helpers
{
        public interface IPaginationHelper
        {
            PaginationDetails GeneratePaginationDetails(int page, int pageSize, int totalCount);
        }
}
