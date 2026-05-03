using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderDeliveryInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryEmail",
                table: "Orders",
                type: "nvarchar(320)",
                maxLength: 320,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryName",
                table: "Orders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryPhone",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryEmail",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryPhone",
                table: "Orders");
        }
    }
}
