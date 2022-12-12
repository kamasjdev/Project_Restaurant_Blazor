using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Abstractions;
using Restaurant.Application.Exceptions;
using Restaurant.Shared.ProductSaleProto;

namespace Restaurant.Infrastructure.Grpc.Services
{
	internal class ProductSaleGrpcService : ProductSales.ProductSalesBase
	{
		private readonly IServiceProvider _serviceProvider;

		public ProductSaleGrpcService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public override async Task<AddProductSaleResponse> AddProduct(AddProductSaleRequest request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var productSaleService = scope.ServiceProvider.GetRequiredService<IProductSaleService>();
			var dto = request.AsDto();
			await productSaleService.AddAsync(dto);
			return new AddProductSaleResponse { Id = dto.Id.ToString() };
		}

		public override async Task<Empty> UpdateProduct(AddProductSaleRequest request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var productSaleService = scope.ServiceProvider.GetRequiredService<IProductSaleService>();
			await productSaleService.UpdateAsync(request.AsDto());
			return new Empty();
		}

		public override async Task<Empty> DeleteProductSale(DeleteProductSaleRequest request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var productSaleService = scope.ServiceProvider.GetRequiredService<IProductSaleService>();
			await productSaleService.DeleteAsync(request.Id.AsGuid());
			return new Empty();
		}

		public override async Task<ProductSale> GetProductSale(GetProductSaleRequest request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var productSaleService = scope.ServiceProvider.GetRequiredService<IProductSaleService>();
			var id = request.Id.AsGuid();
			var productSale = await productSaleService.GetAsync(id);

			if (productSale is null)
			{
				throw new ProductSaleNotFoundException(id);
			}

			return new ProductSale
			{
				Id = request.Id,
				Email = productSale.Email,
				ProductSaleState = productSale.ProductSaleState,
				EndPrice = productSale.EndPrice.ToString(),
				Product = new Product { Id = productSale.Product.Id.ToString(), Price = productSale.Product.Price.ToString(), ProductName = productSale.Product.ProductName, ProductKind = productSale.Product.ProductKind },
				Addition = productSale.Addition is not null ?
					new Addition { Id = productSale.Addition.Id.ToString(), AdditionName = productSale.Addition.AdditionName, AdditionKind = productSale.Addition.AdditionKind, Price = productSale.Addition.Price.ToString() }
					: null,
				Order = productSale.Order is not null ?
					new Order { Id = productSale.Order.Id.ToString(), Email = productSale.Order.Email, Created = Timestamp.FromDateTime(productSale.Order.Created), OrderNumber = productSale.Order.OrderNumber, Price = productSale.Order.Price.ToString(), Note = productSale.Order.Note }
					: null
			};
		}

		public override async Task<GetProductSalesByEmailResponse> GetProductSalesInCartByEmail(GetProductSalesByEmailRequest request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var productSaleService = scope.ServiceProvider.GetRequiredService<IProductSaleService>();
			var productSales = await productSaleService.GetAllInCartByEmailAsync(request.Email);
			var response = new GetProductSalesByEmailResponse();
			response.ProductSales.AddRange(productSales.Select(ps => new ProductSale
			{
				Id = ps.Id.ToString(),
				Email = ps.Email,
				ProductSaleState = ps.ProductSaleState,
				EndPrice = ps.EndPrice.ToString(),
				Product = new Product { Id = ps.Product.Id.ToString(), Price = ps.Product.Price.ToString(), ProductName = ps.Product.ProductName, ProductKind = ps.Product.ProductKind },
				Addition = ps.Addition is not null ?
					new Addition { Id = ps.Addition.Id.ToString(), AdditionName = ps.Addition.AdditionName, AdditionKind = ps.Addition.AdditionKind, Price = ps.Addition.Price.ToString() }
					: null,
				Order = ps.Order is not null ?
					new Order { Id = ps.Order.Id.ToString(), Email = ps.Order.Email, Created = Timestamp.FromDateTime(ps.Order.Created), OrderNumber = ps.Order.OrderNumber, Price = ps.Order.Price.ToString(), Note = ps.Order.Note }
					: null
			}));
			return response;
		}

		public override async Task<GetProductSalesByOrderIdResponse> GetProductSalesByOrderId(GetProductSalesByOrderIdRequest request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var productSaleService = scope.ServiceProvider.GetRequiredService<IProductSaleService>();
			var orderId = request.OrderId.AsGuid();
			var productSales = await productSaleService.GetAllByOrderIdAsync(orderId);
			var response = new GetProductSalesByOrderIdResponse();
			response.ProductSales.AddRange(productSales.Select(ps => new ProductSale
			{
				Id = ps.Id.ToString(),
				Email = ps.Email,
				ProductSaleState = ps.ProductSaleState,
				EndPrice = ps.EndPrice.ToString(),
				Product = new Product { Id = ps.Product.Id.ToString(), Price = ps.Product.Price.ToString(), ProductName = ps.Product.ProductName, ProductKind = ps.Product.ProductKind },
				Addition = ps.Addition is not null ?
					new Addition { Id = ps.Addition.Id.ToString(), AdditionName = ps.Addition.AdditionName, AdditionKind = ps.Addition.AdditionKind, Price = ps.Addition.Price.ToString() }
					: null,
				Order = ps.Order is not null ?
					new Order { Id = ps.Order.Id.ToString(), Email = ps.Order.Email, Created = Timestamp.FromDateTime(ps.Order.Created), OrderNumber = ps.Order.OrderNumber, Price = ps.Order.Price.ToString(), Note = ps.Order.Note }
					: null
			}));
			return response;
		}
	}
}
