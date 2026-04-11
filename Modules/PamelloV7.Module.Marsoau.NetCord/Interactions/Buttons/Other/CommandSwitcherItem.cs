using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Other;

public interface ICommandSwitcherItem
{
    public bool IsFollowUp { get; }
    public bool IsShown { get; }
    
    public Task Show();
    public Task Hide();
}

public class CommandSwitcherItem<TCommand> : ICommandSwitcherItem
    where TCommand : DiscordCommand
{
    private readonly DiscordCommandsService _commands;
    
    private readonly DiscordCommand _parentCommand;
    
    public TCommand? Command { get; private set; }
    
    private Func<TCommand, Task> _show;
    
    public bool IsFollowUp { get; }
    public bool IsShown => Command is not null;
    
    public CommandSwitcherItem(
        DiscordCommandsService commands,
        DiscordCommand parentCommand,
        Func<TCommand, Task> show,
        bool isFollowUp
    ) {
        _commands = commands;
        _parentCommand = parentCommand;
        _show = show;
        
        IsFollowUp = isFollowUp;
    }
    
    public async Task Show() {
        if (Command != null) return;

        Command = await _commands.GetAsync<TCommand>(_parentCommand.Interaction, _parentCommand, IsFollowUp);
        
        await _show(Command);
    }
    public async Task Hide() {
        if (Command is null) return;

        var command = Command;
        Command = null;
        
        await command.DisposeAsync();
    }
};

