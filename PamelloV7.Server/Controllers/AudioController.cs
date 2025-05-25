using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Repositories;
using PamelloV7.Server.Services;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model.Audio.Speakers;
using PamelloV7.Server.Repositories.Database;
using PamelloV7.Server.Repositories.Dynamic;

namespace PamelloV7.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AudioController : PamelloControllerBase
    {
        private readonly PamelloSpeakerRepository _speakers;
        private readonly PamelloUserRepository _users;

        public AudioController(IServiceProvider services) : base(services) {
            _speakers = services.GetRequiredService<PamelloSpeakerRepository>();
            _users = services.GetRequiredService<PamelloUserRepository>();
        }

        [HttpGet("Out/{value}")]
        public async Task Out(string value) {
            TryGetUser();
            
            var speaker = await _speakers.GetByValueRequired<PamelloInternetSpeaker>(value, User);
            
            var listener = await speaker.AddListener(Response, HttpContext.RequestAborted, User);
            Console.WriteLine($"{(User is null ? $"Unknown ISL-{listener.Id} connection" : $"User {User} connects ISL-{listener.Id}")} to <{speaker.Name}>");

            await listener.Completion.Task;
        }
    }
}
