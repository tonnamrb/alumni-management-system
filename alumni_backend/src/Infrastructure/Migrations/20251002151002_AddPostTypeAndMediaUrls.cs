using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPostTypeAndMediaUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MediaUrls",
                table: "Posts",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Type",
                table: "Posts",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Type_CreatedAt",
                table: "Posts",
                columns: new[] { "Type", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_Type",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_Type_CreatedAt",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "MediaUrls",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Posts");
        }
    }
}
