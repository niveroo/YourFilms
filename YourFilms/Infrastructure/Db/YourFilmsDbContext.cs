using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using YourFilms.Models;

internal class YourFilmsDbContext : DbContext
{
    public YourFilmsDbContext(DbContextOptions<YourFilmsDbContext> options)
        : base(options)
    {
        
    }

    // Define your DbSets here, for example:
    // public DbSet<Film> Films { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }
}