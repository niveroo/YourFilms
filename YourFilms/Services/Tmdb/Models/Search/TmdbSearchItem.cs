using System.Text.Json.Serialization;

namespace YourFilms.Services.Tmdb.Models.Search
{
    public class TmdbSearchItem
    {
        [JsonPropertyName("genre_ids")]
        public List<int>? GenreIds { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("original_language")]
        public string? OriginalLanguage { get; set; }

        [JsonPropertyName("original_title")]    // Movie
        public string? OriginalTitle { get; set; }
        [JsonPropertyName("original_name")]     //Tv
        public string? OriginalName { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("media_type")]
        public string MediaType { get; set; }

        [JsonPropertyName("popularity")]
        public double? Popularity { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("release_date")]      // Movie
        public string? ReleaseDate { get; set; }
        [JsonPropertyName("first_air_date")]    // Tv
        public string? FirstAirDate { get; set; }

        [JsonPropertyName("title")]             // Movie
        public string? Title { get; set; }
        [JsonPropertyName("name")]              // Tv
        public string? Name { get; set; }

        [JsonPropertyName("vote_average")]
        public double? VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public int? VoteCount { get; set; }
    }
}
