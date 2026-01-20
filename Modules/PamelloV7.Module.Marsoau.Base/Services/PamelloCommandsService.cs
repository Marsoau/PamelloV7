using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Services;

namespace PamelloV7.Module.Marsoau.Base.Services;

public class PamelloCommandsService : IPamelloCommandsService
{
    private readonly IServiceProvider _services;
    
    public List<Type> CommandTypes { get; private set; }
    
    public PamelloCommandsService(IServiceProvider services) {
        _services = services;
    }
    
    public void Load() {
        var typeResolver = _services.GetRequiredService<IAssemblyTypeResolver>();
        
        CommandTypes = typeResolver.GetInheritorsOf<PamelloCommand>().ToList();
        
        foreach (var commandType in CommandTypes) {
            Console.WriteLine($"Command: {commandType.Name}");
        }
    }
    
    public TCommand Get<TCommand>(IPamelloUser scopeUser) where TCommand : PamelloCommand, new() {
        return (TCommand)Get(typeof(TCommand), scopeUser);
    }

    public PamelloCommand Get(Type requestedType, IPamelloUser scopeUser) {
        var commandType = CommandTypes.FirstOrDefault(type => type == requestedType);
        if (commandType is null) throw new PamelloException($"Command {requestedType.Name} not found");
        
        var scopeUserField = typeof(PamelloCommand).GetField("ScopeUser");
        if (scopeUserField is null) throw new PamelloException($"Command {requestedType.Name} does not have ScopeUser field");
        
        var command = Activator.CreateInstance(commandType) as PamelloCommand;
        Debug.Assert(command is not null, "Command cannot be null here");
        
        scopeUserField.SetValue(command, scopeUser);
        
        return command;
    }
}
