using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Builders;

public class ErrorMessageBuilder : DiscordComponentBuilder
{
    private int _seconds;
    private Task? _countdownTask;
    
    public async Task Countdown() {
        while (_seconds > 0) {
            await Task.Delay(1000);
            _seconds--;
            
            await Message.Refresh();
        }
    }
    
    public IMessageComponentProperties Build(string header, string message, int seconds) {
        if (_countdownTask is null) {
            _seconds = seconds;
            _countdownTask = Task.Run(Countdown);
        }
        
        return BasicComponentsBuilder.Info(header,
            $"""
            {message}
            -# Deletes in {_seconds} seconds
            """
        );
    }
}
