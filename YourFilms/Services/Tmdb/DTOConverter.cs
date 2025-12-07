using YourFilms.DTOs;
using YourFilms.Services.Tmdb.Models;


namespace YourFilms.Services.Tmdb
{
    public class DTOConverter
    {
        // Map TmdbSearchResult to SearchMovieDTO
        public static SearchMovieDTO? ToSearchMovieDto(TmdbSearchResult tmdb, string? mediaTypeOverride = null)
        {
            if (tmdb.Id is null)
            {
                return null;
            }

            var genres = tmdb.GenreIds?
                .Where(id => GenreMap.ContainsKey(id))
                .Select(id => new GenreDTO
                {
                    Id = id,
                    Name = GenreMap[id]
                })
                .ToList() ?? new List<GenreDTO>();

            return new SearchMovieDTO
            {
                Id = tmdb.Id.Value,
                MediaType = mediaTypeOverride ?? tmdb.MediaType ?? string.Empty,
                Title = tmdb.Title ?? tmdb.Name ?? string.Empty,
                PosterPath = tmdb.PosterPath,
                ReleaseDate = tmdb.ReleaseDate ?? tmdb.FirstAirDate,
                Genres = genres
            };
        }

        // Map TmdbTitleDetails to MovieDTO
        public static MovieDTO ToMovieDto(TmdbTitleDetails details, string mediaType)
        {
            var runtime = details.Runtime ??
                          (details.EpisodeRunTime != null && details.EpisodeRunTime.Count > 0
                              ? details.EpisodeRunTime[0]
                              : 0);

            var genres = details.Genres?
                .Where(g => !string.IsNullOrWhiteSpace(g.Name))
                .Select(g => new GenreDTO
                {
                    Id = g.Id,
                    Name = g.Name!
                })
                .ToList() ?? new List<GenreDTO>();

            return new MovieDTO
            {
                Id = details.Id,
                MediaType = mediaType,
                Title = details.Title ?? details.Name ?? string.Empty,
                OriginalTitle = details.OriginalTitle ?? details.OriginalName ?? details.Title ?? details.Name ?? string.Empty,
                Overview = details.Overview ?? string.Empty,
                ReleaseDate = details.ReleaseDate ?? details.FirstAirDate ?? string.Empty,
                Runtime = runtime,
                PosterPath = details.PosterPath ?? string.Empty,
                BackdropPath = details.BackdropPath ?? string.Empty,
                VoteAverage = details.VoteAverage ?? 0,
                VoteCount = details.VoteCount ?? 0,
                Popularity = details.Popularity ?? 0,
                Genres = genres
            };
        }

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
