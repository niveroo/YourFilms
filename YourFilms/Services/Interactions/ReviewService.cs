using Microsoft.EntityFrameworkCore;
using YourFilms.DTOs;
using YourFilms.Infrastructure.Db;
using YourFilms.Models;

namespace YourFilms.Services.Interactions
{
    public class ReviewService
    {
        private readonly YourFilmsDbContext _context;
        private readonly MediaSyncService _mediaSyncService;

        public ReviewService(YourFilmsDbContext context, MediaSyncService mediaSyncService)
        {
            _context = context;
            _mediaSyncService = mediaSyncService;
        }

        public async Task<ReviewResponseDTO> AddReviewAsync(int userId, CreateReviewDTO dto)
        {
            // Ensure local media exists
            int localMediaId = await _mediaSyncService.GetLocalMediaIdAsync(dto.TmdbId, dto.MediaType);

            if (localMediaId == 0)
            {
                localMediaId = await _mediaSyncService.AddLocalMediaAsync(dto.TmdbId, dto.MediaType);
            }

            var review = new Review
            {
                UserId = userId,
                MovieId = localMediaId, 
                Rating = dto.Rating,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            
            var user = await _context.Users.FindAsync(userId);

            return new ReviewResponseDTO
            {
                Id = review.Id,
                UserId = review.UserId,
                Username = user?.Username ?? "Unknown",
                Rating = review.Rating,
                Content = review.Content,
                CreatedAt = review.CreatedAt
            };
        }

        public async Task<Review?> UpdateReviewAsync(int userId, UpdateReviewDTO dto)
        {
            var review = await _context.Reviews.FindAsync(dto.reviewId);
            if (review == null || review.UserId != userId) return null;
            review.Rating = dto.Rating;
            review.Content = dto.Content;
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<bool> DeleteReviewAsync(int userId, int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return false;
            if (review.UserId != userId) return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ReviewResponseDTO>> GetReviewsByMediaIdAsync(int mediaId, string mediaType)
        {
            int localMediaId = await _mediaSyncService.GetLocalMediaIdAsync(mediaId, mediaType);
            if (localMediaId == 0) return new List<ReviewResponseDTO>();
            
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.MovieId == localMediaId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewResponseDTO
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Username = r.User.Username,
                    Rating = r.Rating,
                    Content = r.Content,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<ReviewResponseDTO>> GetReviewsByUserIdAsync(int userId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewResponseDTO
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Username = r.User.Username,
                    Rating = r.Rating,
                    Content = r.Content,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }
    }
}
