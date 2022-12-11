using Grpc.Core;
using Restaurant.Shared.AdditionProto;
using Restaurant.UI.DTO;
using Restaurant.UI.Services.Abstractions;

namespace Restaurant.UI.Services.Implementation
{
    internal sealed class AdditionService : IAdditionService
    {
		private readonly List<AdditionDto> _additions = new()
		{
			new AdditionDto { Id = Guid.NewGuid(), AdditionName = "Addition#1", Price= 100M, AdditionKind = "Drink" },
			new AdditionDto { Id = Guid.NewGuid(), AdditionName = "Addition#2", Price= 50M, AdditionKind = "Drink" },
			new AdditionDto { Id = Guid.NewGuid(), AdditionName = "Addition#3", Price= 20M, AdditionKind = "Salad" },
			new AdditionDto { Id = Guid.NewGuid(), AdditionName = "Addition#4", Price= 150M, AdditionKind = "Drink" },
			new AdditionDto { Id = Guid.NewGuid(), AdditionName = "Addition#5", Price= 25M, AdditionKind = "Salad" },
		};
		private readonly Additions.AdditionsClient _additionsClient;

		public AdditionService(Additions.AdditionsClient additionsClient)
		{
			_additionsClient = additionsClient;
		}

		public async Task<Guid> AddAsync(AdditionDto additionDto)
		{
			await Task.CompletedTask;
			additionDto.Id = Guid.NewGuid();
			_additions.Add(additionDto);
			var id = await _additionsClient.AddAdditionAsync(new Addition
			{
				AdditionName = additionDto.AdditionName,
				AdditionKind = additionDto.AdditionKind,
				Price = additionDto.Price.ToString()
			});
			return Guid.Parse(id.Id);
		}

		public Task DeleteAsync(Guid id)
		{
			var addition = _additions.SingleOrDefault(a => a.Id == id);

			if (addition is null)
			{
				return Task.CompletedTask;
			}

			_additions.Remove(addition);
			return Task.CompletedTask;
		}

		public async Task<IEnumerable<AdditionDto>> GetAllAsync()
		{
			await Task.CompletedTask;
			return _additions;
		}

		public Task<AdditionDto?> GetAsync(Guid id)
		{
			return Task.FromResult(_additions.SingleOrDefault(a => a.Id == id));
		}

		public Task UpdateAsync(AdditionDto additionDto)
		{
			return Task.CompletedTask;
		}
	}
}
