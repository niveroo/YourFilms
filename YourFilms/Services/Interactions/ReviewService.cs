using Microsoft.AspNetCore.Mvc.Formatters;
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

        public async Task<Review> AddReviewAsync(int userId, CreateReviewDTO dto)
        {
            // Ensure local media exists
            int localMediaId = await _mediaSyncService.GetOrCreateLocalMediaIdAsync(dto.TmdbId, dto.MediaType);

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
            return review;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Review>> GetReviewsByMediaIdAsync(int mediaId, string mediaType)
        {
            int localMediaId = await _mediaSyncService.GetOrCreateLocalMediaIdAsync(mediaId, mediaType);
            
            return await _context.Reviews
                .Where(r => r.MovieId == localMediaId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Review>> GetReviewsByUserIdAsync(int userId)
        {
            return await _context.Reviews
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}