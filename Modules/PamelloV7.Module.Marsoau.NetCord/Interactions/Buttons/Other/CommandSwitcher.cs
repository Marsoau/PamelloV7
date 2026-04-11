using Microsoft.Extensions.DependencyInjection;
using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.General;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Other;

public class CommandSwitcher {
    private readonly DiscordCommandsService _commands;
    
    private readonly DiscordCommand _parentCommand;
    
    public Dictionary<string, ICommandSwitcherItem> Items { get; } = [];

    public CommandSwitcher(DiscordCommand parentCommand, IServiceProvider services) {
        _parentCommand = parentCommand;
        _commands = services.GetRequiredService<DiscordCommandsService>();
    }

    public bool StateOf(string key) {
        return Items.GetValueOrDefault(key)?.IsShown ?? false;
    }

    public void Add<TCommand>(string key, Func<TCommand, Task> show, bool isFollowUp = false)
        where TCommand : DiscordCommand {
        Items[key] = new CommandSwitcherItem<TCommand>(_commands, _parentCommand, show, isFollowUp);
    }

    public async Task Toggle(string key) {
        if (StateOf(key)) {
            await HideAll();
        } else {
            await Show(key);
        }
    }

    public async Task Show(string key) {
        var item = Items.GetValueOrDefault(key);
        if (item is null) return;

        var tasks = Items.Values.Where(otherItem => otherItem != item).Select(otherItem => otherItem.Hide());
        await Task.WhenAll(tasks);
        
        await item.Show();
        
        await Refresh();
    }

    public async Task HideAll() {
        var tasks = Items.Values.Select(item => item.Hide());
        await Task.WhenAll(tasks);
        
        await Refresh();
    }

    private async Task Refresh() {
        var refresh = _parentCommand.UpdatableMessage?.Refresh();
        if (refresh is not null) await refresh;
    }
}
