using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Gateway;
using NetCord.Gateway.Voice;
using NetCord.Logging;
using NetCord.Rest;
using PamelloV7.Audio.Modules;
using PamelloV7.Framework.Audio.Services;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Modules;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.NetCord.Config;
using PamelloV7.Module.Marsoau.NetCord.Handlers;
using PamelloV7.Module.Marsoau.NetCord.Logger;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord;

public class NetCord : IPamelloModule
{
    public string Name => "NetCord";
    public string Author => "Marsoau";
    public string Description => "NetCord test with DAVE";
    public ELoadingStage Stage => ELoadingStage.Default;
    public IBrush Color => Brushes.MediumTurquoise;

    public async Task StartupAsync(IServiceProvider services) {
        Output.Write("NetCord setup");
        var clients = services.GetRequiredService<DiscordClientService>();
        var interactions = services.GetRequiredService<DiscordInteractionsHandler>();
        var commands = services.GetRequiredService<DiscordCommandsService>();
        
        var properties = commands.GetProperties().ToList();

        Output.Write("Properties:");
        foreach (var property in properties) {
            var message = Output.Write($"{property.Name}");
            
            var options = (property.Options ?? []).ToList();
            if (options.Count == 0) {
                message.ContentBuilder.Append($" : {property.Description}");
                continue;
            }
            
            foreach (var commandOption in options) {
                if (commandOption.Type == ApplicationCommandOptionType.SubCommandGroup) {
                    foreach (var groupOption in commandOption.Options ?? []) {
                        message.ContentBuilder.Append($"\n  {groupOption.Name}");

                        foreach (var subCommandOption in groupOption.Options ?? []) {
                            message.ContentBuilder.Append($"\n    {subCommandOption.Name} : {subCommandOption.Description}");
                        }
                    }
                }
                else if (commandOption.Type == ApplicationCommandOptionType.SubCommand) {
                    foreach (var subCommandOption in commandOption.Options ?? []) {
                        message.ContentBuilder.Append($"\n  {subCommandOption.Name} : {subCommandOption.Description}");
                    }
                }
            }
        }

        //throw new ModuleStartupException(this, "");
        var completions = new List<TaskCompletionSource>();
        var starts = new List<Task>();
        
        foreach (var client in clients.Clients) {
            var completion = new TaskCompletionSource();
            completions.Add(completion);

            client.Ready += _ => {
                completion.SetResult();
                return ValueTask.CompletedTask;
            };
            
            starts.Add(client.StartAsync().AsTask());
        }
        
        await Task.WhenAll(starts);
        await Task.WhenAll(completions.Select(c => c.Task));

        //var ping = await clients.Main.Rest.GetGlobalApplicationCommandAsync(clients.Main.Id, 1304142495453548646);
        //await ping.DeleteAsync();
        
        await clients.Main.Rest.BulkOverwriteGuildApplicationCommandsAsync(clients.Main.Id, 1304142495453548646, properties);

        await clients.AfterStartupAsync();
        interactions.AfterStartup();
        
        Output.Write("Started NetCord");
    }
}
