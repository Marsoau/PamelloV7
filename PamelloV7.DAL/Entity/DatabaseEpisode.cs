namespace PamelloV7.DAL.Entity
{
    public class DatabaseEpisode : DatabaseEntity
	{
        public DatabaseSong Song { get; set; }
        public string Name { get; set; }
		public int Start { get; set; }
		public bool Skip { get; set; }
	}
}
