using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social_Media_Activity_Feed.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedPostTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    PostID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InitiatorID = table.Column<long>(type: "INTEGER", nullable: false),
                    Caption = table.Column<string>(type: "TEXT", maxLength: 800, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.PostID);
                    table.ForeignKey(
                        name: "FK_Posts_Users_InitiatorID",
                        column: x => x.InitiatorID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PostID = table.Column<long>(type: "INTEGER", nullable: false),
                    CommenterID = table.Column<long>(type: "INTEGER", nullable: false),
                    CommentText = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentID);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostID",
                        column: x => x.PostID,
                        principalTable: "Posts",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Users_CommenterID",
                        column: x => x.CommenterID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post_Likes",
                columns: table => new
                {
                    PostLikeID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LikerID = table.Column<long>(type: "INTEGER", nullable: false),
                    PostID = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Likes", x => x.PostLikeID);
                    table.ForeignKey(
                        name: "FK_Post_Likes_Posts_PostID",
                        column: x => x.PostID,
                        principalTable: "Posts",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_Likes_Users_LikerID",
                        column: x => x.LikerID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post_Media_Links",
                columns: table => new
                {
                    PostID = table.Column<long>(type: "INTEGER", nullable: false),
                    MediaType = table.Column<int>(type: "INTEGER", nullable: false),
                    MediaURL = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_Post_Media_Links_Posts_PostID",
                        column: x => x.PostID,
                        principalTable: "Posts",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Saved_Posts",
                columns: table => new
                {
                    SaverID = table.Column<long>(type: "INTEGER", nullable: false),
                    PostID = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Saved_Posts", x => new { x.SaverID, x.PostID });
                    table.ForeignKey(
                        name: "FK_Saved_Posts_Posts_PostID",
                        column: x => x.PostID,
                        principalTable: "Posts",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Saved_Posts_Users_SaverID",
                        column: x => x.SaverID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommenterID",
                table: "Comments",
                column: "CommenterID");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostID",
                table: "Comments",
                column: "PostID",
                unique: true);

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
                name: "IX_Post_Media_Links_PostID",
                table: "Post_Media_Links",
                column: "PostID");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_InitiatorID",
                table: "Posts",
                column: "InitiatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Saved_Posts_PostID_SaverID",
                table: "Saved_Posts",
                columns: new[] { "PostID", "SaverID" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Post_Likes");

            migrationBuilder.DropTable(
                name: "Post_Media_Links");

            migrationBuilder.DropTable(
                name: "Saved_Posts");

            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
