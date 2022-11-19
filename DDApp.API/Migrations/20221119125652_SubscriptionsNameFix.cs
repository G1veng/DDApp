using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DDApp.API.Migrations
{
    /// <inheritdoc />
    public partial class SubscriptionsNameFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_SubscribedOnId",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "SubscribedOnId",
                table: "Subscriptions",
                newName: "SubscriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_SubscribedOnId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_SubscriptionId",
                table: "Subscriptions",
                column: "SubscriptionId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_SubscriptionId",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "SubscriptionId",
                table: "Subscriptions",
                newName: "SubscribedOnId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_SubscriptionId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_SubscribedOnId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_SubscribedOnId",
                table: "Subscriptions",
                column: "SubscribedOnId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
