using Microsoft.EntityFrameworkCore.Migrations;

namespace LibRes.App.Migrations
{
    public partial class migration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumns: new[] { "Id", "ConcurrencyStamp" },
                keyValues: new object[] { "8fc505ae-2ecb-4dde-b76b-74e49e6ae405", "79bde5b7-010b-49f9-be7e-56c6db1de395" });

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "557b2711-0d33-4059-a56c-b5bdb10b70d4", "2e0cf1dc-286d-491d-b4c0-9299c3b46564", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumns: new[] { "Id", "ConcurrencyStamp" },
                keyValues: new object[] { "557b2711-0d33-4059-a56c-b5bdb10b70d4", "2e0cf1dc-286d-491d-b4c0-9299c3b46564" });

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8fc505ae-2ecb-4dde-b76b-74e49e6ae405", "79bde5b7-010b-49f9-be7e-56c6db1de395", "Admin", "ADMIN" });
        }
    }
}
