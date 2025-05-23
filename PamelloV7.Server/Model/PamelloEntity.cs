﻿using PamelloV7.Core.DTO;
using PamelloV7.DAL;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model.Discord;
using PamelloV7.Server.Repositories;
using PamelloV7.Server.Repositories.Database;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Model
{
    public abstract class PamelloEntity<TDatabaseEntity> : IPamelloEntity where TDatabaseEntity : DatabaseEntity
    {
        protected TDatabaseEntity? DatabaseEntity { get; private set; }

        protected readonly IServiceProvider _services;

        protected readonly PamelloEventsService _events;

        protected readonly PamelloSongRepository _songs;
        protected readonly PamelloEpisodeRepository _episodes;
        protected readonly PamelloPlaylistRepository _playlists;
        protected readonly PamelloUserRepository _users;

        public int Id { get; }
        public abstract string Name { get; set; }

        public PamelloEntity(TDatabaseEntity databaseEntity, IServiceProvider services) {
            _services = services;

            _events = services.GetRequiredService<PamelloEventsService>();

            _songs = services.GetRequiredService<PamelloSongRepository>();
            _episodes = services.GetRequiredService<PamelloEpisodeRepository>();
            _playlists = services.GetRequiredService<PamelloPlaylistRepository>();
            _users = services.GetRequiredService<PamelloUserRepository>();

            Id = databaseEntity.Id;

            DatabaseEntity = databaseEntity;
        }

        protected DatabaseContext GetDatabase() => _services.GetRequiredService<DatabaseContext>();
        protected void Save() {
            //var time = DateTime.Now;
            Task.Run(async () => {
                using var db = GetDatabase();

                var entity = GetDatabaseEntity(db);
                await Task.Delay(2000);

                await db.SaveChangesAsync();
            });
            //Console.WriteLine($"task: {DateTime.Now - time}");
        }

        protected abstract void InitBase();
        public void Init() {
            InitBase();
            DatabaseEntity = null;
        }

        public virtual DiscordString ToDiscordString() {
            return DiscordString.Bold(Name) + " " + DiscordString.Code($"[{Id}]");
        }

        public override string ToString() {
            return $"[{Id}] {Name}";
        }

        public abstract IPamelloDTO GetDTO();
        public abstract TDatabaseEntity GetDatabaseEntity(DatabaseContext? db = null);
    }
}
