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

        [Fact]
        public async Task should_update_addition()
        {
            var addition = await AddDefaultAdditionAsync();
            addition.ChangeAdditionName("Name");
            addition.ChangePrice(12000M);

            await _additonRepository.UpdateAsync(addition);

            var additionAdded = await _additonRepository.GetAsync(addition.Id);
            additionAdded.ShouldNotBeNull();
            additionAdded.Price.Value.ShouldBe(addition.Price.Value);
            additionAdded.AdditionName.Value.ShouldBe(addition.AdditionName.Value);
        }

        [Fact]
        public async Task should_delete_addition()
        {
            var addition = await AddDefaultAdditionAsync();

            await _additonRepository.DeleteAsync(addition);

            var additionDeleted = await _additonRepository.GetAsync(addition.Id);
            additionDeleted.ShouldBeNull();
        }

        [Fact]
        public async Task should_get_all_additions()
        {
            await AddDefaultAdditionAsync();
            await AddDefaultAdditionAsync();

            var additions = await _additonRepository.GetAllAsync();

            additions.ShouldNotBeNull();
            additions.ShouldNotBeEmpty();
            additions.Count().ShouldBeGreaterThan(1);
        }

        private readonly IAdditonRepository _additonRepository;

        public AdditionRepositoryTests(OptionsProvider optionsProvider) : base(optionsProvider)
        {
            _additonRepository = GetRequiredService<IAdditonRepository>();
        }
    }
}
