using PamelloV7.Core.DTO;
using PamelloV7.Wrapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Wrapper.Repositories
{
    public class RemotePlayerRepository : RemoteRepository<RemotePlayer, PamelloPlayerDTO>
    {
        protected override string ControllerName => "Player";

        public RemotePlayerRepository(PamelloClient client) : base(client) {
            SubscribeToEventsDataUpdates();
        }

        internal void SubscribeToEventsDataUpdates() {
            _client.Events.OnPlayerNameUpdated += async (e) => {
                var player = await Get(e.PlayerId, false);
                if (player is not null) player._dto.Name = e.Name;
            };
            _client.Events.OnPlayerNextPositionRequestUpdated += async (e) => {
                var player = await _client.Users.Current.GetSelectedPlayer();
                if (player is not null) player._dto.NextPositionRequest = e.NextPositionRequest;
            };
            _client.Events.OnPlayerProtectionUpdated += async (e) => {
                var player = await _client.Users.Current.GetSelectedPlayer();
                if (player is not null) player._dto.IsProtected = e.IsProtected;
            };
            _client.Events.OnPlayerCurrentSongIdUpdated += async (e) => {
                var player = await _client.Users.Current.GetSelectedPlayer();
                if (player is not null) player._dto.CurrentSongId = e.CurrentSongId;
            };
            _client.Events.OnPlayerCurrentSongTimePassedUpdated += async (e) => {
                var player = await _client.Users.Current.GetSelectedPlayer();
                if (player is not null) player._dto.CurrentSongTimePassed = e.CurrentSongTimePassed;
            };
            _client.Events.OnPlayerCurrentSongTimeTotalUpdated += async (e) => {
                var player = await _client.Users.Current.GetSelectedPlayer();
                if (player is not null) player._dto.CurrentSongTimeTotal = e.CurrentSongTimeTotal;
            };
            _client.Events.OnPlayerIsPausedUpdated += async (e) => {
                var player = await _client.Users.Current.GetSelectedPlayer();
                if (player is not null) player._dto.IsPaused = e.IsPaused;
            };
            _client.Events.OnPlayerQueueIsFeedRandomUpdated += async (e) => {
                var player = await _client.Users.Current.GetSelectedPlayer();
                if (player is not null) player._dto.QueueIsFeedRandom = e.QueueIsFeedRandom;
            };
            _client.Events.OnPlayerQueueIsNoLeftoversUpdated += async (e) => {
                var player = await _client.Users.Current.GetSelectedPlayer();
                if (player is not null) player._dto.QueueIsNoLeftovers = e.IsNoLeftovers;
            };
            _client.Events.OnPlayerQueueIsRandomUpdated += async (e) => {
                var player = await _client.Users.Current.GetSelectedPlayer();
                if (player is not null) player._dto.QueueIsRandom = e.IsRandom;
            };
            _client.Events.OnPlayerQueueIsReversedUpdated += async (e) => {
                var player = await _client.Users.Current.GetSelectedPlayer();
                if (player is not null) player._dto.QueueIsReversed = e.IsReversed;
            };
            _client.Events.OnPlayerQueuePositionUpdated += async (e) => {
                var player = await _client.Users.Current.GetSelectedPlayer();
                if (player is not null) player._dto.QueuePosition = e.QueuePosition;
            };
            _client.Events.OnPlayerQueueEntriesDTOsUpdated += async (e) => {
                var player = await _client.Users.Current.GetSelectedPlayer();
                if (player is not null) player._dto.QueueEntriesDTOs = e.QueueEntriesDTOs;
            };
            _client.Events.OnPlayerStateUpdated += async (e) => {
                var player = await _client.Users.Current.GetSelectedPlayer();
                if (player is not null) player._dto.State = e.State;
            };
        }

        protected override Task<PamelloPlayerDTO?> GetDTO(int id)
            => _client.HttpGetAsync<PamelloPlayerDTO>($"Data/Player?id={id}");
        protected override Task<PamelloPlayerDTO?> GetDTO(string value)
            => _client.HttpGetAsync<PamelloPlayerDTO>($"Data/Player?value={value}");
        protected override RemotePlayer CreateRemoteEntity(PamelloPlayerDTO dto)
            => new RemotePlayer(dto, _client);
    }
}
