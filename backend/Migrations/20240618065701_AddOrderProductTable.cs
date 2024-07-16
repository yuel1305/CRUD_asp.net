using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_product_Productid",
                table: "order");

            migrationBuilder.DropIndex(
                name: "IX_order_Productid",
                table: "order");

            migrationBuilder.DropColumn(
                name: "Productid",
                table: "order");


            migrationBuilder.CreateTable(
                name: "order_product",
                columns: table => new
                {
                    orderId = table.Column<int>(type: "integer", nullable: false),
                    productId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_product", x => new { x.orderId, x.productId });
                    table.ForeignKey(
                        name: "FK_order_product_order_orderId",
                        column: x => x.orderId,
                        principalTable: "order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_product_product_productId",
                        column: x => x.productId,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_product_productId",
                table: "order_product",
                column: "productId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_product");

            

            migrationBuilder.AddColumn<int>(
                name: "Productid",
                table: "order",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_order_Productid",
                table: "order",
                column: "Productid");

            migrationBuilder.AddForeignKey(
                name: "FK_order_product_Productid",
                table: "order",
                column: "Productid",
                principalTable: "product",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
