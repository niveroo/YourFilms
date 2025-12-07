using YourFilms.DTOs;
using YourFilms.Services.Tmdb.Models;
using static YourFilms.Services.Tmdb.DTOConverter;

namespace YourFilms.Services;

public class TmdbApiService
{
    private readonly TmdbClient _client;

    public TmdbApiService(TmdbClient client)
    {
        _client = client;
    }

    // Search movies and TV shows by query
    public async Task<List<SearchMovieDTO>> SearchMoviesAsync(string query, int page, CancellationToken cancellationToken = default)
    {
        var response = await _client.SearchAllAsync(query, page, cancellationToken);

        if (response?.Results == null)
        {
            return new List<SearchMovieDTO>();
        }

        return response.Results
            .Select(result => ToSearchMovieDto(result))
            .Where(dto => dto is not null)
            .Select(dto => dto!)
            .ToList();
    }

    // Get detailed information about a movie or TV show by ID
    public async Task<MovieDTO?> GetDetailsAsync(string type, int id, CancellationToken cancellationToken = default)
    {
        if (!TryGetTypeContext(type, out var context))
        {
            throw new ArgumentException("Type must be 'movie' or 'tv'.", nameof(type));
        }

        var ctx = context!;

        TmdbTitleDetails? details = ctx.ApiType switch
        {
            "movie" => await _client.GetMovieDetailsAsync(id, cancellationToken),
            "tv" => await _client.GetTvDetailsAsync(id, cancellationToken),
            _ => null
        };

        return details is null ? null : ToMovieDto(details, ctx.MediaType);
    }

    public async Task<List<SearchMovieDTO>> DiscoverAsync(string type, CancellationToken cancellationToken = default)
    {
        if (!TryGetTypeContext(type, out var context))
        {
            throw new ArgumentException("Type must be 'movie' or 'tv'.", nameof(type));
        }

        var ctx = context!;
        var response = await _client.DiscoverAsync(ctx.ApiType, ctx.GenreFilterId, cancellationToken);
        return MapCollection(response, ctx.MediaType, ctx.GenreFilterId);
    }

    public async Task<List<SearchMovieDTO>> GetPopularAsync(string type, CancellationToken cancellationToken = default)
    {
        if (!TryGetTypeContext(type, out var context))
        {
            throw new ArgumentException("Type must be 'movie' or 'tv'.", nameof(type));
        }

        var ctx = context!;
        var response = await _client.GetPopularAsync(ctx.ApiType, cancellationToken);
        return MapCollection(response, ctx.MediaType, ctx.GenreFilterId);
    }

    public async Task<List<GenreDTO>> GetGenresAsync(string type, CancellationToken cancellationToken = default)
    {
        if (!TryGetTypeContext(type, out var context))
        {
            throw new ArgumentException("Type must be 'movie' or 'tv''.", nameof(type));
        }

        var ctx = context!;
        var response = await _client.GetGenresAsync(ctx.ApiType, cancellationToken);

        return response?.Genres?
            .Where(g => !string.IsNullOrWhiteSpace(g.Name))
            .Select(g => new GenreDTO { Id = g.Id, Name = g.Name! })
            .ToList() ?? new List<GenreDTO>();
    }

    private static List<SearchMovieDTO> MapCollection(TmdbSearchResponse? response, string mediaTypeOverride, int? requiredGenreId = null)
    {
        if (response?.Results == null)
        {
            return new List<SearchMovieDTO>();
        }

        var filtered = requiredGenreId.HasValue
            ? response.Results.Where(r => r.GenreIds?.Contains(requiredGenreId.Value) == true)
            : response.Results.AsEnumerable();

        return filtered
            .Select(result => ToSearchMovieDto(result, mediaTypeOverride))
            .Where(dto => dto is not null)
            .Select(dto => dto!)
            .ToList();
    }

    

    private static bool TryGetTypeContext(string type, out TmdbTypeContext? context)
    {
        context = null;
        if (string.IsNullOrWhiteSpace(type))
        {
            return false;
        }

        context = type.Trim().ToLowerInvariant() switch
        {
            "movie" => new TmdbTypeContext("movie", "movie", null),
            "tv" => new TmdbTypeContext("tv", "tv", null),
            _ => null
        };

        return context is not null;
    }
}

internal sealed record TmdbTypeContext(string ApiType, string MediaType, int? GenreFilterId);
