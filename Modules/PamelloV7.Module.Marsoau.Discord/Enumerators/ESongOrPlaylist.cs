namespace PamelloV7.Module.Marsoau.Discord.Enumerators;

public enum ESongOrPlaylist
{
    Song,
    Playlist,
}

public static class ESongOrPlaylistExtensions
{
    public static string ToShortString(this ESongOrPlaylist songOrPlaylist)
    {
        return songOrPlaylist switch
        {
            ESongOrPlaylist.Song => "song",
            ESongOrPlaylist.Playlist => "playlist",
            _ => throw new ArgumentOutOfRangeException(nameof(songOrPlaylist), songOrPlaylist, null)
        };
    }
}