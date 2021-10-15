using Microsoft.EntityFrameworkCore.Migrations;

namespace BELT.Migrations
{
    public partial class FirstMigratio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DurationType",
                table: "Shindigs",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationType",
                table: "Shindigs");
        }
    }
}
