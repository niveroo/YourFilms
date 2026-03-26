using Microsoft.EntityFrameworkCore;
using YourFilms.Models;

namespace YourFilms.Infrastructure.Db;
public class YourFilmsDbContext : DbContext
{
    public YourFilmsDbContext(DbContextOptions<YourFilmsDbContext> options)
        : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Movie>()
            .HasIndex(m => new { m.TmdbId, m.MediaType })
            .IsUnique();

        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.UserId, r.MovieId })
            .IsUnique();

        modelBuilder.Entity<Bookmark>()
            .HasIndex(b => new { b.UserId, b.MovieId })
            .IsUnique();

        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Movie)
            .WithMany()
            .HasForeignKey(r => r.MovieId);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }
}