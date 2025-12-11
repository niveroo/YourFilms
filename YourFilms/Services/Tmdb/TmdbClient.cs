using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using YourFilms.Services.Tmdb.Models;
using YourFilms.Services.Tmdb.Models.Movie;
using YourFilms.Services.Tmdb.Models.Search;
using YourFilms.Services.Tmdb.Models.Tv;

namespace YourFilms.Services;

public class TmdbClient
{
    private const string BaseUrl = "https://api.themoviedb.org/3/";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _http;

    public TmdbClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _http.BaseAddress ??= new Uri(BaseUrl);

        if (!_http.DefaultRequestHeaders.Accept.Any())
        {
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        var apiKey = config["TMDB:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("TMDB:ApiKey is not configured.");
        }

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public Task<TmdbPagedResponse<TmdbSearchItem>> SearchAllAsync(string query, int page, CancellationToken cancellationToken = default)
    {
        var url = $"search/multi?query={Uri.EscapeDataString(query)}&language=en-US&page={page}";
        return GetAsync<TmdbPagedResponse<TmdbSearchItem>>(url, cancellationToken);
    }

    /*public Task<TmdbSearchResponse?> DiscoverAsync(string type, int? genreId = null, CancellationToken cancellationToken = default)
    {
        var url = $"discover/{type}?language=en-US&sort_by=popularity.desc";
        if (genreId.HasValue)
        {
            url += $"&with_genres={genreId.Value}";
        }

        return GetAsync<TmdbSearchResponse>(url, cancellationToken);
    }

    public Task<TmdbSearchResponse?> GetPopularAsync(string type, CancellationToken cancellationToken = default)
    {
        var url = $"{type}/popular?language=en-US";
        return GetAsync<TmdbSearchResponse>(url, cancellationToken);
    }*/

    // Get title details by id
    public Task<TmdbMovieDetails?> GetMovieDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        var url = $"movie/{id}?language=en-US";
        return GetAsync<TmdbMovieDetails>(url, cancellationToken);
    }

    public Task<TmdbTvDetails?> GetTvDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        var url = $"tv/{id}?language=en-US";
        return GetAsync<TmdbTvDetails>(url, cancellationToken);
    }
    //

    //  Get genres list
    public Task<TmdbGenresResponse?> GetGenresAsync(string type, CancellationToken cancellationToken = default)
    {
        var url = $"genre/{type}/list?language=en-US";
        return GetAsync<TmdbGenresResponse>(url, cancellationToken);
    }
    //

    private Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken)
        => _http.GetFromJsonAsync<T>(url, JsonOptions, cancellationToken);
}

