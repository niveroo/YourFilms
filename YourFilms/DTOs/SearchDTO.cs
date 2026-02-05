namespace YourFilms.DTOs
{
    public class SearchDTO
    {
        public int Id { get; set; }
        public string MediaType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string OriginalTitle { get; set; } = string.Empty;
        public string? OriginalLanguage { get; set; }
        public string? Overview { get; set; }
        public double? Popularity { get; set; }
        public string? PosterPath { get; set; }
        public string? ReleaseDate { get; set; }
        public double? VoteAverage { get; set; }
        public int? VoteCount { get; set; }
        public List<GenreDTO>? Genres { get; set; }
    }
}
