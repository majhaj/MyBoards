using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBoardsmyVersion.Migrations
{
    /// <inheritdoc />
    public partial class AdditionWorkItemStateSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(table: "workItemStates",
                column: "State",
                value: "On Hold");

            migrationBuilder.InsertData(table: "workItemStates",
                column: "State",
                value: "Rejected");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "workItemStates",
                keyColumn: "State",
                keyValue: "On Hold");

            migrationBuilder.DeleteData(
                table: "workItemStates",
                keyColumn: "State",
                keyValue: "Rejected");
        }
    }
}
