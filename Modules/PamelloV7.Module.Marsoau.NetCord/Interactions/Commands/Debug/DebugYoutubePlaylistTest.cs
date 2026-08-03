using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Debug;

[DiscordCommand("debug youtube-playlist-test", "Display debug info about youtube playlist")]
public partial class DebugYoutubePlaylistTest
{
    public async Task Execute(
        [Description("playlist-value", "Youtube playlist url")] string playlistValue
    ) {
        var platforms = Services.GetRequiredService<IPlatformService>();
        var youtube = platforms.GetSongPlatform(
            platforms.GetSongPlatformKey(playlistValue)?.Platform ?? throw new PamelloException("Platform not found")
        );

        if (youtube is null) {
            await RespondAsync("Youtube platform not found");
            return;
        }

        var keys = youtube.ValueToKeys(playlistValue);

        var infos = keys.playlistId is not null
            ? await WithLoadingAsync(
                youtube.GetPlaylistSongsKeysAsync(keys.playlistId).ToListAsync().AsTask()
            )
            : [];

        Output.Write("asd");
        
        await RespondAsync(
            $"""
            Playlist keys
            - Song Key: {DiscordString.Code(keys.songId)}
            - Playlist Key: {DiscordString.Code(keys.playlistId)}
            
            keys: {DiscordString.Code(infos.Count)}
            {DiscordString.CodeBlock(string.Join('\n', infos.Select(i => i.ToString())))}
            """
        );
    }
}
