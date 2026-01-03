using Microsoft.AspNetCore.Mvc;
using YourFilms.DTOs;
using YourFilms.Services;
using YourFilms.Services.Tmdb;

namespace YourFilms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly TmdbApiService _tmdb;

        public MoviesController(TmdbApiService tmdb)
        {
            _tmdb = tmdb;
        }

        // GET api/movies/search Search all by name
        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<SearchDTO>>> Search([FromQuery] string query, int page, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query cannot be empty.");

            var result = await _tmdb.SearchAllAsync(query, page, cancellationToken);

            if (result.Results.Count == 0)
                return NotFound("No results found.");

            return Ok(result);
        }

        // GET api/movies/discover/{type} Search with filters
        [HttpGet("discover/{type}")]
        public async Task<ActionResult<PagedResult<SearchDTO>>> Discover(
            string type,
            [FromQuery] TmdbSortOption sort = TmdbSortOption.PopularityDesc,
            [FromQuery] int page = 1,
            [FromQuery] int? genreId = null,
            [FromQuery] int? year = null,
            CancellationToken cancellationToken = default)
        {
            if (page < 1) page = 1;

            if (type != "movie" && type != "tv")
            {
                return BadRequest("Type must be 'movie' or 'tv'.");
            }

            var result = await _tmdb.GetDiscoverAsync(type, sort, page, genreId, year, cancellationToken);
            if (result.Results.Count == 0)
            {
                return NotFound("No results found.");
            }

            return Ok(result);
        }

        // GET api/movies/details/{type}/{id} Returns detailed information about a movie or TV show //
        [HttpGet("details/movie/{id:int}")]
        public async Task<ActionResult<MovieDetailsDTO>> GetMovieDetails(int id, CancellationToken cancellationToken)
        {
            try
            {
                var title = await _tmdb.GetMovieDetailsAsync(id, cancellationToken);
                return title is null ? NotFound($"TMDb returned no movie with id {id}.") : Ok(title);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/movies/details/{type}/{id} Returns detailed information about a movie or TV show //
        [HttpGet("details/tv/{id:int}")]
        public async Task<ActionResult<TvDetailsDTO>> GetTvDetails(int id, CancellationToken cancellationToken)
        {
            try
            {
                var title = await _tmdb.GetTvDetailsAsync(id, cancellationToken);
                return title is null ? NotFound($"TMDb returned no tv with id {id}.") : Ok(title);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}