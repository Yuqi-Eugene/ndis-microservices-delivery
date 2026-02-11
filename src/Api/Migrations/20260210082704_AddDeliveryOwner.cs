using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerEmail",
                table: "ServiceDeliveries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerUserId",
                table: "ServiceDeliveries",
                type: "character varying(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDeliveries_OwnerUserId",
                table: "ServiceDeliveries",
                column: "OwnerUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceDeliveries_OwnerUserId",
                table: "ServiceDeliveries");

            migrationBuilder.DropColumn(
                name: "OwnerEmail",
                table: "ServiceDeliveries");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "ServiceDeliveries");
        }
    }
}
