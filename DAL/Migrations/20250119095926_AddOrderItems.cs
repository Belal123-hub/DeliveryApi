using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add a new temporary column for the integer status
            migrationBuilder.AddColumn<int>(
                name: "StatusTemp",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0); // Default to 0 (Pending)

            // Step 2: Convert the existing string values to integers
            migrationBuilder.Sql(@"
                UPDATE ""Orders""
                SET ""StatusTemp"" = 
                    CASE 
                        WHEN ""Status"" = 'Pending' THEN 0
                        WHEN ""Status"" = 'Delivered' THEN 1
                        ELSE 0 -- Default to Pending for any unexpected values
                    END;
            ");

            // Step 3: Drop the old Status column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            // Step 4: Rename the new column to Status
            migrationBuilder.RenameColumn(
                table: "Orders",
                name: "StatusTemp",
                newName: "Status");

            // Add the UserId column
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Alter the Price columns in Dishes and BasketItems
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Dishes",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "BasketItems",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            // Create the OrderItems table
            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    DishId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the OrderItems table
            migrationBuilder.DropTable(
                name: "OrderItems");

            // Drop the UserId column
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Orders");

            // Step 1: Add the old Status column back as a string
            migrationBuilder.AddColumn<string>(
                name: "StatusOld",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "Pending");

            // Step 2: Convert the integer values back to strings
            migrationBuilder.Sql(@"
                UPDATE ""Orders""
                SET ""StatusOld"" = 
                    CASE 
                        WHEN ""Status"" = 0 THEN 'Pending'
                        WHEN ""Status"" = 1 THEN 'Delivered'
                        ELSE 'Pending' -- Default to Pending for any unexpected values
                    END;
            ");

            // Step 3: Drop the new Status column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            // Step 4: Rename the old column back to Status
            migrationBuilder.RenameColumn(
                table: "Orders",
                name: "StatusOld",
                newName: "Status");

            // Revert the Price columns in Dishes and BasketItems
            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Dishes",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "BasketItems",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }
    }
}