using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Song;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Song.Episode;

[DiscordCommand("song episode list", "View/Manage episodes of a song")]
public partial class SongEpisodeList
{
    public async Task Execute(
        [SongDescription] [DefaultQuery("current")] IPamelloSong song
    ) {
        await RespondPageAsync(page =>
            Builder<Builder>().Build(song, page)
        , () => [song, ..song.Episodes]);
    }

    public class Builder : DiscordComponentBuilder
    {
        public IMessageComponentProperties?[] Build(IPamelloSong song, int page) {
            var container = new ComponentContainerProperties();

            const int pageSize = 5;
            
            var totalPages = song.Episodes.Count / pageSize + (song.Episodes.Count % pageSize > 0 ? 1 : 0);
            if (totalPages == 0) totalPages = 1;

            var itemsOnPage = song.Episodes.Skip(page * pageSize).Take(pageSize).ToList();

            container.AddComponents(
                new TextDisplayProperties($"## Episodes of {song.ToDiscordString()}"),
                new ComponentSeparatorProperties(),
                new ActionRowProperties().AddComponents(
                    Button("Add Episode", ButtonStyle.Primary, () => {}),
                    Button("Reset", ButtonStyle.Secondary, () => {})
                ),
                new ComponentSeparatorProperties()
            );

            if (itemsOnPage.Count > 0) {
                var count = page * pageSize;
                foreach (var episode in itemsOnPage) {
                    container.AddComponents(
                        new ComponentSectionProperties(
                            ModalButton<SongEpisodeEditModal>(count++, "Edit", ButtonStyle.Secondary, [episode])
                        ).AddComponents(
                            new TextDisplayProperties($"{DiscordString.Code(count)} : {episode.ToDiscordString(withSongId: false)}")
                        )
                    );
                }
            }
            else {
                container.AddComponents(
                    new TextDisplayProperties($"-# {DiscordString.Italic("No episodes")}")
                );
            }
            
            container.AddComponents(
                new ComponentSeparatorProperties(),
                new ActionRowProperties().AddComponents(
                    Button("Edit", ButtonStyle.Primary, () => {}),
                    Button("Delete", ButtonStyle.Secondary, () => {}),
                    Button("Rewind", ButtonStyle.Secondary, () => {})
                ),
                new ComponentSeparatorProperties(),
                new ComponentSectionProperties(
                    Button("Clear", ButtonStyle.Secondary, () => {
                        //remove all episodes from a song
                    }).WithDisabled(song.Episodes.Count == 0)
                ).AddComponents(
                    new TextDisplayProperties($"-# Page {page + 1}/{totalPages} ({song.Episodes.Count} episodes)")
                )
            );

            return [
                container,
                Builder<BasicButtonsBuilder>().PageButtons(page, pageSize, song.Episodes.Count),
                Builder<BasicButtonsBuilder>().RefreshButtonRow()
            ];
        }
    }
}
