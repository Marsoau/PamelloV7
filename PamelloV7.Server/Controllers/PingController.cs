using Microsoft.AspNetCore.Mvc;
using PamelloV7.Server.Controllers.Base;

namespace PamelloV7.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class PingController : PamelloControllerBase
{
    public PingController(IServiceProvider services) : base(services) {}

    [HttpGet()]
    public IActionResult Get() {
        return Ok("Pong");
    }
}
