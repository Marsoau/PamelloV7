using Microsoft.AspNetCore.Mvc;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Core.Services;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Controllers
{
    [Route("Command")]
    [ApiController]
    public class CommandsController : PamelloControllerBase
    {
        private readonly IPamelloCommandsService _commands;

        public CommandsController(IServiceProvider services) : base(services) {
            _commands = services.GetRequiredService<IPamelloCommandsService>();
        }

        [HttpGet("{*commandName}")]
        public async Task<IActionResult> Get(string commandName) {
            RequireUser();

            var commandPath = $"{commandName}{Request.QueryString}";
            StaticLogger.Log($"Command Start: {commandPath}");

            var result = await _commands.ExecutePathAsync(commandPath, User);
            
            return Ok(result);
        }
    }
}
