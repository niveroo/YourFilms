using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using YourFilms.Services.Tmdb.Models;
using YourFilms.Services.Tmdb.Models.Movie;
using YourFilms.Services.Tmdb.Models.Search;
using YourFilms.Services.Tmdb.Models.Tv;
using YourFilms.Services.Tmdb;

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

    // Search for movies and tv
    public Task<TmdbPagedResponse<TmdbSearchItem>> SearchAllAsync(string query, int page, CancellationToken cancellationToken = default)
    {
        var url = $"search/multi?query={Uri.EscapeDataString(query)}&language=en-US&page={page}";
        return GetAsync<TmdbPagedResponse<TmdbSearchItem>>(url, cancellationToken);
    }


    // Get discover list for movies and tv
    public async Task<TmdbPagedResponse<TmdbSearchItem>?> GetDiscoverAsync(string type, TmdbSortOption sortOption, int page = 1, int? genreId = null, int? year = null, CancellationToken cancellationToken = default)
    {
        var sortBy = sortOption switch
        {
            TmdbSortOption.PopularityDesc => "popularity.desc",
            TmdbSortOption.RatingDesc => "vote_average.desc",
            _ => "popularity.desc"
        };

        var url = $"discover/{type}?language=en-US&sort_by={sortBy}&page={page}&vote_count.gte=1000";

        if (genreId.HasValue)
        {
            url += $"&with_genres={genreId.Value}";
        }

        if (year.HasValue)
        {
            if (type == "movie")
            {
                url += $"&primary_release_year={year.Value}";
            }
            else if (type == "tv")
            {
                url += $"&first_air_date_year={year.Value}";
            }
        }

        var response = await GetAsync<TmdbPagedResponse<TmdbSearchItem>>(url, cancellationToken);
        if (response?.Results != null)
        {
            foreach (var item in response.Results)
            {
                item.MediaType = type;
            }
        }

        return response;
    }

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

    public Task<TmdbPagedResponse<TmdbSearchItem>?> GetTrendingAsync(string timeWindow, int page = 1, CancellationToken cancellationToken = default)
    {
        var url = $"trending/all/{timeWindow}?language=en-US&page={page}";
        return GetAsync<TmdbPagedResponse<TmdbSearchItem>>(url, cancellationToken);
    }

    private Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken)
        => _http.GetFromJsonAsync<T>(url, JsonOptions, cancellationToken);
}

