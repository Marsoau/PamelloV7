namespace PamelloV7.DAL.Entity
{
    public class SongEntity
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public string CoverUrl { get; set; }
        public string YoutubeId { get; set; }
        public int PlayCount { get; set; }

        public UserEntity UserAdded { get; set; }
        public List<EpisodeEntity> Episodes { get; set; }
        public List<PlaylistEntity> Playlists { get; set; }
        public List<string> Associacions { get; set; }
	}
}
