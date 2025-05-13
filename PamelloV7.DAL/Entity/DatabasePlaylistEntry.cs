namespace PamelloV7.DAL.Entity;

public class DatabasePlaylistEntry
{
    public int Id { get; set; }
    public int Order { get; set; }
    public DatabasePlaylist Playlist { get; set; }
    public DatabaseSong Song { get; set; }
}