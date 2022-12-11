using Restaurant.Shared.AdditionProto;
using Restaurant.UI.DTO;
using Restaurant.UI.Services.Abstractions;

namespace Restaurant.UI.Services.Implementation
{
    internal sealed class AdditionService : IAdditionService
    {
		private readonly Additions.AdditionsClient _additionsClient;

		public AdditionService(Additions.AdditionsClient additionsClient)
		{
			_additionsClient = additionsClient;
		}

		public async Task<Guid> AddAsync(AdditionDto additionDto)
		{
			var id = await _additionsClient.AddAdditionAsync(new Addition
			{
				AdditionName = additionDto.AdditionName,
				AdditionKind = additionDto.AdditionKind,
				Price = additionDto.Price.ToString()
			});
			return Guid.Parse(id.Id);
		}

		public async Task DeleteAsync(Guid id)
		{
			await _additionsClient.DeleteAdditionAsync(new DeleteAdditionRequest { Id = id.ToString() });
		}

		public async Task<IEnumerable<AdditionDto>> GetAllAsync()
		{
			var additions = await _additionsClient.GetAdditionsAsync(new Google.Protobuf.WellKnownTypes.Empty());
			return additions.Additions.Select(a => new AdditionDto
			{
				Id = Guid.Parse(a.Id),
				AdditionName = a.AdditionName,
				AdditionKind = a.AdditionKind,
				Price = decimal.Parse(a.Price)
			});
		}

		public async Task<AdditionDto?> GetAsync(Guid id)
		{
			var addition = await _additionsClient.GetAdditionAsync(new GetAdditionRequest { Id = id.ToString() });
			return addition is not null ? new AdditionDto
			{
				Id = Guid.Parse(addition.Id),
				AdditionName = addition.AdditionName,
				AdditionKind = addition.AdditionKind,
				Price = decimal.Parse(addition.Price)
			} : null;
		}

		public async Task UpdateAsync(AdditionDto additionDto)
		{
			await _additionsClient.UpdateAdditionAsync(new Addition
			{
				Id = additionDto.Id.ToString(),
				AdditionName = additionDto.AdditionName,
				AdditionKind = additionDto.AdditionKind,
				Price = additionDto.Price.ToString()
			});
		}
	}
}
