namespace YourFilms.Models
{
    public class Bookmark
    {
        public int Id { get; set; } // Primary key
        public int UserId { get; set; } // Foreign key
        public int MovieId { get; set; } // Foreign key of local db movie id
        public string Category { get; set; } // Category of the bookmark (watchlist, watching, watched, dropped)
        public bool IsFavorite { get; set; } // Is the film bookmarked favorite 
        public DateTime CreatedAt { get; set; } // When the bookmark was created
    }
}
