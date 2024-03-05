using TunaPiano.Models;
using TunaPiano.Dto;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the datbase through Entity Framework Core
builder.Services.AddNpgsql<TunaPianoDbContext>(builder.Configuration["TunaPianoDbConnectionString"]);

// set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ARTISTS
app.MapPost("/artists", (TunaPianoDbContext db, ArtistDto artistDto) =>
{
    Artist artist = new()
    {
        Id = 0,
        Name = artistDto.Name,
        Age = artistDto.Age,
        Bio = artistDto.Bio,
    };

    db.Artists.Add(artist);
    db.SaveChanges();
    return Results.Created($"/artists/{artist.Id}", artist);
});

app.MapDelete("/artists/{artistId}", (TunaPianoDbContext db, int artistId) => 
{
    var artist = db.Artists.SingleOrDefault(a => a.Id == artistId);

    if (artist == null)
    {
        return Results.NotFound();
    }

    db.Artists.Remove(artist);
    db.SaveChanges();
    return Results.NoContent();
});

app.MapPatch("/artists/{artistId}", (TunaPianoDbContext db, int artistId, ArtistDto artistDto) =>
{
    var artistToUpdate = db.Artists.SingleOrDefault(a => a.Id == artistId);

    if (artistToUpdate == null)
    {
        return Results.NotFound();
    }

    if (!string.IsNullOrEmpty(artistDto.Name)) artistToUpdate.Name = artistDto.Name;
    if (artistDto.Age != 0) artistToUpdate.Age = artistDto.Age;
    if (!string.IsNullOrEmpty(artistDto.Bio)) artistToUpdate.Bio = artistDto.Bio;

    db.SaveChanges();
    return Results.Ok();
});

app.MapGet("/artists", (TunaPianoDbContext db) =>
{
    return db.Artists.ToList();
});


// SONGS
app.MapPost("/songs", (TunaPianoDbContext db, SongDto songDto) =>
{
    Song song = new()
    {
        Id = 0,
        Title = songDto.Title,
        ArtistId = songDto.ArtistId,
        Album = songDto.Album,
        Length = songDto.Length,
    };

    db.Songs.Add(song);
    db.SaveChanges();
    return Results.Created($"/songs/{song.Id}", song);
});

app.MapDelete("/songs/{songId}", (TunaPianoDbContext db, int songId) =>
{
    var song = db.Songs.SingleOrDefault(s => s.Id == songId);

    if (song == null)
    {
        return Results.NotFound();
    }

    db.Songs.Remove(song);
    db.SaveChanges();
    return Results.NoContent();
});

app.MapPatch("/songs/{songId}", (TunaPianoDbContext db, int songId, SongDto songDto) =>
{
    var songToUpdate = db.Songs.SingleOrDefault(s => s.Id == songId);

    if (songToUpdate == null) 
    {
        return Results.NotFound();
    }

    if (!string.IsNullOrEmpty(songDto.Title)) songToUpdate.Title = songDto.Title;
    if (songDto.ArtistId != 0) songToUpdate.ArtistId = songDto.ArtistId;
    if (!string.IsNullOrEmpty(songDto.Album)) songToUpdate.Album = songDto.Album;
    if (songDto.Length != 0) songToUpdate.Length = songDto.Length;

    db.SaveChanges();
    return Results.Ok();
});

app.MapGet("/songs", (TunaPianoDbContext db) =>
{
    return db.Songs.ToList();
});

app.MapPatch("/songs/genre", (TunaPianoDbContext db, GenreSongDto genreSongDto) =>
{
    var song = db.Songs.Include(s => s.Genre).SingleOrDefault(s => s.Id == genreSongDto.SongId);
    var genre = db.Genres.Find(genreSongDto.GenreId);

    if (song == null || genre == null)
    {
        return Results.NotFound();
    }

    song.Genre.Add(genre);
    db.SaveChanges();
    return Results.Created($"/songs/genre/{genreSongDto.GenreId}", song);
});

app.MapGet("/songs/{songId}", (TunaPianoDbContext db, int songId) =>
{
    var song = db.Songs.Include(s => s.Genre).SingleOrDefault(s => s.Id == songId);

    if (song == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(song);
});

// GENRES
app.MapPost("/genres", (TunaPianoDbContext db, GenreDto genreDto) =>
{
    Genre genre = new()
    { 
        Id = 0,
        Description = genreDto.Description,
    };

    db.Genres.Add(genre);
    db.SaveChanges();
    return Results.Created($"/genres/{genre.Id}", genre);
});

app.MapDelete("/genres/{genreId}", (TunaPianoDbContext db, int genreId) => 
{
    var genre = db.Genres.SingleOrDefault(g => g.Id == genreId);

    if (genre == null)
    {
        return Results.NotFound();
    }

    db.Genres.Remove(genre);
    db.SaveChanges();
    return Results.NoContent();
});

app.MapPatch("/genres/{genreId}", (TunaPianoDbContext db, int genreId, GenreDto genreDto) => 
{ 
    var genreToUpdate = db.Genres.SingleOrDefault(d =>  d.Id == genreId);

    if (genreToUpdate == null) 
    {
        return Results.NotFound();
    };

    if (!string.IsNullOrEmpty(genreDto.Description)) genreToUpdate.Description = genreDto.Description;

    db.SaveChanges();
    return Results.Ok();
});

app.MapGet("/genres", (TunaPianoDbContext db) =>
{
    return db.Genres.ToList();
});

app.Run();
