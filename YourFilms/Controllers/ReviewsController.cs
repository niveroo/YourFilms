using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YourFilms.DTOs;
using YourFilms.Services.Interactions;

namespace YourFilms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewsController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // POST: api/reviews
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] CreateReviewDTO dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("User ID not found in token");
            }

            try
            {
                var review = await _reviewService.AddReviewAsync(userId, dto);
                return Ok(review);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //## TODO: Add userid and review user id checking
        // DELETE: api/reviews/{id} 
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _reviewService.DeleteReviewAsync(id);

            if (!success) return NotFound("Review not found");

            return Ok();
        }

        // GET: api/reviews/media/{mediaId}
        [HttpGet("media/")]
        public async Task<IActionResult> GetByMedia(int tmdbId, string mediaType)
        {
            var reviews = await _reviewService.GetReviewsByMediaIdAsync(tmdbId, mediaType);
            return Ok(reviews);
        }

        // GET: api/reviews/user/{userId}
        [HttpGet("user/")]
        public async Task<IActionResult> GetByUser()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }
            
            var reviews = await _reviewService.GetReviewsByUserIdAsync(userId);
            return Ok(reviews);
        }
    }
}