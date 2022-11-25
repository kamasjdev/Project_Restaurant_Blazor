using System.Net;

namespace Restaurant.Infrastructure.Exceptions
{
    public record ExceptionResponse(object Response, HttpStatusCode HttpStatusCode);
}
