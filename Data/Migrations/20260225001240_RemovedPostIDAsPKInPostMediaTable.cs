using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social_Media_Activity_Feed.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedPostIDAsPKInPostMediaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Post_Media_Links",
                table: "Post_Media_Links");

            migrationBuilder.AddColumn<long>(
                name: "PostMediaID",
                table: "Post_Media_Links",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Post_Media_Links",
                table: "Post_Media_Links",
                column: "PostMediaID");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Media_Links_PostID",
                table: "Post_Media_Links",
                column: "PostID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Post_Media_Links",
                table: "Post_Media_Links");

            migrationBuilder.DropIndex(
                name: "IX_Post_Media_Links_PostID",
                table: "Post_Media_Links");

            migrationBuilder.DropColumn(
                name: "PostMediaID",
                table: "Post_Media_Links");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Post_Media_Links",
                table: "Post_Media_Links",
                column: "PostID");
        }
    }
}
