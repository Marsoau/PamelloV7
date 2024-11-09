namespace PamelloV7.DAL.Entity
{
    public class PlaylistEntity
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public UserEntity Owner { get; set; }
        public bool IsProtected { get; set; }

        public List<SongEntity> Songs { get; set; }
	}
}
