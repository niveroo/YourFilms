using Microsoft.AspNetCore.Mvc;
using YourFilms.DTOs;
using YourFilms.Services;

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

        [HttpGet("search")]
        public async Task<ActionResult<List<SearchMovieDTO>>> Search([FromQuery] string query, int page, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query cannot be empty or whitespace.");
            }

            var movies = await _tmdb.SearchMoviesAsync(query, page, cancellationToken);

            if (movies.Count == 0)
            {
                return NotFound("No results found.");
            }

            return Ok(movies);
        }

        /// <summary>
        /// Get popular movies and TV shows for homepage
        /// GET api/movies/popular
        /// </summary>
        [HttpGet("popular")]
        public async Task<ActionResult<List<SearchMovieDTO>>> GetPopular(string type, CancellationToken cancellationToken)
        {
            var movies = await _tmdb.GetPopularAsync(type, cancellationToken);

            if (movies.Count == 0)
            {
                return NotFound("No popular content found.");
            }

            return Ok(movies);
        }

        [HttpGet("details/{type}/{id:int}")]
        public async Task<ActionResult<MovieDTO>> GetDetails(string type, int id, CancellationToken cancellationToken)
        {
            try
            {
                var movie = await _tmdb.GetDetailsAsync(type, id, cancellationToken);
                return movie is null ? NotFound($"TMDb returned no {type} with id {id}.") : Ok(movie);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("discover/{type}")]
        public async Task<ActionResult<List<MovieDTO>>> Discover(string type, CancellationToken cancellationToken)
        {
            try
            {
                var movies = await _tmdb.DiscoverAsync(type, cancellationToken);
                return movies.Count == 0 ? NotFound($"TMDb returned no {type} titles for discover.") : Ok(movies);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
