using Microsoft.AspNetCore.Mvc;
using PamelloV7.Server.Controllers.Base;

namespace PamelloV7.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class PingController : PamelloControllerBase
{
    public PingController(IServiceProvider services) : base(services) { }

    [HttpGet("{*ignored}")]
    public IActionResult Get() {
        Console.WriteLine($"{DateTime.Now.TimeOfDay} /Ping");
        return Ok("Pong");
    }
}
