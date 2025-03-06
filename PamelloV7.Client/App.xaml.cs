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
    public partial class App : Application
    {
        private IServiceProvider _services;

        private async void Application_Startup(object sender, StartupEventArgs e) {
            var client = new PamelloClient();

            var user = await client.Users.Get(1);
            Console.WriteLine($": {user?.Name}");
            await Task.Delay(2000);

            user = await client.Users.Get(2);
            Console.WriteLine($": {user?.Name}");
            await Task.Delay(2000);

            user = await client.Users.Get(5);
            Console.WriteLine($": {user?.Name}");
            await Task.Delay(2000);

            user = await client.Users.Get(7);
            Console.WriteLine($": {user?.Name}");
        }
    }
}
