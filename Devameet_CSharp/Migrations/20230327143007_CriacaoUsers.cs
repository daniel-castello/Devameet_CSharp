using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevameetCSharp.Migrations
{
    public partial class CriacaoUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Rooms_MeetId",
                table: "Rooms",
                column: "MeetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Meets_MeetId",
                table: "Rooms",
                column: "MeetId",
                principalTable: "Meets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Meets_MeetId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_MeetId",
                table: "Rooms");
        }
    }
}
