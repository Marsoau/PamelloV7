using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Services;
using PamelloV7.Server.Exceptions;
using PamelloV7.Server.Repositories;

namespace PamelloV7.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserAuthorizationService _authorization;
        private readonly PamelloUserRepository _users;

        public AuthorizationController(IServiceProvider services) {
            _authorization = services.GetRequiredService<UserAuthorizationService>();
            _users = services.GetRequiredService<PamelloUserRepository>();
        }

        [HttpGet("WithCode/{code}")]
        public IActionResult GetWithCode(string code) {
            if (!int.TryParse(code, out var intcode))
                throw new PamelloControllerException(BadRequest($"cant parce code \"{code}\""));

            var userDiscordId = _authorization.GetDiscordId(intcode);
            if (userDiscordId is null)
                throw new PamelloControllerException(BadRequest($"wrong code"));

            var user = _users.GetByDiscord(userDiscordId.Value);
            if (user is null)
                throw new PamelloControllerException(NotFound($"unexpected null user exception"));

            return Ok(user.Token);
        }
    }
}
