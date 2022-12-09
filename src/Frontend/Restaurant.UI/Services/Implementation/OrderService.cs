using Restaurant.UI.DTO;
using Restaurant.UI.Services.Abstractions;
using System.Text;

namespace Restaurant.UI.Services.Implementation
{
    internal sealed class OrderService : IOrderService
    {
        private readonly List<OrderDto> _orders = new();
        private readonly IProductSaleService _productSaleService;

        public OrderService(IProductSaleService productSaleService)
        {
            _productSaleService = productSaleService;
        }

        public async Task AddAsync(AddOrderDto addOrderDto)
        {
            addOrderDto.Id = Guid.NewGuid();
            var productSales = new List<ProductSaleDto>();

            if (addOrderDto.ProductSaleIds is not null)
            {
                var errors = new StringBuilder("");

                foreach (var productSaleId in addOrderDto.ProductSaleIds)
                {
                    var productSale = await _productSaleService.GetAsync(productSaleId);

                    if (productSale is null)
                    {
                        errors.Append($"ProductSale with id '{productSaleId}' doesnt exists. ");
                        continue;
                    }

                    productSales.Add(productSale);
                }

                if (errors.Length > 0)
                {
                    throw new InvalidOperationException(errors.ToString());
                }
            }

            var order = new OrderDetailsDto
            {
                Id = addOrderDto.Id,
                Email = addOrderDto.Email,
                Created = DateTime.UtcNow,
                OrderNumber = Guid.NewGuid().ToString("N"),
                Note = addOrderDto.Note,
                Price = productSales.Sum(ps => ps.EndPrice),
                Products = productSales
            };
            _orders.Add(order);
            productSales.ForEach(ps => ps.Order = order);
        }

        public Task DeleteAsync(Guid id)
        {
            var orderExists = _orders.SingleOrDefault(o => o.Id == id);

            if (orderExists is null)
            {
                throw new InvalidOperationException($"Order with id: '{id}' doesnt exits");
            }

            _orders.Remove(orderExists);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            await Task.CompletedTask;
            return _orders;
        }

        public Task<OrderDetailsDto?> GetAsync(Guid id)
        {
            return Task.FromResult((OrderDetailsDto?)_orders.SingleOrDefault(o => o.Id == id));
        }

        public Task UpdateAsync(AddOrderDto addOrderDto)
        {
            return Task.CompletedTask;
        }
    }
}
