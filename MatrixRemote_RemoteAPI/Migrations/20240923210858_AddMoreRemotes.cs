using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatrixRemote_RemoteAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreRemotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Remotes",
                columns: new[] { "Id", "CreatedDate", "Font", "ImageUrl", "Message", "UpdatedDate" },
                values: new object[] { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "anotherFont", "https://images.pexels.com/photos/123456/pexels-photo-123456.jpeg?auto=compress&cs=tinysrgb&dpr=1&w=500", "another message from space", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Remotes",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
