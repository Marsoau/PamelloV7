namespace PamelloV7.DAL.Entity
{
    public class UserEntity
	{
        public int Id { get; set; }
        public ulong DiscordId { get; set; }
        public Guid Token { get; set; }

        public int SongsPlayed { get; set; }
        public DateTime JoinedAt { get; set; }

        public bool IsAdministrator { get; set; }

        public List<SongEntity> AddedSongs { get; set; }
        public List<PlaylistEntity> OwnedPlaylists { get; set; }
	}
}
