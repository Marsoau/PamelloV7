using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Events;
using PamelloV7.Wrapper;
using PamelloV7.Wrapper.Services;
using System.Configuration;
using System.Data;
using System.Text;
using System.Windows;

namespace PamelloV7.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private PamelloClient _pamello;

        private async void Application_Startup(object sender, StartupEventArgs e) {
            _pamello = new PamelloClient();

            _pamello.Events.OnPamelloEvent += Events_OnPamelloEvent;
            _pamello.Events.OnConnection += Events_OnConnection;
            _pamello.OnAuthorized += Client_OnAuthorized;

            await _pamello.Connect("127.0.0.1:51630");
        }

        private async Task Events_OnPamelloEvent(PamelloEvent pamelloEvent) {
            Console.WriteLine($"Recieved event {pamelloEvent}");
        }

        private async Task Events_OnConnection() {
            Console.WriteLine($"Connected to \"{_pamello.ServerHost}\"");

            try {
                await _pamello.Authorize(Guid.Parse("d01e6353-2ec7-469c-81a5-d3084fb17151"));
            }
            catch (Exception x) {
                Console.WriteLine($"error while authorizing: {x.Message}");
            }
        }

        private async Task Client_OnAuthorized() {
            Console.WriteLine($"Authorized as \"{_pamello.Users.Current.Name}\"");
        }
    }
}
