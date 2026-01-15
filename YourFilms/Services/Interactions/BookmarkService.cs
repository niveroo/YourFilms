using Microsoft.EntityFrameworkCore;
using YourFilms.DTOs;
using YourFilms.Infrastructure.Db;
using YourFilms.Models;

namespace YourFilms.Services.Interactions
{
    public class BookmarkService
    {
        private readonly YourFilmsDbContext _context;
        private readonly MediaSyncService _mediaSyncService;

        public BookmarkService(YourFilmsDbContext context, MediaSyncService mediaSyncService)
        {
            _context = context;
            _mediaSyncService = mediaSyncService;
        }

        public async Task<bool> AddBookmarkAsync(int userId, AddBookmarkDTO dto)
        {
            // Ensure local media exists
            int localMediaId = await _mediaSyncService.GetOrCreateLocalMediaIdAsync(dto.TmdbId, dto.MediaType);

            string categoryString = dto.Category.ToString();

            var exists = await _context.Bookmarks
                .AnyAsync(b => b.UserId == userId && b.MovieId == localMediaId);

            if (exists) return false;

            var bookmark = new Bookmark
            {
                UserId = userId,
                MovieId = localMediaId,
                Category = categoryString,
                IsFavorite = dto.IsFavorite,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookmarks.Add(bookmark);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveBookmarkAsync(int userId, int mediaId, string mediaType)
        {
            var bookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.MovieId == mediaId && b.Category == mediaType);

            if (bookmark == null) return false;

            _context.Bookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Bookmark>> GetUserBookmarksAsync(int userId)
        {
            return await _context.Bookmarks
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Bookmark?> GetBookmarkAsync(int userId, int mediaId, string mediaType)
        {
            int localMediaId = await _mediaSyncService.GetOrCreateLocalMediaIdAsync(mediaId, mediaType);
            return await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.MovieId == localMediaId);
        }
    }
}