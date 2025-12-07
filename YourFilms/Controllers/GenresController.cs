using Microsoft.AspNetCore.Mvc;
using YourFilms.DTOs;
using YourFilms.Services;

namespace YourFilms.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GenresController : ControllerBase
{
    private readonly TmdbApiService _tmdb;

    public GenresController(TmdbApiService tmdb)
    {
        _tmdb = tmdb;
    }

    [HttpGet("movie")]
    public async Task<ActionResult<List<GenreDTO>>> GetMovieGenres(CancellationToken cancellationToken)
    {
        var genres = await _tmdb.GetGenresAsync("movie", cancellationToken);
        return genres.Count == 0 ? NotFound("No movie genres returned from TMDb.") : Ok(genres);
    }

    [HttpGet("tv")]
    public async Task<ActionResult<List<GenreDTO>>> GetTvGenres(CancellationToken cancellationToken)
    {
        var genres = await _tmdb.GetGenresAsync("tv", cancellationToken);
        return genres.Count == 0 ? NotFound("No TV genres returned from TMDb.") : Ok(genres);
    }
}



