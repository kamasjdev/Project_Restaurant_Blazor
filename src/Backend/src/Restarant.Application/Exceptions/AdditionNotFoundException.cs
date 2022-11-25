namespace Restarant.Application.Exceptions
{
    public sealed class AdditionNotFoundException : BussinessException
    {
        public Guid AdditionId { get; }

        public AdditionNotFoundException(Guid additionId) : base($"Addition with id: '{additionId}' was not found")
        {
            AdditionId = additionId;
        }
    }
}
