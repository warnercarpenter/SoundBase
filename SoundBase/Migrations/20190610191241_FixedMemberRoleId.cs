using Microsoft.EntityFrameworkCore.Migrations;

namespace SoundBase.Migrations
{
    public partial class FixedMemberRoleId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUser_MemberRole_MemberRoleId",
                table: "ProjectUser");

            migrationBuilder.DropColumn(
                name: "UserRoleId",
                table: "ProjectUser");

            migrationBuilder.AlterColumn<int>(
                name: "MemberRoleId",
                table: "ProjectUser",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUser_MemberRole_MemberRoleId",
                table: "ProjectUser",
                column: "MemberRoleId",
                principalTable: "MemberRole",
                principalColumn: "MemberRoleId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUser_MemberRole_MemberRoleId",
                table: "ProjectUser");

            migrationBuilder.AlterColumn<int>(
                name: "MemberRoleId",
                table: "ProjectUser",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "UserRoleId",
                table: "ProjectUser",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUser_MemberRole_MemberRoleId",
                table: "ProjectUser",
                column: "MemberRoleId",
                principalTable: "MemberRole",
                principalColumn: "MemberRoleId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
