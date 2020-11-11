using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksAPI.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DollarSeries",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<decimal>(nullable: false),
                    ExchangeDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DollarSeries", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Symbols",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Symbols", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PriceSeries",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    OpenPrice = table.Column<decimal>(nullable: false),
                    MaxPrice = table.Column<decimal>(nullable: false),
                    MinPrice = table.Column<decimal>(nullable: false),
                    ClosingPrice = table.Column<decimal>(nullable: false),
                    Volume = table.Column<int>(nullable: false),
                    SymbolID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceSeries", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PriceSeries_Symbols_SymbolID",
                        column: x => x.SymbolID,
                        principalTable: "Symbols",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceSeries_SymbolID",
                table: "PriceSeries",
                column: "SymbolID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DollarSeries");

            migrationBuilder.DropTable(
                name: "PriceSeries");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Symbols");
        }
    }
}
