namespace YourFilms.DTOs
{
    public class SearchMovieDTO
    {
        public int Id { get; set; }
        public string MediaType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? PosterPath { get; set; }
        public string? ReleaseDate { get; set; }
        public List<GenreDTO>? Genres { get; set; }
    }
}
