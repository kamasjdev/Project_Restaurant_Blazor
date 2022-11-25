using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Restaurant.Infrastructure;

namespace Restaurant.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly IOptionsMonitor<AppOptions> _appOptions;

        public HealthCheckController(IOptionsMonitor<AppOptions> appOptions)
        {
            _appOptions = appOptions;
        }

        [HttpGet]
        public string Get()
        {
            return _appOptions.CurrentValue.Name ?? "Api";
        }
    }
}
