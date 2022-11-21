using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DDApp.API.Migrations
{
    /// <inheritdoc />
    public partial class DirectImagesRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DirectImage_Attaches_Id",
                table: "DirectImage");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectImage_Directs_DirectId",
                table: "DirectImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DirectImage",
                table: "DirectImage");

            migrationBuilder.RenameTable(
                name: "DirectImage",
                newName: "DirectImages");

            migrationBuilder.RenameIndex(
                name: "IX_DirectImage_DirectId",
                table: "DirectImages",
                newName: "IX_DirectImages_DirectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DirectImages",
                table: "DirectImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DirectImages_Attaches_Id",
                table: "DirectImages",
                column: "Id",
                principalTable: "Attaches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectImages_Directs_DirectId",
                table: "DirectImages",
                column: "DirectId",
                principalTable: "Directs",
                principalColumn: "DirectId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DirectImages_Attaches_Id",
                table: "DirectImages");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectImages_Directs_DirectId",
                table: "DirectImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DirectImages",
                table: "DirectImages");

            migrationBuilder.RenameTable(
                name: "DirectImages",
                newName: "DirectImage");

            migrationBuilder.RenameIndex(
                name: "IX_DirectImages_DirectId",
                table: "DirectImage",
                newName: "IX_DirectImage_DirectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DirectImage",
                table: "DirectImage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DirectImage_Attaches_Id",
                table: "DirectImage",
                column: "Id",
                principalTable: "Attaches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectImage_Directs_DirectId",
                table: "DirectImage",
                column: "DirectId",
                principalTable: "Directs",
                principalColumn: "DirectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
