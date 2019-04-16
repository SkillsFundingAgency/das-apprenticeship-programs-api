using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Apprenticeships.Api.Types.V3
{
    public abstract class PagedResults<T> where T : class
    {
        public long TotalResults { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public IEnumerable<T> Results { get; set; }
    }
}
