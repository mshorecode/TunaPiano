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

app.MapGet("/artists/{artistId}", (TunaPianoDbContext db, int artistId) => 
{ 
    var artist = db.Artists
        .Include(a => a.Songs)
        .SingleOrDefault(a => a.Id == artistId);

    if (artist == null)
    {
        return Results.NotFound();
    }

    var response = new
    {
        id = artistId,
        name = artist.Name,
        age = artist.Age,
        bio = artist.Bio,
        song_count = artist.Songs?.Count(),
        song = artist.Songs?.Select( song => new 
        {
            id = song.Id,
            title = song.Title,
            album = song.Album,
            length = song.Length,
        })
    };

    return Results.Ok(response);
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
    var songToUpdate = db.Songs
        .Include(s => s.Genre)
        .SingleOrDefault(s => s.Id == songId);

    if (songToUpdate == null) 
    {
        return Results.NotFound();
    }

    if (!string.IsNullOrEmpty(songDto.Title)) songToUpdate.Title = songDto.Title;
    if (songDto.ArtistId != 0) songToUpdate.ArtistId = songDto.ArtistId;
    if (!string.IsNullOrEmpty(songDto.Album)) songToUpdate.Album = songDto.Album;
    if (songDto.Length != 0) songToUpdate.Length = songDto.Length;

    if (songDto.GenreIds != null && songDto.GenreIds.Any())
    {
        var genreEntities = db.Genres.Where(g => songDto.GenreIds.Contains(g.Id)).ToList();

        songToUpdate.Genre.Clear();

        foreach (var genre in genreEntities)
        {
            songToUpdate.Genre.Add(genre);
        }
    }

    db.SaveChanges();
    return Results.Ok();
});

app.MapGet("/songs", (TunaPianoDbContext db) =>
{
    return db.Songs.ToList();
});

app.MapGet("/songs/{songId}", (TunaPianoDbContext db, int songId) =>
{
    var song = db.Songs
        .Include(s => s.Artist)
        .Include(s => s.Genre)
        .SingleOrDefault(s => s.Id == songId);

    if (song == null)
    {
        return Results.NotFound();
    }

    var response = new
    {
        id = song.Id,
        title = song.Title,
        artist = new
        {
            id = song.Artist.Id,
            name = song.Artist.Name,
            age = song.Artist.Age,
            bio = song.Artist.Bio,
        },
        album = song.Album,
        length = song.Length,
        genres = song.Genre.Select( genre => new 
        { 
            id = genre.Id,
            description = genre.Description,
        })
    };

    return Results.Ok(response);
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

app.MapGet("/genres/{genreId}", (TunaPianoDbContext db, int genreId) =>
{
    var genre = db.Genres
        .Include(g => g.Song)
        .SingleOrDefault(g => g.Id == genreId);

    if (genre == null)
    {
        return Results.NotFound();
    }

    var response = new
    {
        id = genreId,
        description = genre.Description,
        song = genre.Song.Select(song => new
        {
            id = song.Id,
            title = song.Title,
            artist_id = song.ArtistId,
            album = song.Album,
            length = song.Length,
        }),
    };

    return Results.Ok(response);
});

app.Run();
