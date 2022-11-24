namespace Restaurant.Core.Exceptions
{
    public sealed class InvalidAdditionKindException : DomainException
    {
        public string AdditionKind { get; }

        public InvalidAdditionKindException(string additionKind) : base($"Invalid AdditionKind: '{additionKind}'")
        {
            AdditionKind = additionKind;
        }
    }
}
