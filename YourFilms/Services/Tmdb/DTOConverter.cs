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
        public static SearchDTO ToSearchDto(TmdbSearchItem item, List<GenreDTO> movieGenres, List<GenreDTO> tvGenres)
        {
            List<GenreDTO> resolvedGenres = new();

            if (item.GenreIds != null)
            {
                resolvedGenres =
                    (item.MediaType == "movie" ? movieGenres : tvGenres)
                    .Where(g => item.GenreIds.Contains(g.Id))
                    .ToList();
            }

            string title = "";
            string original = "";
            string release = "";

            if (item.MediaType == "movie")
            {
                title = item.Title ?? "";
                original = item.OriginalTitle ?? "";
                release = item.ReleaseDate ?? "";
            }
            else if (item.MediaType == "tv")
            {
                title = item.Name ?? "";
                original = item.OriginalName ?? "";
                release = item.FirstAirDate ?? "";
            }

            return new SearchDTO
            {
                Id = item.Id,
                MediaType = item.MediaType,
                Title = title,
                OriginalTitle = original,
                OriginalLanguage = item.OriginalLanguage,
                Overview = item.Overview,
                Popularity = item.Popularity,
                PosterPath = item.PosterPath,
                ReleaseDate = release,
                VoteAverage = item.VoteAverage,
                VoteCount = item.VoteCount,
                Genres = resolvedGenres
            };
        }


        // Map TmdbTvItem to SearchDTO
        public static SearchDTO ToSearchDto(TmdbTvItem tmdb, List<GenreDTO> tvGenres)
        {
            var genres = tvGenres
                .Where(g => tmdb.GenreIds.Contains(g.Id))
                .ToList();

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
        public static SearchDTO ToSearchDto(TmdbSearchItem tmdb, List<GenreDTO> movieGenres)
        {
            var genres = movieGenres
                .Where(g => tmdb.GenreIds.Contains(g.Id))
                .ToList();

            var title = string.Empty;
            var original = string.Empty;
            var release = string.Empty;

            switch (tmdb.MediaType)
            {
                case "movie":
                        title = tmdb.Title;
                        original = tmdb.OriginalTitle ?? string.Empty;
                        release = tmdb.ReleaseDate;
                        break;
                case "tv":
                        title = tmdb.Name;
                        original = tmdb.OriginalName ?? string.Empty;
                        release = tmdb.FirstAirDate;
                        break;
            }

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
    }
}
