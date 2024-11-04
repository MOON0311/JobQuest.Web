using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobQuest.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class secondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "JobPosts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "JobPosts",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");
        }
    }
}
