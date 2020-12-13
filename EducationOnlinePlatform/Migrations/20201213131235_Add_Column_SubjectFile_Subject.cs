using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EducationOnlinePlatform.Migrations
{
    public partial class Add_Column_SubjectFile_Subject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId",
                table: "Files",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                columns: new[] { "Id", "SubjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_Files_SubjectId",
                table: "Files",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Subjects_SubjectId",
                table: "Files",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Subjects_SubjectId",
                table: "Files");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_SubjectId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Files");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "Id");
        }
    }
}
