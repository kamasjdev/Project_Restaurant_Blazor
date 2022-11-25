namespace Restaurant.Application.Exceptions
{
    public sealed class AdditionNotFoundException : BusinessException
    {
        public Guid AdditionId { get; }

        public AdditionNotFoundException(Guid additionId) : base($"Addition with id: '{additionId}' was not found")
        {
            AdditionId = additionId;
        }
    }
}
