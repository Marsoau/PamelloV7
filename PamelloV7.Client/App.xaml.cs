using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Client.Model;
using PamelloV7.Client.Pages;
using PamelloV7.Client.Services;
using PamelloV7.Client.Windows;
using PamelloV7.Core;
using PamelloV7.Core.Events;
using PamelloV7.Wrapper;
using PamelloV7.Wrapper.Services;
using System.Configuration;
using System.Data;
using System.Net;
using System.Text;
using System.Windows;

namespace PamelloV7.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private IServiceProvider _services;

        private async void Application_Startup(object sender, StartupEventArgs e) {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<MainWindow>();

            serviceCollection.AddSingleton<ConnectionPage>();
            serviceCollection.AddSingleton<AuthorizationPage>();
            serviceCollection.AddSingleton<MainPage>();
            serviceCollection.AddSingleton<PlayersPage>();
            serviceCollection.AddSingleton<SettingsPage>();
            serviceCollection.AddSingleton<UserPage>();

            serviceCollection.AddSingleton<PamelloClient>();

            serviceCollection.AddSingleton<SavedServerService>();

            _services = serviceCollection.BuildServiceProvider();

            var servers = _services.GetRequiredService<SavedServerService>();

            var loopback = new SavedServer("Loopback Server", IPEndPoint.Parse("127.0.0.1:51630"));

            loopback.Tokens.Add(Guid.Parse("D01E6353-2EC7-469C-81A5-D3084FB17151"));
            //loopback.Tokens.Add(Guid.Parse("71205227-970C-419A-9205-33FF509C1821"));
            //loopback.Tokens.Add(Guid.Parse("27174539-1498-4ABB-B6EF-F646C93CC200"));
            //loopback.Tokens.Add(Guid.Parse("A3788EA1-BFE5-4E05-ACF2-F88429649938"));

            servers.Add(loopback);
            servers.Add(new SavedServer("Public IP Server", IPEndPoint.Parse("217.97.47.2:51630")));
            servers.Add(new SavedServer("Loopbak 2", IPEndPoint.Parse("127.0.0.1:51630")));

            var mainWindow = _services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            mainWindow.SwitchPage<ConnectionPage>();
            return;
            List<int> before = [1, 2, 3, 4, 5];
            List<int> after = [3, 1, 5, 4, 2, 7];

            var difference = DifferenceResult<int>.From(before, after, true);

            difference.ExcludeMoved();

            Console.WriteLine("added:");
            foreach (var item in difference.Added) {
                Console.WriteLine($"| {item.Value} at {item.Key}");
            }
            Console.WriteLine("moved:");
            foreach (var item in difference.Moved) {
                Console.WriteLine($"| from {item.Key} to {item.Value}");
            }
            Console.WriteLine("changed:");
            foreach (var item in difference.Changed) {
                Console.WriteLine($"| {item.Value} at {item.Key}");
            }
            Console.WriteLine("deleted:");
            foreach (var item in difference.Deleted) {
                Console.WriteLine($"| {item.Value} at {item.Key}");
            }

            difference.Apply(before, value => value);

            Console.WriteLine("after applied:");
            foreach (var item in before) {
                Console.WriteLine($"| {item}");
            }
        }
    }
}
