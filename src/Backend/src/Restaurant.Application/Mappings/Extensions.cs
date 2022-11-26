using Restaurant.Application.DTO;
using Restaurant.Core.Entities;
using Restaurant.Core.ValueObjects;

namespace Restarant.Application.Mappings
{
    internal static class Extensions
    {
        public static Addition AsEntity(this AdditionDto additionDto)
        {
            return new Addition(additionDto.Id, additionDto.AdditionName, additionDto.Price, additionDto.AdditionKind);
        }

        public static AdditionDto AsDto(this Addition addition)
        {
            return new AdditionDto
            {
                Id = addition.Id,
                AdditionName = addition.AdditionName,
                Price = addition.Price,
                AdditionKind = addition.AdditionKind.ToString()
            };
        }

        public static Product AsEntity(this ProductDto productDto)
        {
            return new Product(productDto.Id, productDto.ProductName, productDto.Price, productDto.ProductKind);
        }

        public static ProductDto AsDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Price = product.Price,
                ProductKind = product.ProductKind.ToString()
            };
        }

        public static ProductDetailsDto AsDetailsDto(this Product product)
        {
            return new ProductDetailsDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Price = product.Price,
                ProductKind = product.ProductKind.ToString(),
                Orders = product.Orders.Select(o => o.AsDto())
            };
        }

        public static Order AsEntity(this OrderDto orderDto)
        {
            return new Order(orderDto.Id, orderDto.OrderNumber, orderDto.Created,
                orderDto.Price, Email.Of(orderDto.Email), orderDto.Note);
        }

        public static OrderDto AsDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                Created = order.Created,
                Email = order.Email.Value,
                Note = order.Note,
                OrderNumber = order.OrderNumber,
                Price = order.Price
            };
        }

        public static OrderDetailsDto AsDetailsDto(this Order order)
        {
            return new OrderDetailsDto
            {
                Id = order.Id,
                Created = order.Created,
                Email = order.Email.Value,
                Note = order.Note,
                OrderNumber = order.OrderNumber,
                Price = order.Price,
                Products = order.Products.Select(p => p.AsDto())
            };
        }

        public static ProductSaleDto AsDto(this ProductSale productSale)
        {
            return new ProductSaleDto
            {
                Id = productSale.Id,
                Addition = productSale.Addition?.AsDto(),
                Email = productSale.Email.Value,
                EndPrice = productSale.EndPrice,
                Order = productSale.Order?.AsDto(),
                Product = productSale.Product.AsDto(),
                ProductSaleState = productSale.ProductSaleState.ToString()
            };
        }

        public static UserDto AsDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email.Value,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
