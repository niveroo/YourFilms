using System.Text.Json.Serialization;
using YourFilms.Services.Tmdb.Models.Tv;

namespace YourFilms.DTOs
{
    public class TvDetailsDTO
    {
        public string MediaType { get; set; } = "tv";
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string OriginalName { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public string FirstAirDate { get; set; } = string.Empty;
        public List<int> EpisodeRunTime { get; set; } = new();
        public string PosterPath { get; set; } = string.Empty;
        public string BackdropPath { get; set; } = string.Empty;
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public double Popularity { get; set; }
        public List<GenreDTO> Genres { get; set; } = new();
        public int? NumberOfSeasons { get; set; }
        public int? NumberOfEpisodes { get; set; }
        public List<SeasonDTO>? Seasons { get; set; }
        public string? Status { get; set; }
    }
}
