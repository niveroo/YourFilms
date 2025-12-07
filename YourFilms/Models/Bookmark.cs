namespace YourFilms.Models
{
    public class Bookmark
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public string Category { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
