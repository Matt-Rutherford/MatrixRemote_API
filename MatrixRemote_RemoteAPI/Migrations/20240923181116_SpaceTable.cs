using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatrixRemote_RemoteAPI.Migrations
{
    /// <inheritdoc />
    public partial class SpaceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MessageDTOs",
                columns: new[] { "Id", "Font", "ImageUrl", "Message" },
                values: new object[] { 1, "font", "https://images.pexels.com/photos/998641/pexels-photo-998641.jpeg?auto=compress&cs=tinysrgb&dpr=1&w=500", "message from space" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MessageDTOs",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
