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
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public GenresController(TmdbApiService tmdb)
    {
        _tmdb = tmdb;
    }

    [HttpGet("movie")]
    public async Task<ActionResult<List<GenreDTO>>> GetMovieGenres(CancellationToken cancellationToken)
    {
        return await _tmdb.GetMoviesGenresAsync("movie", cancellationToken);
    }

    [HttpGet("tv")]
    public async Task<ActionResult<List<GenreDTO>>> GetTvGenres(CancellationToken cancellationToken)
    {
        return await _tmdb.GetMoviesGenresAsync("tv", cancellationToken);
    }
}