using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DDApp.API.Migrations
{
    /// <inheritdoc />
    public partial class Direct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Directs",
                columns: table => new
                {
                    DirectId = table.Column<Guid>(type: "uuid", nullable: false),
                    DirectTitle = table.Column<string>(type: "text", nullable: false),
                    IsDirectGroup = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directs", x => x.DirectId);
                });

            migrationBuilder.CreateTable(
                name: "DirectMembers",
                columns: table => new
                {
                    DirectId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMembers", x => new { x.DirectId, x.UserId });
                    table.ForeignKey(
                        name: "FK_DirectMembers_Directs_DirectId",
                        column: x => x.DirectId,
                        principalTable: "Directs",
                        principalColumn: "DirectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DirectMessages",
                columns: table => new
                {
                    DirectMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    DirectId = table.Column<Guid>(type: "uuid", nullable: false),
                    DirectMessage = table.Column<string>(type: "text", nullable: true),
                    Sended = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessages", x => x.DirectMessageId);
                    table.ForeignKey(
                        name: "FK_DirectMessages_Directs_DirectId",
                        column: x => x.DirectId,
                        principalTable: "Directs",
                        principalColumn: "DirectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectMessages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DirectFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DirectMessagesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectFiles_Attaches_Id",
                        column: x => x.Id,
                        principalTable: "Attaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectFiles_DirectMessages_DirectMessagesId",
                        column: x => x.DirectMessagesId,
                        principalTable: "DirectMessages",
                        principalColumn: "DirectMessageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DirectFiles_DirectMessagesId",
                table: "DirectFiles",
                column: "DirectMessagesId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMembers_UserId",
                table: "DirectMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessages_DirectId",
                table: "DirectMessages",
                column: "DirectId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessages_SenderId",
                table: "DirectMessages",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DirectFiles");

            migrationBuilder.DropTable(
                name: "DirectMembers");

            migrationBuilder.DropTable(
                name: "DirectMessages");

            migrationBuilder.DropTable(
                name: "Directs");
        }
    }
}
