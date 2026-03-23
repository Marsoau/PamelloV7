using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Services;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Repositories;
using PamelloV7.Server.Exceptions;
using PamelloV7.Server.Speakers;

namespace PamelloV7.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AudioController : PamelloControllerBase
    {
        private readonly IPamelloSpeakerRepository _speakers;
        private readonly IPamelloUserRepository _users;

        public AudioController(IServiceProvider services) : base(services) {
            _speakers = services.GetRequiredService<IPamelloSpeakerRepository>();
            _users = services.GetRequiredService<IPamelloUserRepository>();
        }

        [HttpGet("Out/{value}")]
        public async Task Out(string value) {
            TryGetUser();

            StaticLogger.Log($"GOT: {HttpContext.Request.Path}");

            var speaker = _speakers.GetByName(User, value);
            if (speaker is not PamelloInternetSpeaker internetSpeaker) throw new PamelloControllerException(BadRequest($"Speaker \"{value}\" not found"));
            
            var listener = await internetSpeaker.CreateListener(Response, HttpContext.RequestAborted, User);
            StaticLogger.Log($"{(User is null ? $"Unknown ISL connection" : $"User {User} connects ISL")} to <{speaker.Name}>");

            await Task.Delay(-1, HttpContext.RequestAborted);
            StaticLogger.Log("REQUEST ABORTED");
            StaticLogger.Log("REQUEST ABORTED");
            StaticLogger.Log("REQUEST ABORTED");
        }
    }
}
