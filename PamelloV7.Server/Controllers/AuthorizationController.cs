using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Services;
using PamelloV7.Server.Exceptions;
using PamelloV7.Server.Repositories;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model.Events;

namespace PamelloV7.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserAuthorizationService _authorization;
        private readonly PamelloEventsService _events;
        private readonly PamelloUserRepository _users;

        public AuthorizationController(IServiceProvider services) {
            _authorization = services.GetRequiredService<UserAuthorizationService>();
            _events = services.GetRequiredService<PamelloEventsService>();
            _users = services.GetRequiredService<PamelloUserRepository>();
        }

        [HttpGet("Events/{eventsToken}/WithCode/{code}")]
        public IActionResult GetWithCode(Guid eventsToken, int code) {
            PamelloEventListener events;

            try {
                events = _events.AuthorizeEventsWithCode(eventsToken, code);
            }
            catch (PamelloException x) {
                throw new PamelloControllerException(BadRequest(x.Message));
            }

            if (events.User is null)
                throw new PamelloControllerException(BadRequest("Unexpected exception with null user after authorization"));

            return Ok(events.User.Token);
        }

        [HttpGet("Events/{eventsToken}/WithToken/{userToken}")]
        public IActionResult GetWithToken(Guid eventsToken, Guid userToken) {
            PamelloEventListener events;

            try {
                events = _events.AuthorizeEventsWithToken(eventsToken, userToken);
            }
            catch (PamelloException x) {
                throw new PamelloControllerException(BadRequest(x.Message));
            }

            if (events.User is null)
                throw new PamelloControllerException(BadRequest("Unexpected exception with null user after authorization"));

            return Ok(events.User.Token);
        }

        [HttpGet("CloseEvents/{eventsToken}/{userToken}")]
        public IActionResult GetClose(Guid eventsToken, Guid userToken) {
            try {
                _events.UnauthorizeEvents(eventsToken, userToken);
            }
            catch (PamelloException x) {
                throw new PamelloControllerException(BadRequest(x.Message));
            }

            return Ok();
        }

        [HttpGet("GetToken/{code}")]
        public IActionResult GetToken(int code) {
            var discordId = _authorization.GetDiscordId(code);
            if (discordId is null) throw new PamelloException($"Code {code} is invalid");

            var user = _users.GetByDiscord(discordId.Value);
            if (user is null) throw new PamelloException($"Cant get user my discord id {discordId} (from code {code})");

            return Ok(user.Token);
        }
    }
}
