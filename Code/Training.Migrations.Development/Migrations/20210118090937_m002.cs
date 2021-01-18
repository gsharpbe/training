using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Training.Migrations
{
    public partial class m002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Address",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    IsObsolete = table.Column<bool>(nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(nullable: true),
                    Creator = table.Column<string>(nullable: true),
                    ModifyDate = table.Column<DateTimeOffset>(nullable: true),
                    Modifier = table.Column<string>(nullable: true),
                    IsoCode = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_CountryId",
                table: "Address",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Country_Guid",
                table: "Country",
                column: "Guid",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Country_CountryId",
                table: "Address",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Country_CountryId",
                table: "Address");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropIndex(
                name: "IX_Address_CountryId",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Address");
        }
    }
}
