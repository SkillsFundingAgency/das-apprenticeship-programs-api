namespace SFA.DAS.Apprenticeships.Api.Types.Pagination
{
    public class PaginationDetails
    {
        public int NumberOfRecordsToSkip { get; set; }
        public int Page { get; set; }
        public int TotalCount { get; set; }
        public int NumberPerPage { get; set; }
        public int LastPage { get; set; }
    }
}
