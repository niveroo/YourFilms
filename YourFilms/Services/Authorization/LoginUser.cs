using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YourFilms.DTOs;
using YourFilms.Infrastructure.Db;
using YourFilms.Models;
using YourFilms.Services;

namespace YourFilms.Services.Authorization
{
    public sealed class LoginUser(YourFilmsDbContext context, TokenProvider tokenProvider)
    {
        public async Task<string?> handle(LoginRequestDTO request)
        {
            /*var user = await context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);*/

            var passwordHasher = new PasswordHasher<User>();

            //Test user
            var user = new Models.User()
            {
                Id = 1,
                Username = "nivero",
                CreatedAt = DateTime.UtcNow,
                Email = "nivero@gmail.com",
            };
            user.PasswordHash = passwordHasher.HashPassword(user, "123");
            //test user

            if (user == null)
            {
                return null;
            }

            string requestPasswordHash = passwordHasher.HashPassword(user, request.Password);
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            string token = tokenProvider.Create(user);
            return token;
        }
    }
}
