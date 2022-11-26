using System.Text;

namespace Restaurant.Application.Exceptions
{
    public sealed class InvalidEmailSettings
    {
        private readonly Dictionary<string, string> _settings = new();

        public bool HasErrors()
        {
            return _settings.Any();
        }

        public void AddProperty(string property)
        {
            _settings[property] = property;
        }

        public void AddPropertyWithValue(string property, string value) 
        {
            _settings[property] = value;
        }

        public void RemoveProperty(string property) 
        {
            _settings.Remove(property);
        }

        public override string ToString()
        {
            var message = new StringBuilder();
            message.Append("Invalid EmailSettings:\n");
            var count = _settings.Count;
            var iteration = 0;

            foreach (var error in _settings)
            {
                if (string.IsNullOrEmpty(error.Value))
                {
                    message.Append($"Property: '{error.Key}'");
                }
                else
                {
                    message.Append($"Property: '{error.Key}' with descripted messge: {error.Value}");
                }
                
                iteration++;

                if (count > iteration)
                {
                    message.Append('\n');
                }
            }

            return message.ToString();
        }
    }
}
