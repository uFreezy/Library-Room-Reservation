using Microsoft.EntityFrameworkCore.Migrations;

namespace LibRes.App.Migrations
{
    public partial class migration666 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                "AspNetRoles",
                new[] {"Id", "ConcurrencyStamp"},
                new object[] {"449738d0-3844-432a-834b-41489823dd8f", "1ba5cebb-2fc8-4198-9173-19245e631d1b"});

            migrationBuilder.InsertData(
                "AspNetRoles",
                new[] {"Id", "ConcurrencyStamp", "Name", "NormalizedName"},
                new object[]
                    {"21594b89-04f1-40b2-b62e-67b93ff74bc8", "7cbb8d4c-91a4-40fe-812b-e08a70f58ae1", "Admin", "ADMIN"});
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                "AspNetRoles",
                new[] {"Id", "ConcurrencyStamp"},
                new object[] {"21594b89-04f1-40b2-b62e-67b93ff74bc8", "7cbb8d4c-91a4-40fe-812b-e08a70f58ae1"});

            migrationBuilder.InsertData(
                "AspNetRoles",
                new[] {"Id", "ConcurrencyStamp", "Name", "NormalizedName"},
                new object[]
                    {"449738d0-3844-432a-834b-41489823dd8f", "1ba5cebb-2fc8-4198-9173-19245e631d1b", "Admin", "ADMIN"});
        }
    }
}