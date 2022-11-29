using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Abstractions;
using Restaurant.Application.DTO;
using Restaurant.Application.Exceptions;
using Restaurant.Shared.AdditionProto;

namespace Restaurant.Infrastructure.Grpc.Services
{
	internal sealed class AdditionGrpcService : Additions.AdditionsBase
	{
		private readonly IServiceProvider _serviceProvider;

		public AdditionGrpcService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public override async Task<AddAdditionResponse> AddAddition(Addition request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var additionService = scope.ServiceProvider.GetRequiredService<IAdditionService>();
			_ = decimal.TryParse(request.Price, out var price);
			var addition = new AdditionDto { AdditionName = request.AdditionName, Price = price, AdditionKind = request.AdditionName };
			await additionService.AddAsync(addition);
			return new AddAdditionResponse { Id = addition.Id.ToString() };
		}

		public override async Task<Empty> UpdateAddition(Addition request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var additionService = scope.ServiceProvider.GetRequiredService<IAdditionService>();
			_ = decimal.TryParse(request.Price, out var price);
			var addition = new AdditionDto { AdditionName = request.AdditionName, Price = price, AdditionKind = request.AdditionName };
			await additionService.UpdateAsync(addition);
			return new Empty();
		}

		public override async Task<Empty> DeleteAddition(DeleteAdditionRequest request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var additionService = scope.ServiceProvider.GetRequiredService<IAdditionService>();
			_ = Guid.TryParse(request.Id, out var id);
			await additionService.DeleteAsync(id);
			return new Empty();
		}

		public override async Task<Addition> GetAddition(GetAdditionRequest request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var additionService = scope.ServiceProvider.GetRequiredService<IAdditionService>();
			_ = Guid.TryParse(request.Id, out var id);
			var addition = await additionService.GetAsync(id);

			if (addition is null)
			{
				throw new AdditionNotFoundException(id);
			}

			return new Addition
			{
				Id = addition.Id.ToString(),
				AdditionName = addition.AdditionName,
				AdditionKind = addition.AdditionKind,
				Price = addition.Price.ToString()
			};
		}

		public override async Task<GetAdditionsResponse> GetAdditions(Empty request, ServerCallContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var additionService = scope.ServiceProvider.GetRequiredService<IAdditionService>();
			var additions = await additionService.GetAllAsync();
			var additionsResponse = new GetAdditionsResponse();
			additionsResponse.Additions.AddRange(additions.Select(a =>
				new Addition
				{
					Id = a.Id.ToString(),
					AdditionName = a.AdditionName,
					AdditionKind = a.AdditionKind,
					Price = a.Price.ToString()
				}
			));
			return additionsResponse;
		}
	}
}
