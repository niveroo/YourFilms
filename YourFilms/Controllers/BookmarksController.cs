using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YourFilms.DTOs;
using YourFilms.Models;
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
        [HttpPost("Add")]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] AddBookmarkDTO dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var bookmark = await _bookmarkService.AddBookmarkAsync(userId, dto);

            return Ok(bookmark);
        }

        // DELETE: api/bookmarks
        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Remove(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var deleted = await _bookmarkService.RemoveBookmarkAsync(userId, id);

            if (deleted == false) return NotFound("Bookmark not found");

            return Ok();
        }

        // GET: api/bookmarks/check
        [HttpGet("GetBookmark")]
        [Authorize]
        public async Task<IActionResult> Check([FromQuery] int tmdbId, [FromQuery] string mediaType)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var bookmark = await _bookmarkService.GetBookmarkAsync(userId, tmdbId, mediaType);

            if (bookmark != null)
            {
                return Ok(new { IsBookmarked = true, Bookmark = bookmark });
            }

            return Ok(new { IsBookmarked = false });
        }

        // GET: api/bookmarks/user/me
        [HttpGet("User/GetMyBookmarks")]
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
        [HttpGet("User/{userId}")]
        public async Task<IActionResult> GetUserBookmarks(int userId)
        {
            var bookmarks = await _bookmarkService.GetUserBookmarksAsync(userId);
            return Ok(bookmarks);
        }

        // Update: api/bookmarks/{bookmarkId}
        [HttpPost("Update")]
        [Authorize]
        public async Task<IActionResult> Update(UpdateBookmarkDTO dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var bookmark = await _bookmarkService.UpdateBookmarkAsync(userId, dto);

            if (bookmark == null) return NotFound("Bookmark not found");

            return Ok(bookmark);
        }
    }
}