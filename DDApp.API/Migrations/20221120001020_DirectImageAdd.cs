using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DDApp.API.Migrations
{
    /// <inheritdoc />
    public partial class DirectImageAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DirectImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DirectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectImage_Attaches_Id",
                        column: x => x.Id,
                        principalTable: "Attaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectImage_Directs_DirectId",
                        column: x => x.DirectId,
                        principalTable: "Directs",
                        principalColumn: "DirectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DirectImage_DirectId",
                table: "DirectImage",
                column: "DirectId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DirectImage");
        }
    }
}
