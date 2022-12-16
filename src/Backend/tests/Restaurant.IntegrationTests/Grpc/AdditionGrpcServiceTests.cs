using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Restaurant.Application.Exceptions;
using Restaurant.IntegrationTests.Common;
using Restaurant.Shared.AdditionProto;
using Shouldly;

namespace Restaurant.IntegrationTests.Grpc
{
    public class AdditionGrpcServiceTests : GrpcTestBase
    {
        [Fact]
        public async Task should_add_addition()
        {
            var addition = new Addition { AdditionName = nameof(Addition), AdditionKind = "Salad", Price = "120,25" };
            
            var response = await _client.AddAdditionAsync(addition);

            response.ShouldNotBeNull();
            response.Id.ShouldNotBeNullOrWhiteSpace();
            var additionAdded = await _client.GetAdditionAsync(new GetAdditionRequest { Id = response.Id });
            additionAdded.ShouldNotBeNull();
            additionAdded.AdditionName.ShouldBe(addition.AdditionName);
            additionAdded.AdditionKind.ShouldBe(addition.AdditionKind);
            decimal.Parse(additionAdded.Price).ShouldBe(decimal.Parse(addition.Price));
        }

        [Fact]
        public async Task should_update_addition()
        {
            var additionAdded = await AddDefaultAdditionAsync();
            var addition = new Addition
            {
                Id = additionAdded.Id.Value.ToString(),
                AdditionName = "abcTest123",
                AdditionKind = "Drink",
                Price = "500,50"
            };

            await _client.UpdateAdditionAsync(addition);

            var additionUpdated = await _client.GetAdditionAsync(new GetAdditionRequest { Id = addition.Id });
            additionUpdated.ShouldNotBeNull();
            additionUpdated.AdditionName.ShouldBe(addition.AdditionName);
            additionUpdated.AdditionKind.ShouldBe(addition.AdditionKind);
            decimal.Parse(additionUpdated.Price).ShouldBe(decimal.Parse(addition.Price));        }

        [Fact]
        public async Task should_delete_addition()
        {
            var additionAdded = await AddDefaultAdditionAsync();

            await _client.DeleteAdditionAsync(new DeleteAdditionRequest { Id = additionAdded.Id.Value.ToString() });

            var expectedException = new AdditionNotFoundException(additionAdded.Id);
            var additionNotExistsException = await Record.ExceptionAsync(() =>  _client.GetAdditionAsync(new GetAdditionRequest { Id = additionAdded.Id.Value.ToString() }).ResponseAsync);
            additionNotExistsException.ShouldNotBeNull();
            additionNotExistsException.ShouldBeOfType<RpcException>();
            ((RpcException)additionNotExistsException).StatusCode.ShouldBe(StatusCode.FailedPrecondition);
            ((RpcException)additionNotExistsException).Status.StatusCode.ShouldBe(StatusCode.FailedPrecondition);
            ((RpcException)additionNotExistsException).Status.Detail.ShouldNotBeNullOrWhiteSpace();
            ((RpcException)additionNotExistsException).Status.Detail.ShouldBe(expectedException.Message);
        }

        [Fact]
        public async Task should_get_all_addition()
        {
            var additionAdded1 = await AddDefaultAdditionAsync();
            var additionAdded2 = await AddDefaultAdditionAsync();

            var additions = await _client.GetAdditionsAsync(new Empty());

            additions.ShouldNotBeNull();
            additions.Additions.ShouldNotBeNull();
            additions.Additions.ShouldNotBeEmpty();
            additions.Additions.Count.ShouldBeGreaterThan(1);
            additions.Additions.ShouldContain(a => a.Id.Equals(additionAdded1.Id.Value.ToString(), StringComparison.InvariantCultureIgnoreCase));
            additions.Additions.ShouldContain(a => a.Id.Equals(additionAdded2.Id.Value.ToString(), StringComparison.InvariantCultureIgnoreCase));
        }

        [Fact]
        public async Task given_not_existing_addition_should_return_exception()
        {
            var id = Guid.NewGuid();
            var expectedException = new AdditionNotFoundException(id);

            var exception = await Record.ExceptionAsync(() => _client.GetAdditionAsync(new GetAdditionRequest { Id = id.ToString() }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.FailedPrecondition);
            ((RpcException)exception).Status.StatusCode.ShouldBe(StatusCode.FailedPrecondition);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
            ((RpcException)exception).Status.Detail.ShouldBe(expectedException.Message);
        }

        private readonly Additions.AdditionsClient _client;

        public AdditionGrpcServiceTests(OptionsProvider optionsProvider)
            : base(optionsProvider)
        {
            _client = new Additions.AdditionsClient(Channel);
        }
    }
}
