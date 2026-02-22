using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social_Media_Activity_Feed.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedHalfOfTheSelfReferencingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Post_Media_Links_PostID",
                table: "Post_Media_Links");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Post_Media_Links",
                table: "Post_Media_Links",
                column: "PostID");

            migrationBuilder.CreateTable(
                name: "Blocked_Accounts",
                columns: table => new
                {
                    BlockingUserID = table.Column<long>(type: "INTEGER", nullable: false),
                    BlockedAccountId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocked_Accounts", x => new { x.BlockingUserID, x.BlockedAccountId });
                    table.ForeignKey(
                        name: "FK_Blocked_Accounts_Users_BlockedAccountId",
                        column: x => x.BlockedAccountId,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Blocked_Accounts_Users_BlockingUserID",
                        column: x => x.BlockingUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Close_Friends",
                columns: table => new
                {
                    AddingUserID = table.Column<long>(type: "INTEGER", nullable: false),
                    CloseFriendAccountId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Close_Friends", x => new { x.AddingUserID, x.CloseFriendAccountId });
                    table.ForeignKey(
                        name: "FK_Close_Friends_Users_AddingUserID",
                        column: x => x.AddingUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Close_Friends_Users_CloseFriendAccountId",
                        column: x => x.CloseFriendAccountId,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Follows",
                columns: table => new
                {
                    FollowerID = table.Column<long>(type: "INTEGER", nullable: false),
                    FollowedID = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Follows", x => new { x.FollowerID, x.FollowedID });
                    table.ForeignKey(
                        name: "FK_Follows_Users_FollowedID",
                        column: x => x.FollowedID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Follows_Users_FollowerID",
                        column: x => x.FollowerID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blocked_Accounts_BlockedAccountId_BlockingUserID",
                table: "Blocked_Accounts",
                columns: new[] { "BlockedAccountId", "BlockingUserID" });

            migrationBuilder.CreateIndex(
                name: "IX_Close_Friends_CloseFriendAccountId_AddingUserID",
                table: "Close_Friends",
                columns: new[] { "CloseFriendAccountId", "AddingUserID" });

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowedID_FollowerID",
                table: "Follows",
                columns: new[] { "FollowedID", "FollowerID" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blocked_Accounts");

            migrationBuilder.DropTable(
                name: "Close_Friends");

            migrationBuilder.DropTable(
                name: "Follows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Post_Media_Links",
                table: "Post_Media_Links");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Media_Links_PostID",
                table: "Post_Media_Links",
                column: "PostID");
        }
    }
}
