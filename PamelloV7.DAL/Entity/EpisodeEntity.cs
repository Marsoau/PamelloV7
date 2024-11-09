namespace PamelloV7.DAL.Entity
{
    public class EpisodeEntity
	{
        public int Id { get; set; }
        public SongEntity Song { get; set; }
        public string Name { get; set; }
		public int Start { get; set; }
		public bool Skip { get; set; }
	}
}
