namespace BlazorMovies.Shared.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; }
        public int RecordsPerPage { get; set; } = 10;
    }
}
