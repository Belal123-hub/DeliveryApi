using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingToDish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DishRatings",
                table: "DishRatings");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DishRatings");

            migrationBuilder.DropColumn(
                name: "RatedAt",
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
                name: "IX_DishRatings_DishId",
                table: "DishRatings",
                column: "DishId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishRatings_AspNetUsers_UserId",
                table: "DishRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_DishRatings_Dishes_DishId",
                table: "DishRatings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DishRatings",
                table: "DishRatings");

            migrationBuilder.DropIndex(
                name: "IX_DishRatings_DishId",
                table: "DishRatings");

            migrationBuilder.DropColumn(
                name: "RatingScore",
                table: "DishRatings");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "DishRatings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "RatedAt",
                table: "DishRatings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
    }
}
