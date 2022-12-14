using Restaurant.Shared.OrderProto;
using Restaurant.UI.DTO;
using Restaurant.UI.Services.Abstractions;

namespace Restaurant.UI.Services.Implementation
{
    internal sealed class OrderService : IOrderService
    {
        private readonly Orders.OrdersClient _ordersClient;

        public OrderService(Orders.OrdersClient ordersClient)
        {
            _ordersClient = ordersClient;
        }

        public async Task<Guid> AddAsync(AddOrderDto addOrderDto)
        {
            var request = new AddOrderRequest
            {
                Id = addOrderDto.Id.ToString(),
                Email = addOrderDto.Email,
            };
            if (!string.IsNullOrWhiteSpace(addOrderDto.Note))
            {
                request.Note = addOrderDto.Note;
            }
            request.ProductSaleIds.AddRange(addOrderDto.ProductSaleIds?.Select(ps =>
                    new ProductSaleId { Id = ps.ToString() }));
            var response = await _ordersClient.AddOrderAsync(request);
            return Guid.Parse(response.Id);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _ordersClient.DeleteOrderWithPositionsAsync(new DeleteOrderWithPositionsRequest
            {
                Id = id.ToString()
            });
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            return (await _ordersClient.GetOrdersAsync(new Google.Protobuf.WellKnownTypes.Empty()))
                .Orders.Select(o => new OrderDto
                {
                    Id = Guid.Parse(o.Id),
                    OrderNumber = o.OrderNumber,
                    Price = decimal.Parse(o.Price),
                    Email = o.Email,
                    Note = o.Note,
                    Created = o.Created.ToDateTime(),
                });
        }

        public async Task<IEnumerable<OrderDto>> GetAllByEmailAsync(string email)
        {
            return (await _ordersClient.GetOrdersAsync(new Google.Protobuf.WellKnownTypes.Empty()))
                .Orders.Select(o => new OrderDto
                {
                    Id = Guid.Parse(o.Id),
                    OrderNumber = o.OrderNumber,
                    Price = decimal.Parse(o.Price),
                    Email = o.Email,
                    Note = o.Note,
                    Created = o.Created.ToDateTime(),
                });
        }

        public async Task<OrderDetailsDto?> GetAsync(Guid id)
        {
            var response = await _ordersClient.GetOrderAsync(new GetOrderRequest
            {
                Id = id.ToString()
            });
            return response is not null ?
                new OrderDetailsDto
                {
                    Id = Guid.Parse(response.Id),
                    Email = response.Email,
                    Note = response.Note,
                    OrderNumber = response.OrderNumber,
                    Created = response.Created.ToDateTime(),
                    Price = decimal.Parse(response.Price),
                    Products = response.Products.Select(p => new ProductSaleDto
                    {
                        Id = Guid.Parse(p.Id),
                        Email = p.Email,
                        ProductSaleState = p.ProductSaleState,
                        EndPrice = decimal.Parse(p.EndPrice),
                        Addition = p.Addition is not null ?
                                new AdditionDto
                                {
                                    Id = Guid.Parse(p.Addition.Id),
                                    AdditionKind = p.Addition.AdditionKind,
                                    AdditionName = p.Addition.AdditionName,
                                    Price = decimal.Parse(p.Addition.Price)
                                } : null,
                        Product = new ProductDto
                        {
                            Id = Guid.Parse(p.Product.Id),
                            Price = decimal.Parse(p.Product.Price),
                            ProductKind = p.Product.ProductKind,
                            ProductName = p.Product.ProductName
                        }
                    })
                } : null;
        }

        public async Task UpdateAsync(AddOrderDto addOrderDto)
        {
            var request = new AddOrderRequest
            {
                Id = addOrderDto.Id.ToString(),
                Email = addOrderDto.Email
            };
            if (!string.IsNullOrWhiteSpace(addOrderDto.Note))
            {
                request.Note = addOrderDto.Note;
            }
            request.ProductSaleIds.AddRange(addOrderDto.ProductSaleIds?.Select(ps => new ProductSaleId { Id = ps.ToString() }));
            await _ordersClient.UpdateOrderAsync(request);
        }
    }
}
