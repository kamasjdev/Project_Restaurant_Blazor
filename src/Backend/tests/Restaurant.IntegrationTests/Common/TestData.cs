using Restaurant.Core.Entities;
using Restaurant.Core.ValueObjects;

namespace Restaurant.IntegrationTests.Common
{
    internal static class TestData
    {
        /// <summary>
        /// Returns admin user without hashed password
        /// </summary>
        /// <returns><see cref="User"/></returns>
        public static User GetAdminUser()
        {
            var id = new Guid("00000000-0000-0000-0000-000000000001");
            var user = new User(id, Email.Of("admin@admin.com"), "PasW0Rd1!1241abc", User.Roles.AdminRole, DateTime.UtcNow);
            return user;
        }
    }
}
