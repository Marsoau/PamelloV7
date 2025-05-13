namespace PamelloV7.DAL.Entity
{
    public class DatabasePlaylist : DatabaseEntity
	{
        public string Name { get; set; }
        public DatabaseUser Owner { get; set; }
        public bool IsProtected { get; set; }

        public List<DatabasePlaylistEntry> Entries { get; set; }
        public List<DatabaseUser> FavoriteBy { get; set; }
	}
}
