using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;
using DescriptionAttribute = PamelloV7.Module.Marsoau.NetCord.Attributes.DescriptionAttribute;

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
    private readonly IPamelloUserRepository _users;
    
    public List<DiscordCommandDescriptor> CommandsDescriptors { get; private set; } = [];
    
    public DiscordCommandsService(IServiceProvider services) {
        _services = services;
        
        _types = services.GetRequiredService<IAssemblyTypeResolver>();
        _peql = services.GetRequiredService<IEntityQueryService>();
        _users = services.GetRequiredService<IPamelloUserRepository>();
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
        var command = await GetAsync(interaction);
        
        var executeMethod = command.GetType().GetMethod("Execute");
        if (executeMethod is null) throw new PamelloException($"Discord command with interaction name \"{interaction.Data.Name}\" doesnt have execution method");
        
        var options = interaction.Data.Options.FirstOrDefault() switch {
            { Type: ApplicationCommandOptionType.SubCommandGroup } group 
                => group.Options?.FirstOrDefault(o => o.Type == ApplicationCommandOptionType.SubCommand)?.Options,
            { Type: ApplicationCommandOptionType.SubCommand } sub 
                => sub.Options,
            _ => interaction.Data.Options
        } ?? [];

        var message = Output.Write($"Executing command {command.GetType().Name} by {command.ScopeUser}");
        message.Append(options.Count > 0 ? " with options:" : " with no options");

        foreach (var option in options) {
            message.Append($"\n| {option.Name} : {option.Value}");
        }

        var argumentsTasks = executeMethod.GetParameters().Select(async parameter => {
            var attribute = DescriptionAttribute.GetFromParameter(parameter);
            if (attribute is null) throw new PamelloException("Could not create description attribute");
            
            var option = options.FirstOrDefault(o => o.Name == attribute.Name);
            
            object? value = null;

            if (attribute.Parameter.ParameterType.IsAssignableTo(typeof(IPamelloEntity))) {
                var defaultQuery = attribute.Parameter.GetCustomAttribute<DefaultQueryAttribute>();
                var query = option?.Value ?? defaultQuery?.Query ?? "";
                
                if (new NullabilityInfoContext().Create(attribute.Parameter).WriteState == NullabilityState.NotNull) {
                    value = await _peql.ReflectiveGetSingleRequiredAsync(attribute.Parameter.ParameterType, query, command.ScopeUser);
                }
                else {
                    value = await _peql.ReflectiveGetSingleAsync(attribute.Parameter.ParameterType, query, command.ScopeUser);
                }
            }
            else if (
                attribute.Parameter.ParameterType.IsGenericType &&
                attribute.Parameter.ParameterType.GetGenericTypeDefinition() == typeof(List<>) ||
                attribute.Parameter.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                attribute.Parameter.ParameterType.GenericTypeArguments.First().IsAssignableTo(typeof(IPamelloEntity))
            ) {
                var defaultQuery = attribute.Parameter.GetCustomAttribute<DefaultQueryAttribute>();
                var query = option?.Value ?? defaultQuery?.Query ?? "";
                
                value = await _peql.ReflectiveGetAsync(attribute.Parameter.ParameterType.GenericTypeArguments.First(), query, command.ScopeUser);
            }
            else if (option is not null) {
                var converter = TypeDescriptor.GetConverter(attribute.Parameter.ParameterType);
                value = converter.ConvertFromString(option.Value ?? "");
            }
            else {
                if (attribute.Parameter.HasDefaultValue) {
                    value = attribute.Parameter.DefaultValue;
                }
            }
            
            return value;
        }).ToArray();
        
        try {
            await command.WithLoadingAsync(Task.WhenAll(argumentsTasks));
            var arguments = argumentsTasks.Select(task => task.Result).ToArray();

            if (typeof(Task).IsAssignableFrom(executeMethod.ReturnType)) {
                await (dynamic)executeMethod.Invoke(command, arguments)!;
            }
            else {
                executeMethod.Invoke(command, arguments);
            }
        }
        catch (PamelloException pamelloException) {
            await HandlePamelloException(pamelloException);
        }
        
        return;
        
        Task HandlePamelloException(PamelloException pamelloException) {
            Output.Write($"Command execution exception: {pamelloException.Message}", ELogLevel.Error);
            
            return command.RespondComponentAsync([
                command.Builder<BasicComponentsBuilder>().Info("Exception", pamelloException.Message)
            ]);
        }
    }

    public async Task<DiscordCommand> GetAsync(SlashCommandInteraction interaction) {
        var scopeUser = await _users.GetByPlatformKey(new PlatformKey("discord", interaction.User.Id.ToString()), ServerConfig.Root.AllowUserCreation);
        if (scopeUser is null) throw new PamelloException("User could not be found/created");
        
        var fullName = interaction.Data.Name;
        
        if (interaction.Data.Options.FirstOrDefault(option => option.Type == ApplicationCommandOptionType.SubCommand) is { } subCommand) {
            fullName += $" {subCommand.Name}";
        }
        else if (interaction.Data.Options.FirstOrDefault(option =>
                option.Type == ApplicationCommandOptionType.SubCommandGroup) is { } subGroup) {
            var subCommandInGroup = subGroup.Options!.FirstOrDefault(option => option.Type == ApplicationCommandOptionType.SubCommand);
            fullName += $" {subGroup.Name} {subCommandInGroup!.Name}";
        }
        
        var descriptor = CommandsDescriptors.FirstOrDefault(descriptor => descriptor.Attributes.Any(attribute => attribute.Name == fullName));
        if (descriptor is null) throw new PamelloException($"Discord command with interaction name \"{interaction.Data.Name}\" not found");

        if (Activator.CreateInstance(descriptor.Type) is not DiscordCommand command)
            throw new PamelloException($"Discord command with interaction name \"{interaction.Data.Name}\" cannot be null");
        
        command.Initialize(_services, interaction, scopeUser);
        
        return command;
    }

    private IEnumerable<ApplicationCommandOptionProperties> GetCommandParametersOptions(DiscordCommandAttribute attribute) {
        var descriptor = CommandsDescriptors.FirstOrDefault(descriptor => descriptor.Attributes.Contains(attribute));
        if (descriptor is null) throw new PamelloException($"Discord command with attribute \"{attribute.Name}\" not found");

        var parameters = DescriptionAttribute.GetFromParameters(descriptor.Type);
        
        foreach (var parameter in parameters) {
            var type = Type.GetTypeCode(parameter.Parameter.ParameterType) switch {
                TypeCode.Byte or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or
                TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64
                    => ApplicationCommandOptionType.Integer,
                TypeCode.Single or TypeCode.Double or TypeCode.Decimal => ApplicationCommandOptionType.Double,
                TypeCode.Boolean => ApplicationCommandOptionType.Boolean,
                TypeCode.String or TypeCode.Char => ApplicationCommandOptionType.String,
                TypeCode.Object or TypeCode.DateTime => GetForObject(parameter.Parameter.ParameterType),
                _ => throw new PamelloException("Command parameter cannot be empty"),
            };
            
            var properties = new ApplicationCommandOptionProperties(
                type,
                parameter.Name,
                parameter.Description
            ) {
                Required = !parameter.IsOptional
            };

            yield return properties;
        }

        yield break;

        ApplicationCommandOptionType GetForObject(Type type) {
            //todo some object type specific options, like enums, or entities with suggestions
            return ApplicationCommandOptionType.String;
        }
    }
    
    public IEnumerable<SlashCommandProperties> GetProperties() {
        var infos = CommandsDescriptors.SelectMany(descriptor => descriptor.Attributes).Select(attribute => attribute.GetInfo());
        
        var commandGroups = infos.GroupBy(info => info.Command);
        
        foreach (var commandGroup in commandGroups) {
            var commandInfos = commandGroup.ToList();

            var command = new SlashCommandProperties(commandGroup.Key, commandGroup.Key);
            
            var subGroupsGroups = commandInfos.GroupBy(info => info.SubGroup).ToList();
            if (subGroupsGroups.Count == 1 && subGroupsGroups.First() is { Key: null } noSubGroupGroup) {
                var noSubCommandInfo = noSubGroupGroup.FirstOrDefault(info => info.SubCommand is null);
                if (noSubCommandInfo is not null) {
                    if (noSubGroupGroup.Count() > 1) throw new PamelloException("n;0;>1!");
                    
                    command.Description = noSubCommandInfo.Description;
                    command.Options = GetCommandParametersOptions(noSubCommandInfo.Attribute).ToList();
                    
                    yield return command;
                    continue;
                }
                
                var commandOptions = new List<ApplicationCommandOptionProperties>();
                command.Options = commandOptions;

                foreach (var subCommandInfo in noSubGroupGroup) {
                    var subCommand = new ApplicationCommandOptionProperties(
                        ApplicationCommandOptionType.SubCommand,
                        subCommandInfo.SubCommand!,
                        subCommandInfo.Description
                    );
                    
                    subCommand.Options = GetCommandParametersOptions(subCommandInfo.Attribute).ToList();
                    
                    commandOptions.Add(subCommand);
                }
                
                yield return command;
                continue;
            }
            
            var subGroupsOptions = new List<ApplicationCommandOptionProperties>();
            command.Options = subGroupsOptions;
            
            foreach (var subGroupGroup in subGroupsGroups) {
                if (subGroupGroup.Key is null) {
                    throw new PamelloException("n;0+n!;n");
                }
                
                var subGroupCommandOptions = new List<ApplicationCommandOptionProperties>();
                var subGroupProperties = new ApplicationCommandOptionProperties(
                    ApplicationCommandOptionType.SubCommandGroup,
                    subGroupGroup.Key,
                    subGroupGroup.Key
                );
                
                subGroupProperties.Options = subGroupCommandOptions;

                subGroupsOptions.Add(subGroupProperties);

                foreach (var subCommandInfo in subGroupGroup) {
                    var subCommand = new ApplicationCommandOptionProperties(
                        ApplicationCommandOptionType.SubCommand,
                        subCommandInfo.SubCommand!,
                        subCommandInfo.Description
                    );
                    
                    subCommand.Options = GetCommandParametersOptions(subCommandInfo.Attribute).ToList();
                    
                    subGroupCommandOptions.Add(subCommand);
                }
                
                yield return command;
            }
        }
    }
}
