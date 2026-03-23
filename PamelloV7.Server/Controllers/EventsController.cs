using Microsoft.AspNetCore.Mvc;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Model.Listeners;
using PamelloV7.Server.Services;
using System.Text.Json;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services;

namespace PamelloV7.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EventsController : PamelloControllerBase
    {
        private SSEBroadcastService _events;

        public EventsController(IServiceProvider services) : base(services) {
            _events = services.GetRequiredService<ISSEBroadcastService>() as SSEBroadcastService
                ?? throw new PamelloException("ISSEBroadcastService is expected to be SSEBroadcastService");
        }

        [HttpGet]
        public async Task Connect() {
            StaticLogger.Log("test con");
            var listener = await _events.AddListener(Response, HttpContext.RequestAborted);

            StaticLogger.Log($"created \"{listener.Token}\" events connection");

            await listener.Lifetime.Task;

            StaticLogger.Log($"closed \"{listener.Token}\" events connection");
        }

        [HttpGet("{eventsToken}/Close")]
        public IActionResult GetConnect(Guid eventsToken) {
            StaticLogger.Log("test disc");
            _events.CloseEvents(eventsToken);

            return Ok();
        }
    }
}
