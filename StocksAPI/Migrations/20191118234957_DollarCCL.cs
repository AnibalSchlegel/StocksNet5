using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksAPI.Migrations
{
    public partial class DollarCCL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceSeries_Symbols_SymbolID",
                table: "PriceSeries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Symbols",
                table: "Symbols");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceSeries",
                table: "PriceSeries");

            migrationBuilder.DropIndex(
                name: "IX_PriceSeries_SymbolID",
                table: "PriceSeries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DollarSeries",
                table: "DollarSeries");

            migrationBuilder.DropColumn(
                name: "SymbolID",
                table: "PriceSeries");

            migrationBuilder.RenameTable(
                name: "Symbols",
                newName: "Symbol");

            migrationBuilder.RenameTable(
                name: "PriceSeries",
                newName: "PriceData");

            migrationBuilder.RenameTable(
                name: "DollarSeries",
                newName: "DollarData");

            migrationBuilder.AddColumn<int>(
                name: "Symbol_ID",
                table: "PriceData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DollarType",
                table: "DollarData");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Symbol",
                table: "Symbol",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceData",
                table: "PriceData",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DollarData",
                table: "DollarData",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_PriceData_Symbol_ID",
                table: "PriceData",
                column: "Symbol_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceData_Symbol_Symbol_ID",
                table: "PriceData",
                column: "Symbol_ID",
                principalTable: "Symbol",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceData_Symbol_Symbol_ID",
                table: "PriceData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Symbol",
                table: "Symbol");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceData",
                table: "PriceData");

            migrationBuilder.DropIndex(
                name: "IX_PriceData_Symbol_ID",
                table: "PriceData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DollarData",
                table: "DollarData");

            migrationBuilder.DropColumn(
                name: "Symbol_ID",
                table: "PriceData");

            migrationBuilder.DropColumn(
                name: "DollarType",
                table: "DollarData");

            migrationBuilder.RenameTable(
                name: "Symbol",
                newName: "Symbols");

            migrationBuilder.RenameTable(
                name: "PriceData",
                newName: "PriceSeries");

            migrationBuilder.RenameTable(
                name: "DollarData",
                newName: "DollarSeries");

            migrationBuilder.AddColumn<int>(
                name: "SymbolID",
                table: "PriceSeries",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Symbols",
                table: "Symbols",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceSeries",
                table: "PriceSeries",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DollarSeries",
                table: "DollarSeries",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_PriceSeries_SymbolID",
                table: "PriceSeries",
                column: "SymbolID");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceSeries_Symbols_SymbolID",
                table: "PriceSeries",
                column: "SymbolID",
                principalTable: "Symbols",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
