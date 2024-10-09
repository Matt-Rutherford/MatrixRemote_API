using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MatrixRemote_RemoteAPI.Migrations
{
    /// <inheritdoc />
    public partial class RolesSeeded4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9db86186-0f42-4e1a-aacb-e07ed16b8e41");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a16df5f1-0eea-44e5-aa32-ec7b854099e6");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "34f98dbf-069b-4590-9164-675170b8247f", "1", "Admin", "ADMIN" },
                    { "4d48f062-2c13-48cd-aa98-46ce8452d920", "2", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "34f98dbf-069b-4590-9164-675170b8247f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4d48f062-2c13-48cd-aa98-46ce8452d920");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "9db86186-0f42-4e1a-aacb-e07ed16b8e41", "2", "User", "USER" },
                    { "a16df5f1-0eea-44e5-aa32-ec7b854099e6", "1", "Admin", "ADMIN" }
                });
        }
    }
}
