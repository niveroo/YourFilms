namespace YourFilms.DTOs;

public sealed class ReviewResponseDTO
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Username { get; init; } = null!;
    public int Rating { get; init; }
    public string Content { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
}
