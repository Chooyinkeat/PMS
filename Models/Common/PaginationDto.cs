namespace PMS.Models.Common
{
    public class PaginationDto
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize > 0 ? PageSize : 10));
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int StartItemIndex => TotalItems == 0 ? 0 : ((CurrentPage - 1) * PageSize) + 1;
        public int EndItemIndex => Math.Min(CurrentPage * PageSize, TotalItems);
    }
}
