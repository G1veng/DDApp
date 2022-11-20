using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DDApp.API.Migrations
{
    /// <inheritdoc />
    public partial class DirectFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DirectMessages_DirectId",
                table: "DirectMessages");

            migrationBuilder.DropIndex(
                name: "IX_DirectMembers_DirectId",
                table: "DirectMembers");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessages_DirectId",
                table: "DirectMessages",
                column: "DirectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DirectMessages_DirectId",
                table: "DirectMessages");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessages_DirectId",
                table: "DirectMessages",
                column: "DirectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DirectMembers_DirectId",
                table: "DirectMembers",
                column: "DirectId",
                unique: true);
        }
    }
}
