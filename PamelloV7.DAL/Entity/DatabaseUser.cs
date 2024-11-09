namespace PamelloV7.DAL.Entity
{
    public class DatabaseUser : DatabaseEntity
	{
        public ulong DiscordId { get; set; }
        public Guid Token { get; set; }

        public DateTime JoinedAt { get; set; }
        public int SongsPlayed { get; set; }

        public bool IsAdministrator { get; set; }

        public List<DatabaseSong> AddedSongs { get; set; }
        public List<DatabasePlaylist> OwnedPlaylists { get; set; }
	}
}
