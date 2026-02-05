namespace YourFilms.DTOs
{
    public class CreateReviewDTO
    {
        public int TmdbId { get; set; }
        public string MediaType { get; set; } // "movie" or "tv"
        public int Rating { get; set; }
        public string Content { get; set; }
    }
}
