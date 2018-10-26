using Microsoft.EntityFrameworkCore.Migrations;

namespace LibRes.App.Migrations
{
    public partial class migration4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumns: new[] { "Id", "ConcurrencyStamp" },
                keyValues: new object[] { "557b2711-0d33-4059-a56c-b5bdb10b70d4", "2e0cf1dc-286d-491d-b4c0-9299c3b46564" });

            migrationBuilder.AddColumn<string>(
                name: "SecretAnswer",
                table: "AspNetUsers",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecretQuestion",
                table: "AspNetUsers",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "69abbd60-a665-43d3-809b-d0eab277a81d", "0d93df2f-ffd5-463c-90c8-a420a68c59c4", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumns: new[] { "Id", "ConcurrencyStamp" },
                keyValues: new object[] { "69abbd60-a665-43d3-809b-d0eab277a81d", "0d93df2f-ffd5-463c-90c8-a420a68c59c4" });

            migrationBuilder.DropColumn(
                name: "SecretAnswer",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SecretQuestion",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "557b2711-0d33-4059-a56c-b5bdb10b70d4", "2e0cf1dc-286d-491d-b4c0-9299c3b46564", "Admin", "ADMIN" });
        }
    }
}
