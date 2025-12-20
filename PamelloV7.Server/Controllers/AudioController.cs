using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Services;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Model.Audio;
using PamelloV7.Core.Repositories;
using PamelloV7.Server.Model.Audio.Speakers;

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
            
            var speaker = await _speakers.GetByValueRequired<IPamelloInternetSpeaker>(value, User);
            
            var listener = await speaker.AddListener(Response, HttpContext.RequestAborted, User);
            Console.WriteLine($"{(User is null ? $"Unknown ISL-{listener.Id} connection" : $"User {User} connects ISL-{listener.Id}")} to <{speaker.Name}>");

            await Task.Run(() => listener.Completion.Task.Wait(), HttpContext.RequestAborted);
        }
    }
}
