using Microsoft.EntityFrameworkCore.Migrations;

namespace EducationOnlinePlatform.Migrations
{
    public partial class DescriptionAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Subjects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "EducationSets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "EducationSets");
        }
    }
}
