﻿using PamelloV7.Wrapper.Services;
using PamelloV7.Core.Exceptions;
using System.Text.Json;
using PamelloV7.Wrapper.Repositories;

namespace PamelloV7.Wrapper
{
    public class PamelloClient
    {
        private readonly HttpClient _http;

        public readonly PamelloEventsService Events;
        public readonly PamelloCommandsService Commands;
        public readonly PamelloAuthorizationService Authorization;

        public readonly RemoteUserRepository Users;
        public readonly RemoteSongRepository Songs;
        public readonly RemotePlayerRepository Players;
        public readonly RemoteEpisodeRepository Episodes;
        public readonly RemotePlaylistRepository Playlists;

        public event Action<string?>? OnLog;

        public string? ServerHost;

        public PamelloClient() {
            _http = new HttpClient();

            Events = new PamelloEventsService(this);
            Commands = new PamelloCommandsService(this);
            Authorization = new PamelloAuthorizationService(this);

            Users = new RemoteUserRepository(this);
            Songs = new RemoteSongRepository(this);
            Players = new RemotePlayerRepository(this);
            Episodes = new RemoteEpisodeRepository(this);
            Playlists = new RemotePlaylistRepository(this);
        }

        internal Task HttpGetAsync(string url, Guid? customToken = null)
            => HttpGetAsync<object?>(url, customToken);
        internal async Task<T> HttpGetAsync<T>(string url, Guid? customToken = null) {
            if (ServerHost is null) throw new PamelloException("ServerHost of PamelloClient wasnt set trying to make a request");

            var request = new HttpRequestMessage(HttpMethod.Get, $"http://{ServerHost}/{url}");
            if (Authorization.UserToken is not null) {
                request.Headers.Add("user", (
                    customToken ??
                    Authorization.UserToken ??
                    throw new PamelloException("UserToken is null")
                ).ToString());
            }

            var response = await _http.SendAsync(request);
            var contentString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                throw new Exception(contentString);
            }

            if (contentString.Length == 0) return default;

            var result = JsonSerializer.Deserialize<T>(response.Content.ReadAsStream());

            return result;
        }

        internal void Log(object obj) {
            OnLog?.Invoke(obj.ToString());
        }

        internal void Cleanup() {
            Episodes.Cleanup();
            Players.Cleanup();
            Playlists.Cleanup();
            Songs.Cleanup();
            Users.Cleanup();
        }
    }
}
