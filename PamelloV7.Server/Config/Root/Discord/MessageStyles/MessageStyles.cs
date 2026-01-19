using PamelloV7.Core.Config;

namespace PamelloV7.Server.Config.Root.Discord.MessageStyles;

public class MessageStyles : IConfigNode
{
    public MessageStyle Info { get; set; } = new MessageStyle() {
        Color = "#A795AC"
    };
    public MessageStyle Error { get; set; } = new MessageStyle() {
        Color = "#484848"
    };
    public MessageStyle Exception { get; set; } = new MessageStyle() {
        Color = "#FF3030"
    };

    public void EnsureRight() {
        Info.EnsureRight();
        Error.EnsureRight();
        Exception.EnsureRight();
    }
}