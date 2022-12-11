namespace Restaurant.Infrastructure.Grpc
{
    internal sealed class ValidationException : Exception
    {
        public List<string> _validationErrors = new();
        public IEnumerable<string> ValidationErrors => _validationErrors;

        public ValidationException(string message) : base(message)
        {
            _validationErrors.Add(message);
        }

        public ValidationException(IEnumerable<string> messages) : base(GetValidationMessages(messages))
        {
            _validationErrors.AddRange(messages);
        }

        private static string GetValidationMessages(IEnumerable<string> validationErrors)
        {
            return string.Join(". ", validationErrors);
        }
    }
}
