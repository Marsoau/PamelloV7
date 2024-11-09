using Microsoft.EntityFrameworkCore;
using PamelloV7.DAL.Entity;

namespace PamelloV7.DAL
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<SongEntity> Songs { get; set; }
        public DbSet<EpisodeEntity> Episodes { get; set; }
        public DbSet<PlaylistEntity> Playlists { get; set; }

        public DatabaseContext() {
            if (!Directory.Exists("Data")) {
                Directory.CreateDirectory("Data");
            }

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite(@"Data Source=Data\data.db");
        }
    }
}
