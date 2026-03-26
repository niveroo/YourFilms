using YourFilms.Infrastructure.Db;
using YourFilms.Models;
using YourFilms.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace YourFilms.Services.Authorization
{
    public class RegisterUser
    {
        private readonly YourFilmsDbContext _context;

        public RegisterUser(YourFilmsDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string ErrorMessage)> RegisterAsync(RegisterRequestDTO request)
        {
            var emailValidator = new EmailAddressAttribute();
            if (string.IsNullOrWhiteSpace(request.Email) || !emailValidator.IsValid(request.Email))
            {
                return (false, "Wrong format of email address.");
            }

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return (false, "Email is already taken.");

            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return (false, "Username is already taken.");

            // Hash password
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow
            };

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }
    }
}