namespace Restaurant.Core.Exceptions
{
    public sealed class AdditionNameTooShortException : DomainException
    {
        public string AdditionName { get; }

        public AdditionNameTooShortException(string addtionName) : base($"AdditionName: '{addtionName}' should have at least 3 characters")
        {
            AdditionName = addtionName;
        }
    }
}
