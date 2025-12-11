using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Distributed;
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
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public TmdbApiService(TmdbClient client, IDistributedCache cache)
    {
        _client = client;
        _cache = cache;
    }

    // Search movies and TV shows by query
    public async Task<PagedResult<SearchDTO>> SearchAllAsync(
    string query,
    int page,
    CancellationToken cancellationToken = default)
    {
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
        var movieGenres = await GetMoviesGenresAsync("movie", cancellationToken);
        var tvGenres = await GetMoviesGenresAsync("tv", cancellationToken);

        var results = response.Results
            .Where(r => r.MediaType == "movie" || r.MediaType == "tv")
            .Select(r => DTOConverter.ToSearchDto(r, movieGenres, tvGenres))
            .ToList();

        return new PagedResult<SearchDTO>
        {
            Page = response.Page,
            TotalPages = response.TotalPages,
            TotalResults = response.TotalResults,
            Results = results
        };
    }


    /* // Previously: combined GetDetailsAsync. Keep it for compatibility if needed.
     public async Task<MovieDetailsDTO?> GetDetailsAsync(string type, int id, CancellationToken cancellationToken = default)
     {
         switch (type.ToLower())
         {
             case "movie":
                 {
                     var movie = await _client.GetMovieDetailsAsync(id, cancellationToken);
                     return movie is null ? null : ToDetailsDto(movie) as MovieDetailsDTO;
                 }

             case "tv":
                 {
                     var tv = await _client.GetTvDetailsAsync(id, cancellationToken);
                     return tv is null ? null : ToDetailsDto(tv) as MovieDetailsDTO;
                 }

             default:
                 throw new ArgumentException("Type must be 'movie' or 'tv'.", nameof(type));
         }
     }*/

    // Return TMDb movie DTO model
    public async Task<MovieDetailsDTO?> GetMovieDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        var movie = await _client.GetMovieDetailsAsync(id, cancellationToken);

        return ToDetailsDto(movie);
    }

    // Return TMDb tv DTO model
    public async Task<TvDetailsDTO?> GetTvDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        var tv = await _client.GetTvDetailsAsync(id, cancellationToken);

        return ToDetailsDto(tv);
    }

    //public async Task<List<SearchDTO>> DiscoverAsync(string type, CancellationToken cancellationToken = default)
    //{
    //    if (!TryGetTypeContext(type, out var context))
    //    {
    //        throw new ArgumentException("Type must be 'movie' or 'tv'.", nameof(type));
    //    }

    //    var ctx = context!;
    //    var response = await _client.DiscoverAsync(ctx.ApiType, ctx.GenreFilterId, cancellationToken);
    //    return MapCollection(response, ctx.MediaType, ctx.GenreFilterId);
    //}

    //public async Task<List<SearchDTO>> GetPopularAsync(string type, CancellationToken cancellationToken = default)
    //{
    //    if (!TryGetTypeContext(type, out var context))
    //    {
    //        throw new ArgumentException("Type must be 'movie' or 'tv'.", nameof(type));
    //    }

    //    var ctx = context!;
    //    var response = await _client.GetPopularAsync(ctx.ApiType, cancellationToken);
    //    return MapCollection(response, ctx.MediaType, ctx.GenreFilterId);
    //}

    public async Task<List<GenreDTO>> GetMoviesGenresAsync(string type, CancellationToken cancellationToken = default)
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
        var tmdbResponse = await _client.GetGenresAsync(type, cancellationToken);

        if (tmdbResponse?.Genres == null || tmdbResponse.Genres.Count == 0)
            return new List<GenreDTO>();

        // Map TMDb → DTO
        var mappedGenres = tmdbResponse.Genres
            .Where(g => !string.IsNullOrWhiteSpace(g.Name))
            .Select(g => new GenreDTO
            {
                Id = g.Id,
                Name = g.Name!
            })
            .ToList();

        // Save to Redis
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(mappedGenres, JsonOptions),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
            },
            cancellationToken
        );

        return mappedGenres;
    }

    //private static List<SearchDTO> MapCollection(TmdbSearchResponse? response, string mediaTypeOverride, int? requiredGenreId = null)
    //{
    //    if (response?.Results == null)
    //    {
    //        return new List<SearchDTO>();
    //    }

    //    var filtered = requiredGenreId.HasValue
    //        ? response.Results.Where(r => r.GenreIds?.Contains(requiredGenreId.Value) == true)
    //        : response.Results.AsEnumerable();

    //    return filtered
    //        .Select(result => ToSearchMovieDto(result, mediaTypeOverride))
    //        .Where(dto => dto is not null)
    //        .Select(dto => dto!)
    //        .ToList();
    //}
}