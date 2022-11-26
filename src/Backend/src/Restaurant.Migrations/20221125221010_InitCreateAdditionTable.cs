using FluentMigrator;

namespace Restaurant.Migrations
{
    [Migration(20221125221010)]
    public sealed class InitCreateAdditionTable : Migration
    {
        public override void Down()
        {
            Delete.Table("additions");
        }

        public override void Up()
        {
            Create.Table("additions")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("AdditionName").AsString(150).NotNullable().Indexed("idx_additions_addition_name")
                .WithColumn("Price").AsDecimal().NotNullable()
                .WithColumn("AdditionKind").AsString(25);
        }
    }
}
