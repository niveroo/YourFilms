using Microsoft.EntityFrameworkCore;
using YourFilms.Infrastructure.Db;
using YourFilms.Models;

namespace YourFilms.Services
{
    public class MediaSyncService
    {
        private readonly YourFilmsDbContext _context;
        private readonly TmdbApiService _tmdbService;

        public MediaSyncService(YourFilmsDbContext context, TmdbApiService tmdbService)
        {
            _context = context;
            _tmdbService = tmdbService;
        }

        public async Task<int> GetOrCreateLocalMediaIdAsync(int tmdbId, string mediaType)
        {
            // 1. Check if it already exists locally
            var existingMedia = await _context.Movies
                .FirstOrDefaultAsync(m => m.TmdbId == tmdbId && m.MediaType == mediaType);

            if (existingMedia != null)
            {
                return existingMedia.Id;
            }

            // 2. Fetch from TMDB
            Movie newMovie = null;

            if (mediaType == "movie")
            {
                var movieDetails = await _tmdbService.GetMovieDetailsAsync(tmdbId);
                if (movieDetails == null)
                {
                    throw new Exception($"Movie with TMDB ID {tmdbId} not found.");
                }

                newMovie = new Movie
                {
                    TmdbId = movieDetails.Id,
                    MediaType = "movie",
                    Title = movieDetails.Title,
                    ReleaseDate = DateTime.TryParse(movieDetails.ReleaseDate, out var date) ? date.ToUniversalTime() : DateTime.UtcNow,
                    PosterPath = movieDetails.PosterPath,
                    LastSync = DateTime.UtcNow
                };
            }
            else if (mediaType == "tv")
            {
                var tvDetails = await _tmdbService.GetTvDetailsAsync(tmdbId);
                if (tvDetails == null)
                {
                    throw new Exception($"TV Show with TMDB ID {tmdbId} not found.");
                }

                newMovie = new Movie
                {
                    TmdbId = tvDetails.Id,
                    MediaType = "tv",
                    Title = tvDetails.Name, // TV shows use Name, not Title
                    ReleaseDate = DateTime.TryParse(tvDetails.FirstAirDate, out var date) ? date.ToUniversalTime() : DateTime.UtcNow,
                    PosterPath = tvDetails.PosterPath,
                    LastSync = DateTime.UtcNow
                };
            }
            else
            {
                throw new ArgumentException("Invalid media type. Must be 'movie' or 'tv'.");
            }

            // 3. Save to local DB
            _context.Movies.Add(newMovie);
            await _context.SaveChangesAsync();

            return newMovie.Id;
        }
    }
}
