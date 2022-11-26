using Restaurant.Application.Abstractions;

namespace Restaurant.Infrastructure.Time
{
    internal sealed class Clock : IClock
    {
        public DateTime CurrentDate()
        {
            return DateTime.UtcNow;
        }
    }
}
