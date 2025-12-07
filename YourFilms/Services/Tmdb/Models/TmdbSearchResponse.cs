using System.Text.Json.Serialization;

namespace YourFilms.Services.Tmdb.Models;

public sealed class TmdbSearchResponse
{
    [JsonPropertyName("page")]
    public int? Page { get; set; }
    [JsonPropertyName("results")]
    public List<TmdbSearchResult>? Results { get; set; }
}

