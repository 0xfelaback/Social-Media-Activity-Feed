using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social_Media_Activity_Feed.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedKeytoNotificationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.AddColumn<long>(
                name: "NotificationID",
                table: "Notifications",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "NotificationID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ReceivingUserID_InitaiatorID",
                table: "Notifications",
                columns: new[] { "ReceivingUserID", "InitaiatorID" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ReceivingUserID_InitaiatorID",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NotificationID",
                table: "Notifications");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                columns: new[] { "ReceivingUserID", "InitaiatorID" });
        }
    }
}
