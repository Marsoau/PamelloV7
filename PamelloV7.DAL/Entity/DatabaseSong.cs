﻿namespace PamelloV7.DAL.Entity
{
    public class DatabaseSong : DatabaseEntity
	{
        public string Name { get; set; }
        public string CoverUrl { get; set; }
        public string YoutubeId { get; set; }
        public int PlayCount { get; set; }

        public DateTime AddedAt { get; set; }

        public DatabaseUser AddedBy { get; set; }
        public List<DatabaseUser> FavoriteBy { get; set; }

        public List<DatabaseEpisode> Episodes { get; set; }
        public List<DatabasePlaylistEntry> PlaylistEntries { get; set; }
        public List<DatabaseAssociation> Associations { get; set; }
	}
}
