using Microsoft.AspNetCore.Mvc;
using PamelloV7.Server.Controllers.Base;

namespace PamelloV7.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class PingController : PamelloControllerBase
{
    public PingController(IServiceProvider services) : base(services) {}

    [HttpGet("{*ignored}")]
    public IActionResult Get() {
        var value = Request.Path.Value.Substring(Request.Path.Value.IndexOf('/', 1) + 1);
        return Ok(value.Length == 0 ? "Pong" : $"Pong: {value}");
    }
}
