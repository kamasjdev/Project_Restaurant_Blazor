namespace Restaurant.Core.Exceptions
{
    public sealed class AdditionNameCannotBeEmptyException : DomainException
    {
        public AdditionNameCannotBeEmptyException() : base("AdditionName cannot be empty")
        {
        }
    }
}
