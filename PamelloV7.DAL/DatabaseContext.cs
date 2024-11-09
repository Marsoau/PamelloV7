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

        public DatabaseContext() {
            if (!Directory.Exists("Data")) {
                Directory.CreateDirectory("Data");
            }

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite(@$"Data Source={AppContext.BaseDirectory}Data\data.db");
        }
    }
}
