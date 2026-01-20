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
    [Route("Command/{commandName}")]
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

        [HttpGet]
        public async Task<IActionResult> Get(string commandName) {
            RequireUser();
            StaticLogger.Log("Command Start");

            var requestedCommandType = _typeResolver.GetByName(commandName);
            if (requestedCommandType is null || !requestedCommandType.IsAssignableTo(typeof(PamelloCommand)))
                throw new PamelloControllerException(NotFound($"command with name \"{commandName}\" not found"));

            var command = _commands.Get(requestedCommandType, User);
            
            var commandMethod = command.GetType().GetMethod("Execute");
            if (commandMethod is null) throw new PamelloControllerException(NotFound($"command with name \"{commandName}\" doesnt have execution method"));
            
            var parameters = commandMethod.GetParameters();
            var args = new object?[parameters.Length];
            
            for (var i = 0; i < parameters.Length; i++) {
                var parameter = parameters[i];
                
                if (!Request.Query.TryGetValue(parameter.Name ?? "", out var queryStringValues)) {
                    //throw new PamelloControllerException(BadRequest($"couldnt find required \"{argName}\""));
                }
                var queryStringValue = queryStringValues.FirstOrDefault() ?? "";

                var argumentType = parameter.ParameterType;

                var isEntityType = false;
                var isManyEntity = false;

                if (argumentType.IsAssignableTo(typeof(IPamelloEntity))) {
                    isEntityType = true;
                    Console.WriteLine($"Pamello Type Argument \"{parameter.Name}\": {argumentType.Name}");
                }
                else if (argumentType.IsGenericType && argumentType.GetGenericTypeDefinition() == typeof(List<>)) {
                    argumentType = argumentType.GenericTypeArguments.FirstOrDefault();
                    if (argumentType is null) throw new PamelloException("Argument of a command has null generic list");
                    
                    if (argumentType.IsAssignableTo(typeof(IPamelloEntity))) {
                        isEntityType = true;
                        isManyEntity = true;
                        Console.WriteLine($"Pamello Multi Type Argument \"{parameter.Name}\": {argumentType.Name}");
                    }
                }
                else {
                    Console.WriteLine($"Other Type Argument \"{parameter.Name}\": {argumentType.Name}");
                }

                if (!isEntityType) {
                    try {
                        args[i] = TypeDescriptor.GetConverter(argumentType).ConvertFromString(queryStringValue);
                    }
                    catch {
                        throw new PamelloControllerException(BadRequest($"couldnt convert \"{queryStringValue}\" to type \"{argumentType.Name}\""));
                    }
                    continue;
                }

                if (isManyEntity) {
                    args[i] = _peql.Get(argumentType, queryStringValue, User);
                }
                else {
                    args[i] = _peql.GetSingle(argumentType, queryStringValue, User);
                }
            }

            commandMethod.Invoke(command, args);
            StaticLogger.Log("Command End");
            
            try {
                object? result;

                if (typeof(Task).IsAssignableFrom(commandMethod.ReturnType)) {
                    result = await commandMethod.InvokeAsync(command, args);
                }
                else result = commandMethod.Invoke(command, args);

                return Ok(JsonSerializer.Serialize(result, JsonEntitiesFactory.Options));
            }
            catch (TargetInvocationException tie) {
                throw new PamelloControllerException(BadRequest($"Execution of command interrupted by exception, message: {tie.InnerException?.Message}"));
            }
        }

        public async Task<IActionResult> GetOld(string commandName) {
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
                Console.WriteLine(typeof(IPamelloEntity).IsAssignableFrom(argsInfo[i].ParameterType));
                if (typeof(IPamelloEntity).IsAssignableFrom(argsInfo[i].ParameterType)) {
                    try {
                        Console.WriteLine($"cehchink for {argsInfo[i].Name}");
                        if (Nullable.GetUnderlyingType(argsInfo[i].ParameterType) is not null) {
                            Console.WriteLine($"arg {argsInfo[i].Name} is optional");
                        }
                        if (argsInfo[i].ParameterType == typeof(IPamelloUser)) {
                            argValue = await _users.GetByValueRequired(argStringValues.FirstOrDefault() ?? "", User);
                        }
                        else if (argsInfo[i].ParameterType == typeof(IPamelloSong)) {
                            argValue = await _songs.GetByValueRequired(argStringValues.FirstOrDefault() ?? "", User);
                        }
                        else if (argsInfo[i].ParameterType == typeof(IPamelloEpisode)) {
                            argValue = await _episodes.GetByValueRequired(argStringValues.FirstOrDefault() ?? "", User);
                        }
                        else if (argsInfo[i].ParameterType == typeof(IPamelloPlaylist)) {
                            argValue = await _playlists.GetByValueRequired(argStringValues.FirstOrDefault() ?? "", User);
                        }
                        else if (argsInfo[i].ParameterType == typeof(IPamelloPlayer)) {
                            argValue = await _players.GetByValueRequired(argStringValues.FirstOrDefault() ?? "", User);
                        }
                        else if (argsInfo[i].ParameterType == typeof(IPamelloSpeaker)) {
                            argValue = await _speakers.GetByValueRequired(argStringValues.FirstOrDefault() ?? "", User);
                        }
                        /* DISCORD
                        else if (argsInfo[i].ParameterType == typeof(PamelloDiscordSpeaker)) {
                            argValue = await _speakers.GetByValueRequired<PamelloDiscordSpeaker>(argStringValues.FirstOrDefault() ?? "", User);
                        }
                        */
                        else if (argsInfo[i].ParameterType == typeof(IPamelloInternetSpeaker)) {
                            argValue = await _speakers.GetByValueRequired<IPamelloInternetSpeaker>(argStringValues.FirstOrDefault() ?? "", User);
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

                if (typeof(Task).IsAssignableFrom(commandMethod.ReturnType)) {
                    result = null; //await commandMethod.InvokeAsync(User.Commands, args);
                }
                else result = null; //commandMethod.Invoke(User.Commands, args);

                if (result is IPamelloEntity resultEntity) {
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
