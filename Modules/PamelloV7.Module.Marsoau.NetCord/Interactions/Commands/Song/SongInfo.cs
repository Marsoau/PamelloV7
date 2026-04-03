using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Song;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Song;

[DiscordCommand("song info", "Get info about a song")]
public partial class SongInfo
{
    public async Task Execute(
        [SongDescription] [DefaultQuery("current")] IPamelloSong song
    ) {
        await RespondAsync(() => [
            Builder<BasicComponentsBuilder>().Info(null, $"{Interaction.Id}\n{song.ToDiscordString()}"),
            new ActionRowProperties().AddComponents(
                ModalButton<SongRenameModal, SongRenameModal.Builder>("Rename", ButtonStyle.Secondary,
                    async builder => builder.Build(song),
                    async modal => modal.Submit(song)
                )
            )
        ], () => [song]);
    }
}
