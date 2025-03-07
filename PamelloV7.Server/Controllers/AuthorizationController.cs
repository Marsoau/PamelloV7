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

        [HttpGet("{eventsToken}/WithCode/{code}")]
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

        [HttpGet("{eventsToken}/WithToken/{userToken}")]
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
    }
}
