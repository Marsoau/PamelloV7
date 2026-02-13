using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Audio.Time;
using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Converters;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;

namespace PamelloV7.Module.Marsoau.Base.Services;

public class PamelloCommandsService : IPamelloCommandsService
{
    private readonly IServiceProvider _services;
    
    private readonly IAssemblyTypeResolver _typeResolver;
    private readonly IEntityQueryService _peql;
    
    public List<Type> CommandTypes { get; private set; }
    
    public PamelloCommandsService(IServiceProvider services) {
        _services = services;
        
        _typeResolver = _services.GetRequiredService<IAssemblyTypeResolver>();
        _peql = _services.GetRequiredService<IEntityQueryService>();
    }
    
    public void Load() {
        CommandTypes = _typeResolver.GetInheritorsOf<PamelloCommand>().ToList();
        
        foreach (var commandType in CommandTypes) {
            Console.WriteLine($"Command: {commandType.FullName}");
        }
    }
    
    public TCommand Get<TCommand>(IPamelloUser scopeUser) where TCommand : PamelloCommand, new() {
        return (TCommand)Get(typeof(TCommand), scopeUser);
    }

    public PamelloCommand Get(Type requestedType, IPamelloUser scopeUser) {
        var commandType = CommandTypes.FirstOrDefault(type => type == requestedType);
        if (commandType is null) throw new PamelloException($"Command {requestedType.Name} not found");
        
        var scopeUserField = typeof(PamelloCommand).GetField("ScopeUser");
        var servicesField = typeof(PamelloCommand).GetField("Services");
        if (scopeUserField is null || servicesField is null) throw new PamelloException($"Command {requestedType.Name} should have ScopeUser and Services fields");
        
        var command = Activator.CreateInstance(commandType) as PamelloCommand;
        Debug.Assert(command is not null, "Command cannot be null here");
        
        scopeUserField.SetValue(command, scopeUser);
        servicesField.SetValue(command, _services);
        
        return command;
    }

    public async Task<object?> ExecutePathAsync(string commandPath, IPamelloUser scopeUser) {
        Console.WriteLine($"Executing command path: {commandPath}");
        var split = commandPath.Split('?', StringSplitOptions.RemoveEmptyEntries);
        var commandName = split[0];
        var queryCollection = HttpUtility.ParseQueryString(split.Length > 1 ? split[1] : "");
        var query = queryCollection.AllKeys
            .OfType<string>()
            .ToDictionary(key => key, key => queryCollection[key]);
        
        var requestedCommandType = _typeResolver.GetByName(commandName);
        if (requestedCommandType is null || !requestedCommandType.IsAssignableTo(typeof(PamelloCommand)))
            throw new PamelloException($"command with name \"{commandName}\" not found");

        var command = Get(requestedCommandType, scopeUser);
            
        var commandMethod = command.GetType().GetMethod("Execute");
        if (commandMethod is null) throw new PamelloException($"command with name \"{commandName}\" doesnt have execution method");
            
        var parameters = commandMethod.GetParameters();
        var args = new object?[parameters.Length];
            
        for (var i = 0; i < parameters.Length; i++) {
            var parameter = parameters[i];
                
            if (!query.TryGetValue(parameter.Name!, out var queryStringValue) || queryStringValue is null || queryStringValue.Length == 0) {
                if (!parameter.HasDefaultValue) throw new PamelloException($"couldnt find required \"{parameter.Name}\"");
                
                args[i] = parameter.DefaultValue;
                
                continue;
            }

            var argumentType = parameter.ParameterType;

            var isEntityType = false;
            var isManyEntity = false;
            
            if (argumentType.IsAssignableTo(typeof(IPamelloEntity))) {
                isEntityType = true;
                Console.WriteLine($"Pamello Type Argument \"{parameter.Name}\": {argumentType.Name}");
            }
            else if (argumentType.IsGenericType && argumentType.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
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
                if (argumentType == typeof(AudioTime)) {
                    args[i] = AudioTime.FromStrTime(queryStringValue);
                }
                else {
                    try {
                        args[i] = TypeDescriptor.GetConverter(argumentType).ConvertFromString(queryStringValue);
                    }
                    catch {
                        throw new PamelloException($"couldnt convert \"{queryStringValue}\" to type \"{argumentType.Name}\"");
                    }
                }
                
                continue;
            }

            if (isManyEntity) {
                args[i] = await _peql.ReflectiveGetAsync(argumentType, queryStringValue, scopeUser);
            }
            else {
                args[i] = await _peql.ReflectiveGetSingleAsync(argumentType, queryStringValue, scopeUser);
            }
        }
            
        try {
            object? result;

            if (typeof(Task).IsAssignableFrom(commandMethod.ReturnType)) {
                result = await (dynamic)commandMethod.Invoke(command, args)!;
            }
            else {
                result = commandMethod.Invoke(command, args);
            }

            return result;
        }
        catch (TargetInvocationException tie) {
            throw new PamelloException($"Execution of command interrupted by exception, message: {tie.InnerException?.Message}");
        }
    }
}
