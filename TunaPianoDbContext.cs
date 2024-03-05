using Microsoft.EntityFrameworkCore;
using TunaPiano.Models;

public class TunaPianoDbContext : DbContext
{
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Song> Songs { get; set; }

    public TunaPianoDbContext(DbContextOptions<TunaPianoDbContext> context) : base(context)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Song>()
            .HasMany(song => song.Genre)
            .WithMany(genres => genres.Song)
            .UsingEntity(x => x.ToTable("GenreSong"));
    }

}