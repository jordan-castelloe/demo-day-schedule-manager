using Microsoft.EntityFrameworkCore.Migrations;

namespace DemoDay.Data.Migrations
{
    public partial class addlocationanddegreetomodels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "canRelocate",
                table: "Student",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "hasBachelorsDegree",
                table: "Student",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isLocal",
                table: "Company",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "requiresBachelorsDegree",
                table: "Company",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "canRelocate",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "hasBachelorsDegree",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "isLocal",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "requiresBachelorsDegree",
                table: "Company");
        }
    }
}
