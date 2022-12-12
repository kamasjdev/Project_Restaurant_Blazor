using System.Text;

namespace Restaurant.Application.Exceptions
{
    public sealed class InvalidEmailSettingsException : BusinessException
    {
        public InvalidEmailSettings InvalidEmailSettings { get; }

        public InvalidEmailSettingsException(InvalidEmailSettings invalidEmailSettings) : base(invalidEmailSettings.ToString())
        {
            InvalidEmailSettings = invalidEmailSettings;
        }
    }
}
