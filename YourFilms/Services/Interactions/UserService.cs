using YourFilms.DTOs;
using YourFilms.Infrastructure.Db;
using YourFilms.Models;

namespace YourFilms.Services.Interactions
{
    public class UserService
    {
        private readonly YourFilmsDbContext _context;

        public UserService(YourFilmsDbContext context)
        {
            _context = context;
        }

        public async Task<UserDTO> GetUserDetails(int userId)
        {
            User user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return null;
            }
            return new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
