using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YourFilms.DTOs;
using YourFilms.Services.Interactions;

namespace YourFilms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarksController : ControllerBase
    {
        private readonly BookmarkService _bookmarkService;

        public BookmarksController(BookmarkService bookmarkService)
        {
            _bookmarkService = bookmarkService;
        }

        // POST: api/bookmarks
        // Добавление в избранное
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] AddBookmarkDTO dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            try
            {
                // Сервис сам синхронизирует медиа с TMDB и сохранит закладку
                var success = await _bookmarkService.AddBookmarkAsync(userId, dto);

                if (!success) return BadRequest("Bookmark already exists or could not be added");

                return Ok(new { Message = "Added to bookmarks" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/bookmarks
        // Удаление из избранного
        // Ожидает query параметры: ?mediaId=10&type=movie
        // ВНИМАНИЕ: mediaId здесь должен быть локальным ID из вашей базы (MovieId), так как сервис ищет по нему.
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Remove([FromQuery] int mediaId, [FromQuery] string type)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var success = await _bookmarkService.RemoveBookmarkAsync(userId, mediaId, type);

            if (!success) return NotFound("Bookmark not found");

            return NoContent();
        }

        // GET: api/bookmarks/check
        // Проверка, есть ли медиа в закладках (и получение ID закладки)
        [HttpGet("check")]
        [Authorize]
        public async Task<IActionResult> Check([FromQuery] int mediaId, [FromQuery] string type)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var bookmark = await _bookmarkService.GetBookmarkAsync(userId, mediaId, type);

            if (bookmark != null)
            {
                return Ok(new { IsBookmarked = true, Bookmark = bookmark });
            }

            return Ok(new { IsBookmarked = false });
        }

        // GET: api/bookmarks/user/me
        [HttpGet("user/me")]
        [Authorize]
        public async Task<IActionResult> GetMyBookmarks()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var bookmarks = await _bookmarkService.GetUserBookmarksAsync(userId);
            return Ok(bookmarks);
        }

        // GET: api/bookmarks/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserBookmarks(int userId)
        {
            var bookmarks = await _bookmarkService.GetUserBookmarksAsync(userId);
            return Ok(bookmarks);
        }
    }
}