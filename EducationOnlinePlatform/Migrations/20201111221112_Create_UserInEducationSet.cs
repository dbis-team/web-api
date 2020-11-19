using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EducationOnlinePlatform.Migrations
{
    public partial class Create_UserInEducationSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserInEducationSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    UserId = table.Column<Guid>(nullable: false),
                    EducationSetId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInEducationSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInEducationSet_EducationSets_EducationSetId",
                        column: x => x.EducationSetId,
                        principalTable: "EducationSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserInEducationSet_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserInEducationSet_EducationSetId",
                table: "UserInEducationSet",
                column: "EducationSetId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInEducationSet_UserId",
                table: "UserInEducationSet",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserInEducationSet");
        }
    }
}
