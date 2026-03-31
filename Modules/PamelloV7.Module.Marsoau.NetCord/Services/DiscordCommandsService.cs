using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Services;

public record DiscordCommandDescriptor(
    DiscordCommandAttribute[] Attributes,
    Type Type
);

public class DiscordCommandsService : IPamelloService
{
    private readonly IServiceProvider _services;
    
    private readonly IAssemblyTypeResolver _types;
    private readonly IEntityQueryService _peql;
    
    public List<DiscordCommandDescriptor> CommandsDescriptors { get; private set; } = [];
    
    public DiscordCommandsService(IServiceProvider services) {
        _services = services;
        
        _types = services.GetRequiredService<IAssemblyTypeResolver>();
        _peql = services.GetRequiredService<IEntityQueryService>();
    }

    public void Startup(IServiceProvider services) {
        var types = _types.GetInheritorsOf<DiscordCommand>().ToList();

        foreach (var type in types) {
            var attributes = type.GetCustomAttributes().OfType<DiscordCommandAttribute>().ToArray();
            if (attributes.Length == 0) continue;
            
            CommandsDescriptors.Add(new DiscordCommandDescriptor(
                attributes,
                type
            ));
        }

        Output.Write("Discord commands:");
        foreach (var commandType in CommandsDescriptors) {
            Output.Write($"| {commandType.Type.FullName}");
            foreach (var attribute in commandType.Attributes) {
                Output.Write($"|   {attribute.Name} : {attribute.Description}");
            }
        }
    }

    public async Task ExecuteAsync(SlashCommandInteraction interaction) {
        var command = Get(interaction);
        
        var executeMethod = command.GetType().GetMethod("Execute");
        if (executeMethod is null) throw new PamelloException($"Discord command with interaction name \"{interaction.Data.Name}\" doesnt have execution method");

        if (typeof(Task).IsAssignableFrom(executeMethod.ReturnType)) {
            await (dynamic)executeMethod.Invoke(command, [])!;
        }
        else {
            executeMethod.Invoke(command, []);
        }
    }

    public DiscordCommand Get(SlashCommandInteraction interaction) {
        var descriptor = CommandsDescriptors.FirstOrDefault(descriptor => descriptor.Attributes.Any(attribute => attribute.Name == interaction.Data.Name));
        if (descriptor is null) throw new PamelloException($"Discord command with interaction name \"{interaction.Data.Name}\" not found");

        if (Activator.CreateInstance(descriptor.Type) is not DiscordCommand command)
            throw new PamelloException($"Discord command with interaction name \"{interaction.Data.Name}\" cannot be null");
        
        var servicesField = descriptor.Type.GetField(nameof(DiscordCommand.Services))!;
        var interactionField = descriptor.Type.GetField(nameof(DiscordCommand.Interaction))!;
        
        servicesField.SetValue(command, _services);
        interactionField.SetValue(command, interaction);
        
        return command;
    }
    
    public IEnumerable<ApplicationCommandProperties> GetProperties() {
        var infos = CommandsDescriptors.SelectMany(descriptor => descriptor.Attributes).Select(attribute => attribute.GetInfo());
        
        var commandGroups = infos.GroupBy(info => info.Command);
        
        foreach (var commandGroup in commandGroups) {
            var groupInfos = commandGroup.ToList();
            
            var commandName = commandGroup.Key;
            var isSingleCommand = groupInfos.Count == 1;

            var command = new SlashCommandProperties(commandName, isSingleCommand ? groupInfos[0].Description : commandName);
            if (isSingleCommand) {
                yield return command;
                continue;
            }
        }
    }
}
