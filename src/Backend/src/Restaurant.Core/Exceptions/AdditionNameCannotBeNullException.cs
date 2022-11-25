namespace Restaurant.Core.Exceptions
{
    public class AdditionNameCannotBeNullException : DomainException
    {
        public AdditionNameCannotBeNullException() : base("AdditionName cannot be null")
        {
        }
    }
}
