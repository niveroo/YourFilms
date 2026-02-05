using Microsoft.AspNetCore.Http.HttpResults;
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

        public async Task<Bookmark> AddBookmarkAsync(int userId, AddBookmarkDTO dto)
        {
            int localMediaId = await _mediaSyncService.GetOrCreateLocalMediaIdAsync(dto.TmdbId, dto.MediaType);

            var exists = await _context.Bookmarks
                .AnyAsync(b => b.UserId == userId && b.MovieId == localMediaId);
            if (exists)
            {
                throw new InvalidOperationException("Bookmark already exists");
            }

            var bookmark = new Bookmark
            {
                UserId = userId,
                MovieId = localMediaId,
                Category = dto.Category.ToString(),
                IsFavorite = dto.IsFavorite,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookmarks.Add(bookmark);
            await _context.SaveChangesAsync();
            return bookmark;
        }

        public async Task<bool> RemoveBookmarkAsync(int userId, int bookmarkId)
        {
            var bookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Id == bookmarkId);

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

        public async Task<Bookmark> UpdateBookmarkAsync(int userId, UpdateBookmarkDTO dto)
        {
            var bookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.Id == dto.BookmarkId && b.UserId == userId);
            if (bookmark == null)
            {
                throw new InvalidOperationException("Bookmark not found.");
            }

            bookmark.IsFavorite = dto.IsFavorite;
            bookmark.Category = dto.Category.ToString();
            await _context.SaveChangesAsync();
            return bookmark;
        }
    }
}