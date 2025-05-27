using Microsoft.EntityFrameworkCore;
using PamelloV7.DAL.Entity;

namespace PamelloV7.DAL
{
    public class DatabaseContext : DbContext
    {
        public DbSet<DatabaseUser> Users { get; set; }
        public DbSet<DatabaseSong> Songs { get; set; }
        public DbSet<DatabaseEpisode> Episodes { get; set; }
        public DbSet<DatabasePlaylist> Playlists { get; set; }
        
        public DbSet<DatabasePlaylistEntry> PlaylistEntries { get; set; }
        public DbSet<DatabaseAssociation> Associations { get; set; }
        
        private readonly string _dataPath;

        public DatabaseContext(string dataPath) {
            _dataPath = dataPath;
            
            if (!Directory.Exists(_dataPath)) {
                Directory.CreateDirectory(_dataPath);
            }

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite(@$"Data Source={_dataPath}/data.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<DatabaseUser>(userBuilder => {
                userBuilder.HasMany(user => user.AddedSongs)
                    .WithOne(song => song.AddedBy);
                userBuilder.HasMany(user => user.FavoriteSongs)
                    .WithMany(song => song.FavoriteBy)
                    .UsingEntity(entity => {
                        entity.Property("FavoriteById").HasColumnName("UserId");
                        entity.Property("FavoriteSongsId").HasColumnName("SongId");
                        entity.ToTable("UsersFavoriteSongs");
                    });

                userBuilder.HasMany(user => user.OwnedPlaylists)
                    .WithOne(playlist => playlist.Owner);
                userBuilder.HasMany(user => user.FavoritePlaylists)
                    .WithMany(playlist => playlist.FavoriteBy)
                    .UsingEntity(entity => {
                        entity.Property("FavoriteById").HasColumnName("UserId");
                        entity.Property("FavoritePlaylistsId").HasColumnName("PlaylistId");
                        entity.ToTable("UsersFavoritePlaylists");
                    });
            });
            modelBuilder.Entity<DatabaseSong>(songBuilder => {
                songBuilder.HasMany(song => song.Episodes)
                    .WithOne(episode => episode.Song);
                songBuilder.HasMany(song => song.Associations)
                    .WithOne(association => association.Song);
            });
            
            modelBuilder.Entity<DatabasePlaylistEntry>(entryBuilder => {
                entryBuilder.HasKey(entry => entry.Id);
                entryBuilder.HasOne<DatabasePlaylist>()
                    .WithMany(playlist => playlist.Entries)
                    .HasForeignKey(entry => entry.PlaylistId);
                entryBuilder.HasOne<DatabaseSong>()
                    .WithMany(song => song.PlaylistEntries)
                    .HasForeignKey(entry => entry.SongId);
            });
            
            modelBuilder.Entity<DatabaseAssociation>(associationBuilder => {
                associationBuilder.HasKey(association => association.Association);
            });
        }
    }
}
