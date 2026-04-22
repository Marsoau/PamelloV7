using System.Text;
using Microsoft.AspNetCore.Mvc;
using PamelloV7.Framework.Logging;
using PamelloV7.Server.Controllers.Base;

namespace PamelloV7.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class TestController : PamelloControllerBase
{
    public TestController(IServiceProvider services) : base(services) { }

    [HttpGet]
    public async Task Get() {
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("Got test request");

        try {
            await Task.Delay(5000, HttpContext.RequestAborted);
        }
        catch (OperationCanceledException) {
            //ignore
        }
        
        Output.Write($"RA: {HttpContext.RequestAborted.IsCancellationRequested}");
        
        Console.WriteLine("End test request");

        await Response.BodyWriter.WriteAsync("Hello, World! 3"u8.ToArray());
        await Response.BodyWriter.CompleteAsync();
    }
}
