namespace YourFilms.Models
{
    public class Review
    {
        public int Id { get; set; } // Primary key
        public int UserId { get; set; } // Foreign key
        public int MovieId { get; set; } // Foreign key of local db movie id
        public int Rating { get; set; } // Rating of the review
        public string Content { get; set; } // Content of the review
        public DateTime CreatedAt { get; set; } // When the review was created
        public virtual User User { get; set; } // Navigation property to User

    }
}
