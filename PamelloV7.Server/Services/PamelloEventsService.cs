using PamelloV7.Server.Model;
using PamelloV7.Core.Enumerators;

namespace PamelloV7.Server.Services
{
    public class PamelloEventsService
    {
        public event Func<PamelloUser, Task>? OnUserCreated;
        public event Func<PamelloUser, Task>? OnUserLoaded;
        public event Func<PamelloUser, Task>? OnUserEdited;

		public event Action<PamelloSong>? OnDownloadStart;
		public event Action<PamelloSong, EDownloadResult>? OnDownloadEnd;
		public event Action<PamelloSong, double>? OnDownloadProggress;

        public void UserCreated(PamelloUser user) {
            Task.Run(() => UserCreatedAsync(user));
        }
        public async Task UserCreatedAsync(PamelloUser user) {
            if (OnUserCreated is not null) await OnUserCreated.Invoke(user);
        }

        public void UserLoaded(PamelloUser user) {
            Task.Run(() => UserLoadedAsync(user));
        }
        public async Task UserLoadedAsync(PamelloUser user) {
            if (OnUserLoaded is not null) await OnUserLoaded.Invoke(user);
        }

        public void UserEdited(PamelloUser user) {
            Task.Run(() => UserEditedAsync(user));
        }
        public async Task UserEditedAsync(PamelloUser user) {
            if (OnUserEdited is not null) await OnUserEdited.Invoke(user);
        }

        public void DownloadStart(PamelloSong song)
            => OnDownloadStart?.Invoke(song);
        public void DownloadProggress(PamelloSong song, double proggress)
            => OnDownloadProggress?.Invoke(song, proggress);
        public void DownloadEnd(PamelloSong song, EDownloadResult result)
            => OnDownloadEnd?.Invoke(song, result);
    }
}
