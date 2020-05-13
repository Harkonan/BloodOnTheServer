using Microsoft.EntityFrameworkCore.Migrations;

namespace BloodOnTheWeb.Migrations
{
    public partial class SeatstoSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Seats",
                table: "Sessions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Seats",
                table: "Sessions");
        }
    }
}
