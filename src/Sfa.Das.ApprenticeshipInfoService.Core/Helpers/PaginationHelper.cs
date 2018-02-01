using Sfa.Das.ApprenticeshipInfoService.Core.Models;
namespace Sfa.Das.ApprenticeshipInfoService.Core.Helpers
{
    public class PaginationHelper : IPaginationHelper
    {
        public PaginationDetails GeneratePaginationDetails(int page, int pageSize, int totalCount)
        {
            var paginationDetails = new PaginationDetails { NumberOfRecordsToSkip = 0, Page = 1 };

            if (totalCount <= 0)
            {
                return paginationDetails;
            }

            var pageUsed = page <= 0 ? 1 : page;
            var skip = (pageUsed - 1) * pageSize;
            while (skip >= totalCount)
            {
                skip = skip - pageSize;
            }

            paginationDetails.NumberOfRecordsToSkip = skip;
            paginationDetails.Page = (skip / pageSize) + 1;
            return paginationDetails;
        }
    }
}
