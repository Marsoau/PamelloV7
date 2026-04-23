using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Gateway;
using NetCord.Gateway.ReconnectStrategies;
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
    public int Color => 0x2D68C4;

    public async Task StartupAsync(IServiceProvider services) {
        Output.Write("NetCord setup");
        var clients = services.GetRequiredService<DiscordClientService>();
        var interactions = services.GetRequiredService<DiscordInteractionsHandler>();
        var commands = services.GetRequiredService<DiscordCommandsService>();
        
        var properties = commands.GetProperties().ToList();

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

        var registers = NetCordConfig.Root.Commands.GuildsIds.Select(id => 
            clients.Main.Rest.BulkOverwriteGuildApplicationCommandsAsync(clients.Main.Id, id, properties)
            //Task.CompletedTask
        );
        
        await Task.WhenAll(registers);

        await clients.AfterStartupAsync();
        interactions.AfterStartup();
        
        Output.Write("Started NetCord");
        return;

        clients.Main.VoiceServerUpdate += async args => {
            Output.Write("Server Update");
            Output.Write($"{args.GuildId}");
        };
        clients.Main.VoiceStateUpdate += async state => {
            Output.Write("VSU");
            Output.Write($"Channel: {state.ChannelId}");
        };

        await clients.Main.UpdateVoiceStateAsync(new VoiceStateProperties(1463545154894823648, null));
        var voiceClient = await clients.Main.JoinVoiceChannelAsync(1463545154894823648, 1463545156451045472);
        
        voiceClient.UserConnect += async args => {
            foreach (var user in args.UserIds) {
                Output.Write($"Id: {user}");
            }
        };
        voiceClient.UserDisconnect += async args => {
            Output.Write($"User Connect: {args.UserId}");
        };

        Output.Write("discord end, 2 second delay");
        await Task.Delay(2000);
    }
}
