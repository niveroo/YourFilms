using YourFilms.Models.Enums;

namespace YourFilms.DTOs
{
    public class UpdateBookmarkDTO
    {
        public int BookmarkId { get; set; }
        public bool IsFavorite { get; set; }
        public BookmarkCategory Category { get; set; }
    }
}
