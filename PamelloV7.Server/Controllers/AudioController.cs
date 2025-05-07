using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Repositories;
using PamelloV7.Server.Services;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AudioController : PamelloControllerBase
    {
        private readonly PamelloSpeakerService _speakers;
        private readonly PamelloUserRepository _users;

        public AudioController(IServiceProvider services) : base(services) {
            _speakers = services.GetRequiredService<PamelloSpeakerService>();
            _users = services.GetRequiredService<PamelloUserRepository>();
        }

        [HttpGet("In/{channel}")]
        public async Task In(int channel) {
            throw new NotImplementedException();
        }

        [HttpGet("Out/{channel}")]
        public async Task Out(int channel) {
            var speaker = _speakers.GetInternetSpeaker(channel);
            if (speaker is null) throw new PamelloException($"No speaker found for channel {channel}");

            var listener = await speaker.AddListener(Response);
            Console.WriteLine($"isl {listener.Id} on channel <{channel}> added");

            while (!HttpContext.RequestAborted.IsCancellationRequested) {
                await Task.Delay(1000);
            }

            await listener.CloseConnection();

            Console.WriteLine($"isl {listener.Id} cancelation requested");

            await listener.Completion.Task;

            Console.WriteLine($"isl {listener.Id} on channel <{channel}> closed");
        }
    }
}
