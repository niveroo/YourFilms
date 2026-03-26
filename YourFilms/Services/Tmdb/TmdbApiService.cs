using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using YourFilms.DTOs;
using YourFilms.Services.Tmdb;
using YourFilms.Services.Tmdb.Models.Movie;
using YourFilms.Services.Tmdb.Models.Tv;
using static YourFilms.Services.Tmdb.DTOConverter;

namespace YourFilms.Services;

public class TmdbApiService
{
    private readonly TmdbClient _client;
    private readonly IDistributedCache _cache;
    private readonly IConfiguration _configuration;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public TmdbApiService(TmdbClient client, IDistributedCache cache, IConfiguration configuration)
    {
        _client = client;
        _cache = cache;
        _configuration = configuration;
    }

    // Search movies and TV shows by query
    public async Task<PagedResult<SearchDTO>> SearchAllAsync(
    string query,
    int page,
    CancellationToken cancellationToken = default)
    {
        string cacheKey = $"search:{query}:page:{page}";

        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<PagedResult<SearchDTO>>(cached, JsonOptions)
                   ?? new PagedResult<SearchDTO>
                   {
                       Page = 0,
                       TotalPages = 0,
                       TotalResults = 0,
                       Results = new List<SearchDTO>()
                   };
        }

        var response = await _client.SearchAllAsync(query, page, cancellationToken);

        if (response?.Results == null)
        {
            return new PagedResult<SearchDTO>
            {
                Page = 0,
                TotalPages = 0,
                TotalResults = 0,
                Results = new List<SearchDTO>()
            };
        }

        // Fetch both genre sets
        var movieGenres = await GetGenresAsync("movie", cancellationToken);
        var tvGenres = await GetGenresAsync("tv", cancellationToken);

        var results = response.Results
            .Where(r => r.MediaType == "movie" || r.MediaType == "tv")
            .Select(r => DTOConverter.ToSearchDto(r, movieGenres, tvGenres))
            .ToList();

        var pagedResult = new PagedResult<SearchDTO>
        {
            Page = response.Page,
            TotalPages = response.TotalPages,
            TotalResults = response.TotalResults,
            Results = results
        };

        double cacheHours = _configuration.GetValue<double?>("CacheSettings:SearchExpirationHours") ?? 1;

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(pagedResult, JsonOptions),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(cacheHours)
            },
            cancellationToken
        );

        return pagedResult;
    }

    // Discover movies and TV shows
    public async Task<PagedResult<SearchDTO>> GetDiscoverAsync(
        string type,
        TmdbSortOption sortOption,
        int page = 1,
        int? genreId = null,
        int? year = null,
        CancellationToken cancellationToken = default)
    {
        string cacheKey = $"discover:{type}:sort:{sortOption}:page:{page}:genre:{genreId}:year:{year}";

        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<PagedResult<SearchDTO>>(cached, JsonOptions)
                   ?? new PagedResult<SearchDTO>
                   {
                       Page = 0,
                       TotalPages = 0,
                       TotalResults = 0,
                       Results = new List<SearchDTO>()
                   };
        }

        var titles = await _client.GetDiscoverAsync(type, sortOption, page, genreId, year, cancellationToken);

        if (titles?.Results == null)
        {
            return new PagedResult<SearchDTO>
            {
                Page = 0,
                TotalPages = 0,
                TotalResults = 0,
                Results = new List<SearchDTO>()
            };
        }

        var movieGenres = await GetGenresAsync("movie", cancellationToken);
        var tvGenres = await GetGenresAsync("tv", cancellationToken);

        var results = titles.Results
            .Select(r => DTOConverter.ToSearchDto(r, movieGenres, tvGenres))
            .ToList();

        var pagedResult = new PagedResult<SearchDTO>
        {
            Page = titles.Page,
            TotalPages = titles.TotalPages,
            TotalResults = titles.TotalResults,
            Results = results
        };

        double cacheHours = _configuration.GetValue<double?>("CacheSettings:DiscoverExpirationHours") ?? 1;

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(pagedResult, JsonOptions),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(cacheHours)
            },
            cancellationToken
        );

        return pagedResult;
    }

    // Returns TMDb movie details DTO model
    public async Task<MovieDetailsDTO?> GetMovieDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        var movie = await _client.GetMovieDetailsAsync(id, cancellationToken);

        return ToDetailsDto(movie);
    }

    // Returns TMDb tv details DTO model
    public async Task<TvDetailsDTO?> GetTvDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        var tv = await _client.GetTvDetailsAsync(id, cancellationToken);

        return ToDetailsDto(tv);
    }

    // Get movie or tv genres, with caching
    public async Task<List<GenreDTO>> GetGenresAsync(string type, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"genres:{type}";

        // Try cache
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<List<GenreDTO>>(cached, JsonOptions)
                   ?? new List<GenreDTO>();
        }

        // Fetch from TMDb
        var genres = await _client.GetGenresAsync(type, cancellationToken);

        if (genres?.Genres == null || genres.Genres.Count == 0)
            return new List<GenreDTO>();

        // Map TMDb → DTO
        var mappedGenres = genres.Genres
            .Where(g => !string.IsNullOrWhiteSpace(g.Name))
            .Select(g => new GenreDTO
            {
                Id = g.Id,
                Name = g.Name!
            })
            .ToList();

        // Get expiration from config, fallback to 12 if not set
        double cacheHours = _configuration.GetValue<double?>("CacheSettings:GenresExpirationHours") ?? 12;

        // Save to Redis
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(mappedGenres, JsonOptions),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(cacheHours)
            },
            cancellationToken
        );

        return mappedGenres;
    }

    public async Task<PagedResult<SearchDTO>> GetTrendingAsync(string timeWindow, int page, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"trending:{timeWindow}:page:{page}";

        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<PagedResult<SearchDTO>>(cached, JsonOptions)
                   ?? new PagedResult<SearchDTO>
                   {
                       Page = 0,
                       TotalPages = 0,
                       TotalResults = 0,
                       Results = new List<SearchDTO>()
                   };
        }

        var titles = await _client.GetTrendingAsync(timeWindow, page, cancellationToken);

        if (titles?.Results == null)
        {
            return new PagedResult<SearchDTO>
            {
                Page = 0,
                TotalPages = 0,
                TotalResults = 0,
                Results = new List<SearchDTO>()
            };
        }

        var movieGenres = await GetGenresAsync("movie", cancellationToken);
        var tvGenres = await GetGenresAsync("tv", cancellationToken);

        var results = titles.Results
            .Select(r => DTOConverter.ToSearchDto(r, movieGenres, tvGenres))
            .ToList();

        var pagedResult = new PagedResult<SearchDTO>
        {
            Page = titles.Page,
            TotalPages = titles.TotalPages,
            TotalResults = titles.TotalResults,
            Results = results
        };

        // Get expiration from config, fallback to 2 if not set
        double cacheHours = _configuration.GetValue<double?>("CacheSettings:TrendingExpirationHours") ?? 2;

        await _cache.SetStringAsync(
        cacheKey,
        JsonSerializer.Serialize(pagedResult, JsonOptions),
        new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(cacheHours)
        },
        cancellationToken
    );

        return pagedResult;
    }
}