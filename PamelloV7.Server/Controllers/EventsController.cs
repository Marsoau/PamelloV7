using Microsoft.AspNetCore.Mvc;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Model.Events;
using PamelloV7.Server.Services;
using System.Text.Json;

namespace PamelloV7.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EventsController : PamelloControllerBase
    {
        private PamelloEventsService _events;

        public EventsController(IServiceProvider services) : base(services) {
            _events = services.GetRequiredService<PamelloEventsService>();
        }

        [HttpGet]
        public async Task Connect() {
            var listener = await _events.AddListener(Response);

            Console.WriteLine($"created \"{listener.Token}\" events connection");

            while (!HttpContext.RequestAborted.IsCancellationRequested) {
                await Task.Delay(5000);
            }

            listener.Close();

            Console.WriteLine($"closed \"{listener.Token}\" events connection");
        }
    }
}
