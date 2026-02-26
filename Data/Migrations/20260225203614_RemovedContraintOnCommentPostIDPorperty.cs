using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social_Media_Activity_Feed.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedContraintOnCommentPostIDPorperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.AddColumn<long>(
                name: "commentID",
                table: "Comments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "commentID");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostID",
                table: "Comments",
                column: "PostID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_PostID",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "commentID",
                table: "Comments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "PostID");
        }
    }
}
