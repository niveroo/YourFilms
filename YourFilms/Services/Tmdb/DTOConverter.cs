using System.Linq;
using YourFilms.DTOs;
using YourFilms.Services.Tmdb.Models;
using YourFilms.Services.Tmdb.Models.Movie;
using YourFilms.Services.Tmdb.Models.Search;
using YourFilms.Services.Tmdb.Models.Tv;


namespace YourFilms.Services.Tmdb
{
    public class DTOConverter
    {
        // Map TmdbMovieItem to SearchDTO
        public static SearchDTO ToSearchDto(TmdbMovieItem tmdb)
        {
            var genres = tmdb.GenreIds?
                .Where(id => GenreMap.ContainsKey(id))
                .Select(id => new GenreDTO
                {
                    Id = id,
                    Name = GenreMap[id]
                })
                .ToList() ?? new List<GenreDTO>();

            return new SearchDTO
            {
                Id = tmdb.Id,
                MediaType = "movie",
                Title = tmdb.Title ?? tmdb.OriginalTitle ?? string.Empty,
                OriginalTitle = tmdb.OriginalTitle ?? tmdb.Title ?? string.Empty,
                OriginalLanguage = tmdb.OriginalLanguage,
                Overview = tmdb.Overview,
                Popularity = tmdb.Popularity,
                PosterPath = tmdb.PosterPath,
                ReleaseDate = tmdb.ReleaseDate,
                VoteAverage = tmdb.VoteAverage,
                VoteCount = tmdb.VoteCount,
                Genres = genres
            };
        }

        // Map TmdbTvItem to SearchDTO
        public static SearchDTO ToSearchDto(TmdbTvItem tmdb)
        {
            var genres = tmdb.GenreIds?
                .Where(id => GenreMap.ContainsKey(id))
                .Select(id => new GenreDTO
                {
                    Id = id,
                    Name = GenreMap[id]
                })
                .ToList() ?? new List<GenreDTO>();

            return new SearchDTO
            {
                Id = tmdb.Id,
                MediaType = "tv",
                Title = tmdb.Name ?? string.Empty,
                OriginalTitle = tmdb.OriginalName ?? string.Empty,
                OriginalLanguage = tmdb.OriginalLanguage,
                Overview = tmdb.Overview,
                Popularity = tmdb.Popularity,
                PosterPath = tmdb.PosterPath,
                ReleaseDate = tmdb.FirstAirDate,
                VoteAverage = tmdb.VoteAverage,
                VoteCount = tmdb.VoteCount,
                Genres = genres
            };
        }

        // Map generic TmdbSearchItem to SearchDTO
        public static SearchDTO ToSearchDto(TmdbSearchItem tmdb)
        {
            var genres = tmdb.GenreIds?
                .Where(id => GenreMap.ContainsKey(id))
                .Select(id => new GenreDTO
                {
                    Id = id,
                    Name = GenreMap[id]
                })
                .ToList() ?? new List<GenreDTO>();

            var title = tmdb.Title ?? tmdb.Name ?? tmdb.OriginalTitle ?? tmdb.OriginalName ?? string.Empty;
            var original = tmdb.OriginalTitle ?? tmdb.OriginalName ?? string.Empty;
            var release = tmdb.ReleaseDate ?? tmdb.FirstAirDate;

            return new SearchDTO
            {
                Id = tmdb.Id,
                MediaType = tmdb.MediaType ?? string.Empty,
                Title = title,
                OriginalTitle = original,
                OriginalLanguage = tmdb.OriginalLanguage,
                Overview = tmdb.Overview,
                Popularity = tmdb.Popularity,
                PosterPath = tmdb.PosterPath,
                ReleaseDate = release,
                VoteAverage = tmdb.VoteAverage,
                VoteCount = tmdb.VoteCount,
                Genres = genres
            };
        }

        // Function that converts Tmdb movie details to DTO
        public static MovieDetailsDTO ToDetailsDto(TmdbMovieDetails details)
        {
            var genres = details.Genres?
                .Where(g => !string.IsNullOrWhiteSpace(g.Name))
                .Select(g => new GenreDTO
                {
                    Id = g.Id,
                    Name = g.Name!
                })
                .ToList() ?? new List<GenreDTO>();

            return new MovieDetailsDTO
            {
                Id = details.Id,
                MediaType = "movie",
                Title = details.Title,
                OriginalTitle = details.OriginalTitle,
                Overview = details.Overview,
                ReleaseDate = details.ReleaseDate,
                Runtime = details.Runtime ?? 0,
                PosterPath = details.PosterPath ?? string.Empty,
                BackdropPath = details.BackdropPath ?? string.Empty,
                VoteAverage = details.VoteAverage ?? 0,
                VoteCount = details.VoteCount ?? 0,
                Popularity = details.Popularity ?? 0,
                Genres = genres,
                Status = details.Status,
            };
        }

        // Function that converts Tmdb tv details to DTO
        public static TvDetailsDTO ToDetailsDto(TmdbTvDetails details)
        {
            var genres = details.Genres?
                .Where(g => !string.IsNullOrWhiteSpace(g.Name))
                .Select(g => new GenreDTO
                {
                    Id = g.Id,
                    Name = g.Name!
                })
                .ToList() ?? new List<GenreDTO>();

            return new TvDetailsDTO()
            {
                Id = details.Id,
                MediaType = "tv",
                Name = details.Name,
                OriginalName = details.OriginalName,
                Overview = details.Overview,
                FirstAirDate = details.FirstAirDate,
                EpisodeRunTime = details.EpisodeRunTime,
                PosterPath = details.PosterPath ?? string.Empty,
                BackdropPath = details.BackdropPath ?? string.Empty,
                VoteAverage = details.VoteAverage,
                VoteCount = details.VoteCount,
                Popularity = details.Popularity,
                Genres = genres,
                NumberOfSeasons = details.NumberOfSeasons,
                NumberOfEpisodes = details.NumberOfEpisodes,
                Seasons = details.Seasons?.Select(season => new SeasonDTO
                {
                    SeasonNumber = season.SeasonNumber,
                    Name = season.Name,
                    EpisodeCount = season.EpisodeCount,
                    AirDate = season.AirDate,
                    PosterPath = season.PosterPath,
                    Overview = season.Overview,
                    VoteAverage = season.VoteAverage,
                }).ToList(),
                Status = details.Status
            };
        }


        // Tempera
        private static readonly Dictionary<int, string> GenreMap = new()
    {
        { 28, "Action" },
        { 12, "Adventure" },
        { 16, "Animation" },
        { 35, "Comedy" },
        { 80, "Crime" },
        { 99, "Documentary" },
        { 18, "Drama" },
        { 10751, "Family" },
        { 14, "Fantasy" },
        { 36, "History" },
        { 27, "Horror" },
        { 10402, "Music" },
        { 9648, "Mystery" },
        { 10749, "Romance" },
        { 878, "Science Fiction" },
        { 10770, "TV Movie" },
        { 53, "Thriller" },
        { 10752, "War" },
        { 37, "Western" }
    };
    }
}
