using YourFilms.Models.Enums;

namespace YourFilms.DTOs
{
    public class AddBookmarkDTO
    {
        public int TmdbId { get; set; }
        public string MediaType { get; set; } // "movie" or "tv"
        public bool IsFavorite { get; set; }
        public BookmarkCategory Category { get; set; }
    }
}
