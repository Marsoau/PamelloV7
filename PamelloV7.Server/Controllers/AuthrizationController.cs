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
    public class AuthrizationController : ControllerBase
    {
        private readonly UserAuthorizationService _authorization;
        private readonly PamelloUserRepository _users;

        public AuthrizationController(IServiceProvider services) {
            _authorization = services.GetRequiredService<UserAuthorizationService>();
            _users = services.GetRequiredService<PamelloUserRepository>();
        }

        [HttpGet("WithCode")]
        public IActionResult GetWithCode() {
            StringValues qCodes;
            if (!Request.Query.TryGetValue("code", out qCodes))
                throw new PamelloControllerException(BadRequest("cant find required \"code\" in querry"));

            var qCode = qCodes.FirstOrDefault();

            if (!int.TryParse(qCode, out var code))
                throw new PamelloControllerException(BadRequest($"cant parce code \"{qCode}\""));

            var userDiscordId = _authorization.GetDiscordId(code);
            if (userDiscordId is null)
                throw new PamelloControllerException(BadRequest($"wrong code"));

            var user = _users.GetByDiscord(userDiscordId.Value);
            if (user is null)
                throw new PamelloControllerException(NotFound($"unexpected null user exception"));

            return Ok(user.Token);
        }
    }
}
