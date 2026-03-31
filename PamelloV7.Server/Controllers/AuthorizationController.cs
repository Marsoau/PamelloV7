using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Services;
using PamelloV7.Server.Exceptions;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services;
using PamelloV7.Server.Model.Listeners;

namespace PamelloV7.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly ICodeAuthorizationService _authorization;
        private readonly IPamelloUserRepository _users;

        public AuthorizationController(IServiceProvider services) {
            _authorization = services.GetRequiredService<ICodeAuthorizationService>();
            _users = services.GetRequiredService<IPamelloUserRepository>();
        }

        [HttpGet("GetToken/{code}")]
        public IActionResult GetToken(int code) {
            var user = _authorization.GetUser(code);
            if (user is null) throw new PamelloControllerException(NotFound($"Cant get user by authorization code {code}"));

            return Ok(user.Token);
        }
    }
}
