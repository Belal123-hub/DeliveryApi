using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class GetOldEntitiesWithoutRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketItems_Dishes_DishId",
                table: "BasketItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_AspNetUsers_UserId",
                table: "Baskets");

            migrationBuilder.DropForeignKey(
                name: "FK_DishRatings_AspNetUsers_UserId",
                table: "DishRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_DishRatings_Dishes_DishId",
                table: "DishRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Dishes_DishId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_UserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_DishId",
                table: "OrderItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DishRatings",
                table: "DishRatings");

            migrationBuilder.DropIndex(
                name: "IX_DishRatings_DishId",
                table: "DishRatings");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_UserId",
                table: "Baskets");

            migrationBuilder.DropIndex(
                name: "IX_BasketItems_DishId",
                table: "BasketItems");

            migrationBuilder.DropColumn(
                name: "RatingScore",
                table: "DishRatings");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "DishRatings",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DishRatings",
                table: "DishRatings",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DishRatings",
                table: "DishRatings");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "DishRatings");

            migrationBuilder.AddColumn<int>(
                name: "RatingScore",
                table: "DishRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DishRatings",
                table: "DishRatings",
                columns: new[] { "UserId", "DishId" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_DishId",
                table: "OrderItems",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_DishRatings_DishId",
                table: "DishRatings",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_UserId",
                table: "Baskets",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_DishId",
                table: "BasketItems",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketItems_Dishes_DishId",
                table: "BasketItems",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_AspNetUsers_UserId",
                table: "Baskets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DishRatings_AspNetUsers_UserId",
                table: "DishRatings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DishRatings_Dishes_DishId",
                table: "DishRatings",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Dishes_DishId",
                table: "OrderItems",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
