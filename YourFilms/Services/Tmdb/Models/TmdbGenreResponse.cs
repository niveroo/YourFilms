using System.Text.Json.Serialization;

namespace YourFilms.Services.Tmdb.Models;

public sealed class TmdbGenreResponse
{
    [JsonPropertyName("genres")]
    public List<TmdbGenre>? Genres { get; set; }
}

