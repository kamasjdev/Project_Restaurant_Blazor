using FluentMigrator;

namespace Restaurant.Migrations
{
    [Migration(20221127184530)]
    public class InitCreateTableUsers : Migration
    {
        public override void Down()
        {
            Delete.Table("users");
        }

        public override void Up()
        {
            Create.Table("users")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Email").AsString(5000).NotNullable().Unique("uidx_users_email")
                .WithColumn("Password").AsString(1000).NotNullable()
                .WithColumn("Role").AsString(50).NotNullable().Indexed("idx_users_role")
                .WithColumn("CreatedAt").AsDateTime().NotNullable().Indexed("idx_users_created_at");
        }
    }
}
