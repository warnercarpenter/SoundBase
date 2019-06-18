using Microsoft.EntityFrameworkCore.Migrations;

namespace SoundBase.Migrations
{
    public partial class addedfilepathtotrack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Track",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Track");
        }
    }
}
