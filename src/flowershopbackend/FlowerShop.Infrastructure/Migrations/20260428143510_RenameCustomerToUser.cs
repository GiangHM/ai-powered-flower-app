using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameCustomerToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename Customers table to Users
            migrationBuilder.DropForeignKey(
                name: "FK_EmailVerificationTokens_Customers_CustomerId",
                table: "EmailVerificationTokens");

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_Email",
                table: "Users",
                newName: "IX_Users_Email");

            // Rename the primary key constraint
            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Users");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            // Rename EmailVerificationTokens.CustomerId -> UserId
            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "EmailVerificationTokens",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EmailVerificationTokens_CustomerId",
                table: "EmailVerificationTokens",
                newName: "IX_EmailVerificationTokens_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailVerificationTokens_Users_UserId",
                table: "EmailVerificationTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Rename Orders.CustomerId -> UserId
            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Orders",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailVerificationTokens_Users_UserId",
                table: "EmailVerificationTokens");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Orders",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "EmailVerificationTokens",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_EmailVerificationTokens_UserId",
                table: "EmailVerificationTokens",
                newName: "IX_EmailVerificationTokens_CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailVerificationTokens_Customers_CustomerId",
                table: "EmailVerificationTokens",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Rename the primary key constraint before renaming the table
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Users",
                column: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "Users",
                newName: "IX_Customers_Email");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Customers");
        }
    }
}
