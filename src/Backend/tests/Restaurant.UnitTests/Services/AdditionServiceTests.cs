using NSubstitute;
using Restaurant.Application.Abstractions;
using Restaurant.Application.DTO;
using Restaurant.Application.Exceptions;
using Restaurant.Application.Services;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using Shouldly;

namespace Restaurant.UnitTests.Services
{
    public class AdditionServiceTests
    {
        [Fact]
        public async Task should_create_addition()
        {
            var addition = new AdditionDto { AdditionName = "Addition #1", Price = 20, AdditionKind = AdditionKind.Drink.ToString() };

            await _additionService.AddAsync(addition);

            await _additonRepository.Received(1).AddAsync(Arg.Any<Addition>());
        }

        [Fact]
        public async Task should_update_addition()
        {
            var addition = AddDefaultAddition();
            var additionDto = new AdditionDto { Id = addition.Id, AdditionName = "Addition #1", Price = 20, AdditionKind = AdditionKind.Salad.ToString() };

            await _additionService.UpdateAsync(additionDto);

            await _additonRepository.Received(1).UpdateAsync(addition);
        }

        [Fact]
        public async Task given_invalid_id_when_update_should_throw_an_exception()
        {
            var additionDto = new AdditionDto { Id = Guid.NewGuid(), AdditionName = "Addition #1", Price = 20, AdditionKind = AdditionKind.Salad.ToString() };
            var expectedException = new AdditionNotFoundException(additionDto.Id);

            var exception = await Record.ExceptionAsync(() => _additionService.UpdateAsync(additionDto));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((AdditionNotFoundException)exception).AdditionId.ShouldBe(expectedException.AdditionId);
        }

        [Fact]
        public async Task should_delete_addition()
        {
            var addition = AddDefaultAddition();

            await _additionService.DeleteAsync(addition.Id);

            await _additonRepository.Received(1).DeleteAsync(addition);
        }

        [Fact]
        public async Task given_invalid_id_when_delete_should_throw_an_exception()
        {
            var id = Guid.NewGuid();
            var expectedException = new AdditionNotFoundException(id);

            var exception = await Record.ExceptionAsync(() => _additionService.DeleteAsync(id));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((AdditionNotFoundException)exception).AdditionId.ShouldBe(expectedException.AdditionId);
        }

        [Fact]
        public async Task given_addition_ordered_when_delete_should_throw_an_exception()
        {
            var addition = AddDefaultAddition(new List<EntityId> { new EntityId(Guid.NewGuid()) });
            var expectedException = new CannotDeleteAdditionOrderedException(addition.Id);

            var exception = await Record.ExceptionAsync(() => _additionService.DeleteAsync(addition.Id));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((CannotDeleteAdditionOrderedException)exception).AdditionId.ShouldBe(expectedException.AdditionId);
        }

        private Addition AddDefaultAddition(IEnumerable<EntityId> productSaleIds = null)
        {
            var addition = new Addition(Guid.NewGuid(), "Addition#1", 10, AdditionKind.Drink, productSaleIds);
            _additonRepository.GetAsync(addition.Id).Returns(addition);
            return addition;
        }

        private readonly IAdditionService _additionService;
        private readonly IAdditonRepository _additonRepository;

        public AdditionServiceTests()
        {
            _additonRepository = Substitute.For<IAdditonRepository>();
            _additionService = new AdditionService(_additonRepository);
        }
    }
}
