using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Client.Services;
using PamelloV7.Client.Windows;
using PamelloV7.Client.Components;
using PamelloV7.Wrapper;
using System.Windows.Controls;
using System.Windows;

namespace PamelloV7.Client.Pages
{
    /// <summary>
    /// Interaction logic for ConnectionPage.xaml
    /// </summary>
    public partial class ConnectionPage : Page
    {
        private readonly IServiceProvider _services;

        private readonly MainWindow _mainWindow;

        private readonly PamelloClient _pamello;
        private readonly SavedServerService _servers;

        public ConnectionPage(IServiceProvider services) {
            _services = services;

            _mainWindow = services.GetRequiredService<MainWindow>();

            _pamello = services.GetRequiredService<PamelloClient>();
            _servers = services.GetRequiredService<SavedServerService>();

            InitializeComponent();

            UpdateServerList();
        }

        public void UpdateServerList() {
            StackPanel_ServerList.Children.Clear();

            ServerComponent serverComponent;
            foreach (var server in _servers.Servers) {
                serverComponent = new ServerComponent(_services, server);
                serverComponent.Margin = new System.Windows.Thickness(0, 0, 0, 4);

                StackPanel_ServerList.Children.Add(serverComponent);
            }
        }

        internal async Task ServerComponent_Button_Connect_Click(ServerComponent serverSender) {
            foreach (ServerComponent server in StackPanel_ServerList.Children) {
                if (server != serverSender) server.State = Enumerators.EServerComponentState.Disabled;
            }

            serverSender.State = Enumerators.EServerComponentState.Connecting;

            if (await _pamello.Events.Connect(serverSender.Server.Host)) {
                Console.WriteLine("connected");

                var authorizationPage = _services.GetRequiredService<AuthorizationPage>();

                authorizationPage.Server = serverSender.Server;
            }
            else {
                Console.WriteLine("noconnection((");
            }

            foreach (ServerComponent server in StackPanel_ServerList.Children) {
                server.State = Enumerators.EServerComponentState.Idle;
            }
        }
    }
}
