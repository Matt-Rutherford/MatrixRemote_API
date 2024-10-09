using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MatrixRemote_RemoteAPI.Migrations
{
    /// <inheritdoc />
    public partial class RolesSeeded3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "9db86186-0f42-4e1a-aacb-e07ed16b8e41", "2", "User", "User" },
                    { "a16df5f1-0eea-44e5-aa32-ec7b854099e6", "1", "Admin", "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9db86186-0f42-4e1a-aacb-e07ed16b8e41");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a16df5f1-0eea-44e5-aa32-ec7b854099e6");
        }
    }
}
