using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Builders.Help.Guides;

public class HelpGuideManagingSongEpisodesBuilder : DiscordComponentBuilder
{
    public IMessageComponentProperties?[] Build() {
        var container = new ComponentContainerProperties();

        container.AddComponents(
            MediaGalleryProperties.Create([
                new MediaGalleryItemProperties(new ComponentMediaProperties("https://storage.marsoau.com/share/readme/videos/pamello-episode-list.mp4"))
            ]),
            new ComponentSeparatorProperties(),
            new TextDisplayProperties(
                """
                ### `/song episode list` `{song?}`: **Brings up a real-time interactive song episodes list**
                > Accepts a song as a parameter, when not specified uses your `current` song
                
                Here you can see all of the song episodes, their start positions, start time, and an episode action button, as well as some general action buttons and mode switch buttons
                ## Modes
                There are 3 modes this command can be in, which determine what each episode action button does:
                - `Edit`: Edits the episode info, like its name, start time, and its auto skip status
                - `Delete`: Deletes the episode from the song
                - `Rewind`: Rewinds the playback to the episode, only available if this is the current song
                
                > The buttons to switch modes are located below the episodes list itself
                ## Other action buttons
                - `Add Episode`: Creates a new episode and adds it to the song
                - `Reset`: Resets the episodes of this song to its selected source
                
                > You can browse pages with the `Prev`, `Page`, `Next` buttons, the `Page` button in particular brings up a modal to select a specific page number
                """
            )
        );
        
        return [
            container
        ];
    }
}