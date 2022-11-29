using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Abstractions;
using Restaurant.Application.DTO;
using Restaurant.Application.Exceptions;
using Restaurant.Shared.ProductProto;

namespace Restaurant.Infrastructure.Grpc.Services
{
	internal sealed class ProductGrpcService : Products.ProductsBase
	{
		private readonly IServiceProvider _serviceProvider;

		public ProductGrpcService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public override async Task<AddProductResponse> AddProduct(Product request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
			_ = decimal.TryParse(request.Price, out var price);
			var dto = new ProductDto { ProductName = request.ProductName, Price = price, ProductKind = request.ProductKind };
			await productService.AddAsync(dto);
			return new AddProductResponse { Id = dto.Id.ToString() };
		}

		public override async Task<Empty> UpdateProduct(Product request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
			_ = decimal.TryParse(request.Price, out var price);
			_ = Guid.TryParse(request.Id, out var id);
			var dto = new ProductDto { Id = id, ProductName = request.ProductName, Price = price, ProductKind = request.ProductKind };
			await productService.UpdateAsync(dto);
			return new Empty();
		}

		public override async Task<Empty> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
			_ = Guid.TryParse(request.Id, out var id);
			await productService.DeleteAsync(id);
			return new Empty();
		}

		public override async Task<ProductDetails> GetProduct(GetProductRequest request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
			_ = Guid.TryParse(request.Id, out var id);
			var product = await productService.GetAsync(id);

			if (product is null)
			{
				throw new ProductNotFoundException(id);
			}

			var productDetails = new ProductDetails
			{
				Id = product.Id.ToString(),
				ProductName = product.ProductName?.ToString(),
				Price = product.Price.ToString(),
				ProductKind = product.ProductKind?.ToString()
			};
			productDetails.Orders.AddRange(product.Orders?.Select(o => new Order
			{
				Id = o.Id.ToString(),
				Email = o.Email,
				OrderNumber = o.OrderNumber,
				Price = o.Price.ToString(),
				Created = Timestamp.FromDateTime(o.Created),
				Note = o.Note
			}));
			return productDetails;
		}

		public override async Task<GetProductsResponse> GetProducts(Empty request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
			var products = await productService.GetAllAsync();
			var response = new GetProductsResponse();
			response.Products.AddRange(products.Select(p => new Product
			{
				Id = p.Id.ToString(),
				ProductName = p.ProductName,
				Price = p.Price.ToString(),
				ProductKind = p.ProductKind
			}));
			return response;
		}
	}
}
