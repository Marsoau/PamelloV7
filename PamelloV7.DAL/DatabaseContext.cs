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

        public DbSet<DatabaseAssociation> Associations { get; set; }

        public DatabaseContext() {
            if (!Directory.Exists($"{AppContext.BaseDirectory}Data")) {
                Directory.CreateDirectory($"{AppContext.BaseDirectory}Data");
            }

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite(@$"Data Source={AppContext.BaseDirectory}Data/data.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<DatabaseUser>(userBuilder => {
                userBuilder.HasMany(user => user.AddedSongs)
                    .WithOne(song => song.AddedBy);
                userBuilder.HasMany(user => user.FavoriteSongs)
                    .WithMany(song => song.FavoritedBy)
                    .UsingEntity(entity => {
                        entity.Property("FavoritedById").HasColumnName("UserId");
                        entity.Property("FavoriteSongsId").HasColumnName("SongId");
                        entity.ToTable("UsersFavoriteSongs");
                    });

                userBuilder.HasMany(user => user.OwnedPlaylists)
                    .WithOne(playlist => playlist.Owner);
                userBuilder.HasMany(user => user.FavoritePlaylists)
                    .WithMany(playlist => playlist.FavoritedBy)
                    .UsingEntity(entity => {
                        entity.Property("FavoritedById").HasColumnName("UserId");
                        entity.Property("FavoritePlaylistsId").HasColumnName("PlaylistId");
                        entity.ToTable("UsersFavoritePlaylists");
                    });
            });
            modelBuilder.Entity<DatabaseSong>(songBuilder => {
                songBuilder.HasMany(song => song.Episodes)
                    .WithOne(episode => episode.Song);
                songBuilder.HasMany(song => song.Associations)
                    .WithOne(associacion => associacion.Song);
                songBuilder.HasMany(song => song.Playlists)
                    .WithMany(playlist => playlist.Songs)
                    .UsingEntity(entity => {
                        entity.Property("PlaylistsId").HasColumnName("PlaylistId");
                        entity.Property("SongsId").HasColumnName("SongId");
                        entity.ToTable("PlaylistsSongs");
                    });
            });
            modelBuilder.Entity<DatabaseAssociation>(associacionBuilder => {
                associacionBuilder.HasKey(ascn => ascn.Association);
            });
        }
    }
}
