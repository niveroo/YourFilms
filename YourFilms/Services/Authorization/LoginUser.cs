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
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            var passwordHasher = new PasswordHasher<User>();

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
