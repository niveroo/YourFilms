using System.Text.Json.Serialization;

namespace YourFilms.Services.Tmdb.Models;

public sealed class TmdbGenresResponse
{
    [JsonPropertyName("genres")]
    public List<TmdbGenre>? Genres { get; set; }
}

