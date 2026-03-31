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
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Modules;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.Base.Entities;
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
        
        await clients.Main.Rest.BulkOverwriteGuildApplicationCommandsAsync(clients.Main.Id, 1304142495453548646, commands.GetProperties());
        
        interactions.LateStartup();
        
        Output.Write("Started NetCord");
    }
}
