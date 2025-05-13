namespace PamelloV7.DAL.Entity;

public class DatabasePlaylistEntry
{
    public int Id { get; set; }
    public int Order { get; set; }
    public int PlaylistId { get; set; }
    public int SongId { get; set; }
}