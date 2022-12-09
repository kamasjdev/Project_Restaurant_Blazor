using Restarant.Application.Mappings;
using Restaurant.Application.Abstractions;
using Restaurant.Application.DTO;
using Restaurant.Application.Exceptions;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;

namespace Restaurant.Application.Services
{
    internal sealed class ProductSaleService : IProductSaleService
    {
        private readonly IProductSaleRepository _productSaleRepository;
        private readonly IProductRepository _productRepository;
        private readonly IAdditonRepository _additionRepository;
        private readonly IOrderRepository _orderRepository;

        public ProductSaleService(IProductSaleRepository productSaleRepository, IProductRepository productRepository, IAdditonRepository additonRepository,
            IOrderRepository orderRepository)
        {
            _productSaleRepository = productSaleRepository;
            _productRepository = productRepository;
            _additionRepository = additonRepository;
            _orderRepository = orderRepository;
        }

        public async Task AddAsync(AddProductSaleDto productSaleDto)
        {
            var product = await _productRepository.GetAsync(productSaleDto.ProductId);

            if (product is null)
            {
                throw new ProductNotFoundException(productSaleDto.ProductId);
            }

            Addition addition = null;
            Order order = null;

            if (productSaleDto.AdditionId.HasValue)
            {
                addition = await _additionRepository.GetAsync(productSaleDto.AdditionId.Value);

                if (addition is null)
                {
                    throw new AdditionNotFoundException(productSaleDto.AdditionId.Value);
                }
            }

            if (productSaleDto.OrderId.HasValue)
            {
                order = await _orderRepository.GetAsync(productSaleDto.OrderId.Value);

                if (order is null)
                {
                    throw new OrderNotFoundException(productSaleDto.OrderId.Value);
                }
            }

            var productSale = new ProductSale(Guid.NewGuid(), product, ProductSaleState.New, Email.Of(productSaleDto.Email),
                addition, order);
            await _productSaleRepository.AddAsync(productSale);
            productSaleDto.Id = productSale.Id;
        }

        public async Task DeleteAsync(Guid id)
        {
            var productSale = await _productSaleRepository.GetAsync(id);

            if (productSale is null)
            {
                throw new ProductSaleNotFoundException(id);
            }

            await _productSaleRepository.DeleteAsync(productSale);
        }

        public async Task<IEnumerable<ProductSaleDto>> GetAllInCartByEmailAsync(string email)
        {
            return (await _productSaleRepository.GetAllInCartByEmailAsync(email)).Select(o => o.AsDto());
        }

        public async Task<IEnumerable<ProductSaleDto>> GetAllByOrderIdAsync(Guid orderId)
        {
            return (await _productSaleRepository.GetAllByOrderIdAsync(orderId)).Select(o => o.AsDto());
        }

        public async Task<ProductSaleDto?> GetAsync(Guid productSaleId)
        {
            return (await _productSaleRepository.GetAsync(productSaleId))?.AsDto();
        }

        public async Task UpdateAsync(AddProductSaleDto productSaleDto)
        {
            var productSale = await _productSaleRepository.GetAsync(productSaleDto.Id);

            if (productSale is null)
            {
                throw new ProductSaleNotFoundException(productSaleDto.Id);
            }

            if (productSale.ProductSaleState == ProductSaleState.Ordered)
            {
                throw new CannotUpdateProductSaleException(productSale.Id, productSale.ProductSaleState.ToString());
            }

            var product = await _productRepository.GetAsync(productSaleDto.ProductId);

            if (product is null)
            {
                throw new ProductNotFoundException(productSaleDto.ProductId);
            }

            productSale.ChangeEmail(Email.Of(productSaleDto.Email));
            productSale.ChangeProduct(product);

            if (productSaleDto.AdditionId.HasValue)
            {
                var addition = await _additionRepository.GetAsync(productSaleDto.AdditionId.Value);

                if (addition is null)
                {
                    throw new AdditionNotFoundException(productSaleDto.AdditionId.Value);
                }

                productSale.ChangeAddition(addition);
                await _productSaleRepository.UpdateAsync(productSale);
                return;
            }

            if (productSale.Addition is null)
            {
                await _productSaleRepository.UpdateAsync(productSale);
                return;
            }

            productSale.RemoveAddition();
            await _productSaleRepository.UpdateAsync(productSale);
        }
    }
}
