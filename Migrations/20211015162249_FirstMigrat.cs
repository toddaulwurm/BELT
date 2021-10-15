using Microsoft.EntityFrameworkCore.Migrations;

namespace BELT.Migrations
{
    public partial class FirstMigrat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "Shindigs",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Duration",
                table: "Shindigs",
                type: "double",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
