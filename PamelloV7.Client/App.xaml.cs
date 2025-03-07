using Microsoft.Extensions.DependencyInjection;
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
        private PamelloClient _client;

        private async void Application_Startup(object sender, StartupEventArgs e) {
            _client = new PamelloClient();

            _client.Events.OnConnection += Events_OnConnection;
            _client.OnAuthorized += Client_OnAuthorized;

            await _client.Connect("127.0.0.1:51630");
        }

        private async Task Events_OnConnection() {
            try {
                await _client.Authorize(Guid.Parse("d01e6353-2ec7-469c-81a5-d3084fb17151"));
            }
            catch (Exception x) {
                Console.WriteLine($"error while authorizing: {x.Message}");
            }
        }

        private void Client_OnAuthorized() {
            Console.WriteLine($"Authorized as \"{_client.Users.CurrentUser.Name}\"");
        }
    }
}
