using Microsoft.AspNetCore.Mvc;
using PamelloV7.Framework.Logging;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class PingController : PamelloControllerBase
{
    public PingController(IServiceProvider services) : base(services) { }

    [HttpGet("{*ignored}")]
    public IActionResult Get() {
        StaticLogger.Log($"{DateTime.Now.TimeOfDay} /Ping");
        return Ok("Pong");
    }
}
