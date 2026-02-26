using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social_Media_Activity_Feed.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditMadeToColumnInFollowsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feed_Contents_Users_FollowedID",
                table: "Feed_Contents");

            migrationBuilder.DropForeignKey(
                name: "FK_Follows_Users_FollowedID",
                table: "Follows");

            migrationBuilder.RenameColumn(
                name: "FollowedID",
                table: "Follows",
                newName: "FollowedUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Follows_FollowedID_FollowerID",
                table: "Follows",
                newName: "IX_Follows_FollowedUserID_FollowerID");

            migrationBuilder.RenameColumn(
                name: "FollowedID",
                table: "Feed_Contents",
                newName: "FollowedUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Feed_Contents_FollowerID_FollowedID",
                table: "Feed_Contents",
                newName: "IX_Feed_Contents_FollowerID_FollowedUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Feed_Contents_FollowedID",
                table: "Feed_Contents",
                newName: "IX_Feed_Contents_FollowedUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Feed_Contents_Users_FollowedUserID",
                table: "Feed_Contents",
                column: "FollowedUserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_Users_FollowedUserID",
                table: "Follows",
                column: "FollowedUserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feed_Contents_Users_FollowedUserID",
                table: "Feed_Contents");

            migrationBuilder.DropForeignKey(
                name: "FK_Follows_Users_FollowedUserID",
                table: "Follows");

            migrationBuilder.RenameColumn(
                name: "FollowedUserID",
                table: "Follows",
                newName: "FollowedID");

            migrationBuilder.RenameIndex(
                name: "IX_Follows_FollowedUserID_FollowerID",
                table: "Follows",
                newName: "IX_Follows_FollowedID_FollowerID");

            migrationBuilder.RenameColumn(
                name: "FollowedUserID",
                table: "Feed_Contents",
                newName: "FollowedID");

            migrationBuilder.RenameIndex(
                name: "IX_Feed_Contents_FollowerID_FollowedUserID",
                table: "Feed_Contents",
                newName: "IX_Feed_Contents_FollowerID_FollowedID");

            migrationBuilder.RenameIndex(
                name: "IX_Feed_Contents_FollowedUserID",
                table: "Feed_Contents",
                newName: "IX_Feed_Contents_FollowedID");

            migrationBuilder.AddForeignKey(
                name: "FK_Feed_Contents_Users_FollowedID",
                table: "Feed_Contents",
                column: "FollowedID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_Users_FollowedID",
                table: "Follows",
                column: "FollowedID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
