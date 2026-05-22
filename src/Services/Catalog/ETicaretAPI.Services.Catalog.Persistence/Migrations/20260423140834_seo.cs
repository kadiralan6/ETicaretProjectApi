using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETicaretAPI.Services.Catalog.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class seo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_products_category_id",
                table: "products");

            migrationBuilder.AddColumn<string>(
                name: "short_description",
                table: "products",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "alt_text",
                table: "product_images",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_category_id_is_active",
                table: "products",
                columns: new[] { "category_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "IX_products_is_featured_is_active",
                table: "products",
                columns: new[] { "is_featured", "is_active" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_products_category_id_is_active",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_is_featured_is_active",
                table: "products");

            migrationBuilder.DropColumn(
                name: "short_description",
                table: "products");

            migrationBuilder.DropColumn(
                name: "alt_text",
                table: "product_images");

            migrationBuilder.CreateIndex(
                name: "IX_products_category_id",
                table: "products",
                column: "category_id");
        }
    }
}
