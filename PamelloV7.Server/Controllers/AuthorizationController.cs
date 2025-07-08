﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Services;
using PamelloV7.Server.Exceptions;
using PamelloV7.Server.Repositories;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Server.Model.Listeners;
using PamelloV7.Server.Repositories.Database;

namespace PamelloV7.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly ICodeAuthorizationService _authorization;
        private readonly PamelloEventsService _events;
        private readonly IPamelloUserRepository _users;

        public AuthorizationController(IServiceProvider services) {
            _authorization = services.GetRequiredService<ICodeAuthorizationService>();
            _events = services.GetRequiredService<PamelloEventsService>();
            _users = services.GetRequiredService<IPamelloUserRepository>();
        }

        [HttpGet("Events/{eventsToken}/WithCode/{code}")]
        public IActionResult GetWithCode(Guid eventsToken, int code) {
            PamelloEventListener events;

            events = _events.AuthorizeEventsWithCode(eventsToken, code);

            if (events.User is null)
                throw new PamelloControllerException(BadRequest("Unexpected exception with null user after authorization"));

            return Ok(events.User.Token);
        }

        [HttpGet("Events/{eventsToken}/WithToken/{userToken}")]
        public IActionResult GetWithToken(Guid eventsToken, Guid userToken) {
            PamelloEventListener events;

            events = _events.AuthorizeEventsWithToken(eventsToken, userToken);

            if (events.User is null)
                throw new PamelloControllerException(BadRequest("Unexpected exception with null user after authorization"));

            return Ok(events.User.Token);
        }

        [HttpGet("Events/{eventsToken}/Unauthorize")]
        public IActionResult GetClose(Guid eventsToken) {
            _events.UnauthorizeEvents(eventsToken);

            return Ok();
        }

        [HttpGet("GetToken/{code}")]
        public IActionResult GetToken(int code) {
            var user = _authorization.GetUser(code);
            if (user is null) throw new PamelloControllerException(NotFound($"Cant get user by authorization code {code}"));

            return Ok(user.Token);
        }
    }
}
