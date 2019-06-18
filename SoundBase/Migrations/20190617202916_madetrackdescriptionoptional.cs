using Microsoft.EntityFrameworkCore.Migrations;

namespace SoundBase.Migrations
{
    public partial class madetrackdescriptionoptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Track",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Track",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
