using FluentMigrator;

namespace Restaurant.Migrations
{
    [Migration(20221126135010)]
    public class InitCreateTableProducts : Migration
    {
        public override void Down()
        {
            Delete.Table("products");
        }

        public override void Up()
        {
            Create.Table("products")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("ProductName").AsString(150).NotNullable().Indexed("idx_products_product_name")
                .WithColumn("Price").AsDecimal().NotNullable()
                .WithColumn("ProductKind").AsString(25).NotNullable();
        }
    }
}
