using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social_Media_Activity_Feed.Data.Migrations
{
    /// <inheritdoc />
    public partial class StoppedAtTPHNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Post_Likes",
                table: "Post_Likes");

            migrationBuilder.DropIndex(
                name: "IX_Post_Likes_LikerID",
                table: "Post_Likes");

            migrationBuilder.DropIndex(
                name: "IX_Post_Likes_PostID_LikerID",
                table: "Post_Likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_PostID",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "PostLikeID",
                table: "Post_Likes");

            migrationBuilder.RenameColumn(
                name: "CommentID",
                table: "Comments",
                newName: "LikeCount");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Posts",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Posts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Post_Likes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "LikeCount",
                table: "Comments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Comments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Post_Likes",
                table: "Post_Likes",
                columns: new[] { "LikerID", "PostID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "PostID");

            migrationBuilder.CreateTable(
                name: "Feed_Contents",
                columns: table => new
                {
                    UserID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FollowerID = table.Column<long>(type: "INTEGER", nullable: false),
                    FollowedID = table.Column<long>(type: "INTEGER", nullable: false),
                    PostID = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feed_Contents", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_Feed_Contents_Posts_PostID",
                        column: x => x.PostID,
                        principalTable: "Posts",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feed_Contents_Users_FollowedID",
                        column: x => x.FollowedID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SenderID = table.Column<long>(type: "INTEGER", nullable: false),
                    ReceiptientID = table.Column<long>(type: "INTEGER", nullable: false),
                    MessageContent = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK_Messages_Users_ReceiptientID",
                        column: x => x.ReceiptientID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Users_SenderID",
                        column: x => x.SenderID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    ReceivingUserID = table.Column<long>(type: "INTEGER", nullable: false),
                    InitaiatorID = table.Column<long>(type: "INTEGER", nullable: false),
                    NotificationType = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => new { x.ReceivingUserID, x.InitaiatorID });
                    table.ForeignKey(
                        name: "FK_Notifications_Users_InitaiatorID",
                        column: x => x.InitaiatorID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_ReceivingUserID",
                        column: x => x.ReceivingUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Message_Media_Links",
                columns: table => new
                {
                    MessageID = table.Column<long>(type: "INTEGER", nullable: false),
                    MediaType = table.Column<int>(type: "INTEGER", nullable: false),
                    MediaURL = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message_Media_Links", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK_Message_Media_Links_Messages_MessageID",
                        column: x => x.MessageID,
                        principalTable: "Messages",
                        principalColumn: "MessageID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_Likes_PostID",
                table: "Post_Likes",
                column: "PostID");

            migrationBuilder.CreateIndex(
                name: "IX_Feed_Contents_FollowedID",
                table: "Feed_Contents",
                column: "FollowedID");

            migrationBuilder.CreateIndex(
                name: "IX_Feed_Contents_FollowerID_FollowedID",
                table: "Feed_Contents",
                columns: new[] { "FollowerID", "FollowedID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feed_Contents_PostID",
                table: "Feed_Contents",
                column: "PostID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiptientID_SenderID",
                table: "Messages",
                columns: new[] { "ReceiptientID", "SenderID" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderID_ReceiptientID",
                table: "Messages",
                columns: new[] { "SenderID", "ReceiptientID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_InitaiatorID",
                table: "Notifications",
                column: "InitaiatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationType",
                table: "Notifications",
                column: "NotificationType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feed_Contents");

            migrationBuilder.DropTable(
                name: "Message_Media_Links");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Post_Likes",
                table: "Post_Likes");

            migrationBuilder.DropIndex(
                name: "IX_Post_Likes_PostID",
                table: "Post_Likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Post_Likes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "LikeCount",
                table: "Comments",
                newName: "CommentID");

            migrationBuilder.AddColumn<long>(
                name: "PostLikeID",
                table: "Post_Likes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<long>(
                name: "CommentID",
                table: "Comments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Post_Likes",
                table: "Post_Likes",
                column: "PostLikeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "CommentID");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Likes_LikerID",
                table: "Post_Likes",
                column: "LikerID");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Likes_PostID_LikerID",
                table: "Post_Likes",
                columns: new[] { "PostID", "LikerID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostID",
                table: "Comments",
                column: "PostID",
                unique: true);
        }
    }
}
