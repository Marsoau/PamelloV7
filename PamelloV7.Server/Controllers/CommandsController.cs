using Microsoft.AspNetCore.Mvc;
using PamelloV7.Server.Attributes;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Modules;
using PamelloV7.Server.Exceptions;
using Microsoft.Extensions.Primitives;
using PamelloV7.Server.Model;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Repositories;
using PamelloV7.Core.Exceptions;
using System.ComponentModel;
using System.Reflection;
using PamelloV7.Server.Extensions;

namespace PamelloV7.Server.Controllers
{
    [Route("Command/{commandName}")]
    [ApiController]
    public class CommandsController : PamelloControllerBase
    {
        private readonly PamelloUserRepository _users;
        private readonly PamelloSongRepository _songs;
        private readonly PamelloEpisodeRepository _episodes;
        private readonly PamelloPlaylistRepository _playlists;
        private readonly PamelloPlayerRepository _players;

        public CommandsController(IServiceProvider services) : base(services) {
            _users = services.GetRequiredService<PamelloUserRepository>();
            _songs = services.GetRequiredService<PamelloSongRepository>();
            _episodes = services.GetRequiredService<PamelloEpisodeRepository>();
            _playlists = services.GetRequiredService<PamelloPlaylistRepository>();
            _players = services.GetRequiredService<PamelloPlayerRepository>();
        }

        [HttpGet]
        public async Task<IActionResult> Get(string commandName) {
            RequireUser();

            var commandMethod = typeof(PamelloCommandsModule).GetMethod(commandName);
            if (commandMethod is null || !commandMethod.CustomAttributes.Any(attibute => attibute.AttributeType == typeof(PamelloCommandAttribute))) {
                throw new PamelloControllerException(NotFound($"command with name \"{commandName}\" not found"));
            }

            Console.WriteLine($"[Command {commandMethod.Name}] {User}");

            var argsInfo = commandMethod.GetParameters();
            var args = new object?[argsInfo.Length];

            string? argName;
            object? argValue = null;
            StringValues argStringValues;
            string argStringValue;
            for (int i = 0; i < argsInfo.Length; i++) {
                argName = argsInfo[i].Name;
                if (argName is null) continue;

                if (!Request.Query.TryGetValue(argName, out argStringValues)) {
                    //throw new PamelloControllerException(BadRequest($"couldnt find required \"{argName}\""));
                }
                argStringValue = argStringValues.FirstOrDefault();

                Console.WriteLine(argsInfo[i].ParameterType);
                Console.WriteLine(typeof(IEntity).IsAssignableFrom(argsInfo[i].ParameterType));
                if (typeof(IEntity).IsAssignableFrom(argsInfo[i].ParameterType)) {
                    try {
                        Console.WriteLine($"cehchink for {argsInfo[i].Name}");
                        if (Nullable.GetUnderlyingType(argsInfo[i].ParameterType) is not null) {
                            Console.WriteLine($"arg {argsInfo[i].Name} is optional");
                        }
                        if (argsInfo[i].ParameterType == typeof(PamelloUser)) {
                            argValue = await _users.GetByValueRequired(argStringValues.FirstOrDefault() ?? "", User);
                        }
                        else if (argsInfo[i].ParameterType == typeof(PamelloSong)) {
                            argValue = await _songs.GetByValueRequired(argStringValues.FirstOrDefault() ?? "", User);
                        }
                        else if (argsInfo[i].ParameterType == typeof(PamelloEpisode)) {
                            argValue = await _episodes.GetByValueRequired(argStringValues.FirstOrDefault() ?? "", User);
                        }
                        else if (argsInfo[i].ParameterType == typeof(PamelloPlaylist)) {
                            argValue = await _playlists.GetByValueRequired(argStringValues.FirstOrDefault() ?? "", User);
                        }
                        else if (argsInfo[i].ParameterType == typeof(PamelloPlayer)) {
                            argValue = await _players.GetByValueRequired(argStringValues.FirstOrDefault() ?? "", User);
                        }
                    }
                    catch (PamelloException x) {
                        throw new PamelloControllerException(BadRequest(x.Message));
                    }
                }
                else {
                    try {
                        argValue = TypeDescriptor.GetConverter(argsInfo[i].ParameterType).ConvertFromString(argStringValues.FirstOrDefault());
                    }
                    catch {
                        throw new PamelloControllerException(BadRequest($"couldnt convert \"{argStringValue}\" to type \"{argsInfo[i].ParameterType.Name}\""));
                    }
                }

                args[i] = argValue;
            }

            try {
                object? result;

                /*
                if (typeof(Task).IsAssignableFrom(commandMethod.ReturnType)) {
                    result = await commandMethod.InvokeAsync(User.Commands, args);
                }
                else
                */
                result = commandMethod.Invoke(User.Commands, args);

                if (result is IEntity resultEntity) {
                    result = resultEntity.Id;
                }

                return Ok(result);
            }
			catch (TargetInvocationException tie) {
				throw new PamelloControllerException(BadRequest($"Execution of command interrupted by exception, message: {tie.InnerException?.Message}"));
			}
        }
    }
}
