using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using YourFilms.DTOs;
using YourFilms.Services;

namespace YourFilms.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GenresController : ControllerBase
{
    private readonly TmdbApiService _tmdb;
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public GenresController(TmdbApiService tmdb, IDistributedCache cache)
    {
        _tmdb = tmdb;
        _cache = cache;
    }

    [HttpGet("movie")]
    public async Task<ActionResult<List<GenreDTO>>> GetMovieGenres(CancellationToken cancellationToken)
    {
        const string cacheKey = "genres:movie";

        // Try from cache
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached != null)
        {
            var genres = JsonSerializer.Deserialize<List<GenreDTO>>(cached, JsonOptions);
            return Ok(genres);
        }

        // Fetch from TMDb
        var fresh = await _tmdb.GetGenresAsync("movie", cancellationToken);
        if (fresh.Count == 0) return NotFound("No movie genres returned from TMDb.");

        // Save to Redis
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(fresh, JsonOptions),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)  // adjust as needed
            },
            cancellationToken
        );

        return Ok(fresh);
    }

    [HttpGet("tv")]
    public async Task<ActionResult<List<GenreDTO>>> GetTvGenres(CancellationToken cancellationToken)
    {
        const string cacheKey = "genres:tv";

        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached != null)
        {
            var genres = JsonSerializer.Deserialize<List<GenreDTO>>(cached, JsonOptions);
            return Ok(genres);
        }

        var fresh = await _tmdb.GetGenresAsync("tv", cancellationToken);
        if (fresh.Count == 0) return NotFound("No TV genres returned from TMDb.");

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(fresh, JsonOptions),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
            },
            cancellationToken
        );

        return Ok(fresh);
    }
}