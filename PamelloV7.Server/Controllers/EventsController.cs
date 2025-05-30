using Microsoft.AspNetCore.Mvc;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Model.Listeners;
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
            Console.WriteLine("test con");
            var listener = await _events.AddListener(Response, HttpContext.RequestAborted);

            Console.WriteLine($"created \"{listener.Token}\" events connection");

            await listener.Completion.Task;

            Console.WriteLine($"closed \"{listener.Token}\" events connection");
        }

        [HttpGet("{eventsToken}/Close")]
        public IActionResult GetConnect(Guid eventsToken) {
            Console.WriteLine("test disc");
            _events.CloseEvents(eventsToken);

            return Ok();
        }
    }
}
