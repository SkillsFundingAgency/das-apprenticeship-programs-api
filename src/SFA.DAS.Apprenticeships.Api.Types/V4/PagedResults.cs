using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types.V4
{
    public abstract class PagedResults<T> where T : class
    {
        public long TotalResults { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public IEnumerable<T> Results { get; set; }
    }
}
