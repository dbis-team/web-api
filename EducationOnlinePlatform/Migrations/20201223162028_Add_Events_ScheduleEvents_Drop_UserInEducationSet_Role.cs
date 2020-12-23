using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EducationOnlinePlatform.Migrations
{
    public partial class Add_Events_ScheduleEvents_Drop_UserInEducationSet_Role : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserRole",
                table: "UserInEducationSet");

            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "ScheduleEvents");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTimeFrom",
                table: "ScheduleEvents",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTimeTo",
                table: "ScheduleEvents",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId",
                table: "ScheduleEvents",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEvents_SubjectId",
                table: "ScheduleEvents",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEvents_Subjects_SubjectId",
                table: "ScheduleEvents",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEvents_Subjects_SubjectId",
                table: "ScheduleEvents");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEvents_SubjectId",
                table: "ScheduleEvents");

            migrationBuilder.DropColumn(
                name: "DateTimeFrom",
                table: "ScheduleEvents");

            migrationBuilder.DropColumn(
                name: "DateTimeTo",
                table: "ScheduleEvents");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "ScheduleEvents");

            migrationBuilder.AddColumn<string>(
                name: "UserRole",
                table: "UserInEducationSet",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "ScheduleEvents",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
