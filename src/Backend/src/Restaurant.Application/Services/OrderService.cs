using Restarant.Application.Mappings;
using Restaurant.Application.Abstractions;
using Restaurant.Application.DTO;
using Restaurant.Application.Exceptions;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;

namespace Restaurant.Application.Services
{
    internal sealed class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductSaleRepository _productSaleRepository;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IOrderRepository orderRepository, IProductSaleRepository productSaleRepository, IClock clock,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _productSaleRepository = productSaleRepository;
            _clock = clock;
            _unitOfWork = unitOfWork;
        }

        public async Task AddAsync(AddOrderDto addOrderDto)
        {
            var productSales = new List<ProductSale>();

            if (addOrderDto.ProductSaleIds is not null)
            {
                foreach (var productSaleId in addOrderDto.ProductSaleIds)
                {
                    var productSale = await _productSaleRepository.GetAsync(productSaleId);

                    if (productSale is null)
                    {
                        throw new ProductSaleNotFoundException(productSaleId);
                    }

                    productSales.Add(productSale);
                }
            }

            var order = new Order(Guid.NewGuid(), Guid.NewGuid().ToString(), _clock.CurrentDate(),
                productSales.Sum(p => p.EndPrice), Email.Of(addOrderDto.Email), addOrderDto.Note, productSales);
            addOrderDto.Id = order.Id;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _orderRepository.AddAsync(order);

                foreach (var product in productSales)
                {
                    await _productSaleRepository.UpdateAsync(product);
                }

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var order = await _orderRepository.GetAsync(id);

            if (order is null)
            {
                throw new OrderNotFoundException(id);
            }

            foreach (var product in order.Products)
            {
                product.RemoveOrder();
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _orderRepository.DeleteAsync(order);

                foreach (var product in order.Products)
                {
                    await _productSaleRepository.UpdateAsync(product);
                }

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteWithPositionsAsync(Guid id)
        {
            var order = await _orderRepository.GetAsync(id);

            if (order is null)
            {
                throw new OrderNotFoundException(id);
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _productSaleRepository.DeleteByOrderAsync(id);
                await _orderRepository.DeleteAsync(order);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            return (await _orderRepository.GetAllAsync()).Select(o => o.AsDto());
        }

        public async Task<OrderDetailsDto?> GetAsync(Guid id)
        {
            var order = await _orderRepository.GetAsync(id);
            return order?.AsDetailsDto();
        }

        public async Task UpdateAsync(AddOrderDto addOrderDto)
        {
            var order = await _orderRepository.GetAsync(addOrderDto.Id);

            if (order is null)
            {
                throw new OrderNotFoundException(addOrderDto.Id);
            }

            order.ChangeEmail(Email.Of(addOrderDto.Email));
            order.ChangeNote(addOrderDto.Note);

            var productsToAdd = new List<ProductSale>();
            var productsToDelete = new List<ProductSale>();
            if (addOrderDto.ProductSaleIds is not null)
            {
                foreach (var productSaleId in addOrderDto.ProductSaleIds)
                {
                    var productSaleExists = order.Products.SingleOrDefault(p => p.Id == productSaleId);

                    if (productSaleExists is not null)
                    {
                        continue;
                    }

                    var productSale = await _productSaleRepository.GetAsync(productSaleId);

                    if (productSale is null)
                    {
                        throw new ProductSaleNotFoundException(productSaleId);
                    }

                    order.AddProduct(productSale);
                    productsToAdd.Add(productSale);
                }

                var products = new List<ProductSale>(order.Products);
                foreach (var productSale in products)
                {
                    var productSaleIdExists = addOrderDto.ProductSaleIds.Any(p => p == productSale.Id);

                    if (productSaleIdExists)
                    {
                        continue;
                    }

                    order.RemoveProduct(productSale);
                    productsToDelete.Add(productSale);
                }
            }

            order.ChangePrice(order.Products.Sum(o => o.EndPrice));

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _orderRepository.UpdateAsync(order);

                foreach (var product in productsToAdd)
                {
                    await _productSaleRepository.UpdateAsync(product);
                }
                
                foreach (var product in productsToDelete)
                {
                    await _productSaleRepository.DeleteAsync(product);
                }

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
