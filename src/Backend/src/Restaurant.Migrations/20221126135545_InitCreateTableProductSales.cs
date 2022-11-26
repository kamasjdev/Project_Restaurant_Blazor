using FluentMigrator;

namespace Restaurant.Migrations
{
    [Migration(20221126135545)]
    public class InitCreateTableProductSales : Migration
    {
        public override void Down()
        {
            Delete.Table("product_sales");
        }

        public override void Up()
        {
            Create.Table("product_sales")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("OrderId").AsGuid().ForeignKey("orders", "Id").Nullable().Indexed("idx_product_sales_order_id")
                .WithColumn("ProductId").AsGuid().ForeignKey("products", "Id").Indexed("idx_product_sales_product_id")
                .WithColumn("AdditionId").AsGuid().ForeignKey("additions", "Id").Nullable().Indexed("idx_product_sales_addition_id")
                .WithColumn("Email").AsString(300).Indexed("idx_product_sales_email")
                .WithColumn("EndPrice").AsDecimal()
                .WithColumn("ProductSaleState").AsString(25).NotNullable();
        }
    }
}
