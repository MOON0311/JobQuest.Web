using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobQuest.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatesalarytype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPosts_Users_UserId",
                table: "JobPosts");

            migrationBuilder.DropIndex(
                name: "IX_JobPosts_UserId",
                table: "JobPosts");

            migrationBuilder.AddColumn<string>(
                name: "SalaryType",
                table: "JobPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ResumeFilePath",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryType",
                table: "JobPosts");

            migrationBuilder.AlterColumn<string>(
                name: "ResumeFilePath",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_UserId",
                table: "JobPosts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPosts_Users_UserId",
                table: "JobPosts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
