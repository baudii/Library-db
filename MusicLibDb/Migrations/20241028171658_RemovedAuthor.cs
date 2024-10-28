using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Library_MVC.Migrations
{
    /// <inheritdoc />
    public partial class RemovedAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Authors_AuthorId",
                table: "Songs");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropIndex(
                name: "IX_Songs_AuthorId",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Songs");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Songs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Songs");

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Songs",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Songs_AuthorId",
                table: "Songs",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Authors_AuthorId",
                table: "Songs",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id");
        }
    }
}
