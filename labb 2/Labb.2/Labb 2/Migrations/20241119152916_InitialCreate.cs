using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Labb_2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Butiker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Butiksnamn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gatuadress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Postnummer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stad = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Butiker", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Forfattare",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fornamn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Efternamn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fodelsedatum = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forfattare", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Forlag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Namn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forlag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kategorier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Namn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Beskrivning = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategorier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bocker",
                columns: table => new
                {
                    ISBN13 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Titel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sprak = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pris = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Utgivningsdatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ForfattareId = table.Column<int>(type: "int", nullable: false),
                    ForlagId = table.Column<int>(type: "int", nullable: false),
                    KategoriId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bocker", x => x.ISBN13);
                    table.ForeignKey(
                        name: "FK_Bocker_Forfattare_ForfattareId",
                        column: x => x.ForfattareId,
                        principalTable: "Forfattare",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bocker_Forlag_ForlagId",
                        column: x => x.ForlagId,
                        principalTable: "Forlag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bocker_Kategorier_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategorier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lagersaldo",
                columns: table => new
                {
                    ButikId = table.Column<int>(type: "int", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Antal = table.Column<int>(type: "int", nullable: false),
                    BokISBN13 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lagersaldo", x => new { x.ButikId, x.ISBN });
                    table.ForeignKey(
                        name: "FK_Lagersaldo_Bocker_BokISBN13",
                        column: x => x.BokISBN13,
                        principalTable: "Bocker",
                        principalColumn: "ISBN13");
                    table.ForeignKey(
                        name: "FK_Lagersaldo_Butiker_ButikId",
                        column: x => x.ButikId,
                        principalTable: "Butiker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bocker_ForfattareId",
                table: "Bocker",
                column: "ForfattareId");

            migrationBuilder.CreateIndex(
                name: "IX_Bocker_ForlagId",
                table: "Bocker",
                column: "ForlagId");

            migrationBuilder.CreateIndex(
                name: "IX_Bocker_KategoriId",
                table: "Bocker",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Lagersaldo_BokISBN13",
                table: "Lagersaldo",
                column: "BokISBN13");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lagersaldo");

            migrationBuilder.DropTable(
                name: "Bocker");

            migrationBuilder.DropTable(
                name: "Butiker");

            migrationBuilder.DropTable(
                name: "Forfattare");

            migrationBuilder.DropTable(
                name: "Forlag");

            migrationBuilder.DropTable(
                name: "Kategorier");
        }
    }
}
