using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.IntegrationTests.Common;
using Shouldly;

namespace Restaurant.IntegrationTests.RepositoryTests
{
    public class AdditionRepositoryTests : BaseTest
    {
        [Fact]
        public async Task should_add_addition()
        {
            var addition = new Addition(Guid.NewGuid(), "Addition#1", 100M, AdditionKind.Salad);

            await _additonRepository.AddAsync(addition);

            var additionAdded = await _additonRepository.GetAsync(addition.Id);
            additionAdded.ShouldNotBeNull();
            additionAdded.AdditionName.Value.ShouldBe(addition.AdditionName.Value);
            additionAdded.Price.Value.ShouldBe(addition.Price.Value);
            additionAdded.AdditionKind.ShouldBe(addition.AdditionKind);
        }

        private readonly IAdditonRepository _additonRepository;

        public AdditionRepositoryTests(OptionsProvider optionsProvider) : base(optionsProvider)
        {
            _additonRepository = GetRequiredService<IAdditonRepository>();
        }
    }
}
