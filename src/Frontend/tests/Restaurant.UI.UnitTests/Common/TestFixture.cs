using Restaurant.UI.DTO;
using System.Security.Claims;

namespace Restaurant.UI.UnitTests.Common
{
    internal static class TestFixture
    {
        public static IEnumerable<Claim> CreateDefaultUser(string email = "test@test.com", string role = "user")
        {
            var id = Guid.NewGuid();
            var claims = new List<Claim>()
            {
                new Claim("sub", id.ToString()),
                new Claim("unique_name", id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };
            return claims;
        }

        public static IEnumerable<ProductDto> GetProducts()
        {
            return new List<ProductDto>
            {
                new ProductDto { Id = Guid.NewGuid(), ProductName = "Product#1", Price = 100M, ProductKind = "MainDish" },
                new ProductDto { Id = Guid.NewGuid(), ProductName = "Product#2", Price = 10M, ProductKind = "Soup" },
                new ProductDto { Id = Guid.NewGuid(), ProductName = "Product#3", Price = 50M, ProductKind = "Pizza" },
            };
        }
    }
}
