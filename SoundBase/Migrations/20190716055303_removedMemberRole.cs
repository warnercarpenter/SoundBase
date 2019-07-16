using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SoundBase.Migrations
{
    public partial class removedMemberRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUser_MemberRole_MemberRoleId",
                table: "ProjectUser");

            migrationBuilder.DropTable(
                name: "MemberRole");

            migrationBuilder.DropIndex(
                name: "IX_ProjectUser_MemberRoleId",
                table: "ProjectUser");

            migrationBuilder.DropColumn(
                name: "MemberRoleId",
                table: "ProjectUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MemberRoleId",
                table: "ProjectUser",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MemberRole",
                columns: table => new
                {
                    MemberRoleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 55, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberRole", x => x.MemberRoleId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUser_MemberRoleId",
                table: "ProjectUser",
                column: "MemberRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUser_MemberRole_MemberRoleId",
                table: "ProjectUser",
                column: "MemberRoleId",
                principalTable: "MemberRole",
                principalColumn: "MemberRoleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
