namespace YourFilms.DTOs
{
    public class PagedResult<T>
    {
            public int Page { get; set; }
            public int TotalPages { get; set; }
            public int TotalResults { get; set; }
            public List<T> Results { get; set; } = new();
    }
}
