using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MatrixRemote_RemoteAPI.Migrations
{
    /// <inheritdoc />
    public partial class RolesSeeded5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                    { "cd53d24d-3e94-4823-a7e8-944bef0f50f2", "1", "Admin", "ADMIN" },
                    { "ea068fd4-1ae4-4cbb-ad1a-4bd2702ff677", "2", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd53d24d-3e94-4823-a7e8-944bef0f50f2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ea068fd4-1ae4-4cbb-ad1a-4bd2702ff677");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "34f98dbf-069b-4590-9164-675170b8247f", "1", "Admin", "ADMIN" },
                    { "4d48f062-2c13-48cd-aa98-46ce8452d920", "2", "User", "USER" }
                });
        }
    }
}
