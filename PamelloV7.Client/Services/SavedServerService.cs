using PamelloV7.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Client.Services
{
    public class SavedServerService
    {
        public readonly List<SavedServer> Servers;

        public event Func<Task>? OnServersChanged;

        public SavedServerService() {
            Servers = new List<SavedServer>();

            Load();
        }

        private void Load() {
        }

        public async void Add(SavedServer server) {
            Servers.Add(server);

            if (OnServersChanged is not null) await OnServersChanged.Invoke();
        }
        public async void Remove(SavedServer server) {
            Servers.Remove(server);

            if (OnServersChanged is not null) await OnServersChanged.Invoke();
        }

        public void Save() {

        }
    }
}
