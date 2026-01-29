using Microsoft.AspNetCore.Mvc;
using PamelloV7.Core.Attributes;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Modules;
using PamelloV7.Server.Exceptions;
using Microsoft.Extensions.Primitives;
using PamelloV7.Server.Model;
using PamelloV7.Core.Exceptions;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Converters;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;
using PamelloV7.Server.Extensions;
using PamelloV7.Server.Model.Audio.Modules.Pamello;
using PamelloV7.Server.Model.Audio.Speakers;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Controllers
{
    [Route("Command")]
    [ApiController]
    public class CommandsController : PamelloControllerBase
    {
        private readonly IPamelloCommandsService _commands;
        
        private readonly IAssemblyTypeResolver _typeResolver;
        
        private readonly IEntityQueryService _peql;
        
        private readonly IPamelloUserRepository _users;
        private readonly IPamelloSongRepository _songs;
        private readonly IPamelloEpisodeRepository _episodes;
        private readonly IPamelloPlaylistRepository _playlists;
        private readonly IPamelloPlayerRepository _players;
        private readonly IPamelloSpeakerRepository _speakers;

        public CommandsController(IServiceProvider services) : base(services) {
            _commands = services.GetRequiredService<IPamelloCommandsService>();
            
            _typeResolver = services.GetRequiredService<IAssemblyTypeResolver>();
            
            _peql = services.GetRequiredService<IEntityQueryService>();
        }

        [HttpGet("{*commandName}")]
        public async Task<IActionResult> Get(string commandName) {
            RequireUser();

            var commandPath = $"{commandName}{Request.QueryString}";
            StaticLogger.Log($"Command Start: {commandPath}");

            var result = await _commands.ExecuteAsync(commandPath, User);
            
            return Ok(result);
        }
    }
}
