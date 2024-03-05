using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TunaPiano.Migrations
{
    public partial class rsGenreSongFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GenreSong_Genres_GenresId",
                table: "GenreSong");

            migrationBuilder.DropForeignKey(
                name: "FK_GenreSong_Songs_SongsId",
                table: "GenreSong");

            migrationBuilder.RenameColumn(
                name: "SongsId",
                table: "GenreSong",
                newName: "SongId");

            migrationBuilder.RenameColumn(
                name: "GenresId",
                table: "GenreSong",
                newName: "GenreId");

            migrationBuilder.RenameIndex(
                name: "IX_GenreSong_SongsId",
                table: "GenreSong",
                newName: "IX_GenreSong_SongId");

            migrationBuilder.AddForeignKey(
                name: "FK_GenreSong_Genres_GenreId",
                table: "GenreSong",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GenreSong_Songs_SongId",
                table: "GenreSong",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GenreSong_Genres_GenreId",
                table: "GenreSong");

            migrationBuilder.DropForeignKey(
                name: "FK_GenreSong_Songs_SongId",
                table: "GenreSong");

            migrationBuilder.RenameColumn(
                name: "SongId",
                table: "GenreSong",
                newName: "SongsId");

            migrationBuilder.RenameColumn(
                name: "GenreId",
                table: "GenreSong",
                newName: "GenresId");

            migrationBuilder.RenameIndex(
                name: "IX_GenreSong_SongId",
                table: "GenreSong",
                newName: "IX_GenreSong_SongsId");

            migrationBuilder.AddForeignKey(
                name: "FK_GenreSong_Genres_GenresId",
                table: "GenreSong",
                column: "GenresId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GenreSong_Songs_SongsId",
                table: "GenreSong",
                column: "SongsId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
