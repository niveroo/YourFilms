using System.Text.Json.Serialization;

namespace YourFilms.Services.Tmdb.Models;

public sealed class TmdbGenre
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

