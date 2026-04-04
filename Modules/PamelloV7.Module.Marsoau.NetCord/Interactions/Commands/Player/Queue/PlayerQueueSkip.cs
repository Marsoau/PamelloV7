using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Extensions;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Player.Queue;

[DiscordCommand("player queue skip", "Skip current song")]
public partial class PlayerQueueSkip
{
    protected IPamelloSong? OldSong;
    protected IPamelloSong? NewSong;
    
    public async Task Execute() {
        OldSong = Command<Framework.Commands.PlayerQueueSkip>().Execute();
        NewSong = RequiredQueue.CurrentSong;
        
        await RespondAsync(() => [
            Builder<ContainerBuilder>().Build(OldSong, NewSong),
            !Queue?.Entries.Any() ?? false ? null :
            Button("Skip Again", ButtonStyle.Secondary, async () => {
                OldSong = Command<Framework.Commands.PlayerQueueSkip>().Execute();
                NewSong = RequiredQueue.CurrentSong;

                if (UpdatableMessage is not null) await UpdatableMessage.Refresh();
            }).InActionRow()
        ], () => [OldSong, NewSong, SelectedPlayer]);
    }
    
    public class ContainerBuilder : DiscordComponentBuilder
    {
        public ComponentContainerProperties Build(IPamelloSong? oldSong, IPamelloSong? newSong) {
            var container = new ComponentContainerProperties().AddComponents(
                new TextDisplayProperties(oldSong is not null ? $"{DiscordString.Bold("Skipped")} {oldSong.ToDiscordString()}" : "Nothing was skipped")
            );

            if (newSong is not null) {
                container.AddComponents(
                    new ComponentSeparatorProperties(),
                    new TextDisplayProperties($"{DiscordString.Bold("Playing")} {newSong.ToDiscordString()}")
                );
            }

            return container;
        }
    }
}
