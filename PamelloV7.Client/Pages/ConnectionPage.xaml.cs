using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Client.Services;
using PamelloV7.Client.Windows;
using PamelloV7.Client.Components;
using PamelloV7.Wrapper;
using System.Windows.Controls;
using System.Windows;
using PamelloV7.Client.Model;
using System.Net;

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

        private SavedServer? _selectedServer;

        private int? _selectedServerIndex;

        public ConnectionPage(IServiceProvider services) {
            _services = services;

            _mainWindow = services.GetRequiredService<MainWindow>();

            _pamello = services.GetRequiredService<PamelloClient>();
            _servers = services.GetRequiredService<SavedServerService>();

            InitializeComponent();

            _servers.OnServersChanged += _servers_OnServersChanged;

            UpdateServerList();
        }

        private async Task _servers_OnServersChanged() {
            UpdateServerList();
        }

        public void UpdateServerList() {
            UnsubscribeFromServerComponentEvents();

            StackPanel_ServerList.Children.Clear();

            ServerComponent serverComponent;
            foreach (var server in _servers.Servers) {
                serverComponent = new ServerComponent(_services, server);
                serverComponent.Margin = new System.Windows.Thickness(2);

                StackPanel_ServerList.Children.Add(serverComponent);
            }

            SubscribeToServerComponentEvents();
        }

        private void SubscribeToServerComponentEvents() {
            foreach (var item in StackPanel_ServerList.Children) {
                if (item is not ServerComponent serverComponent) continue;

                serverComponent.OnConnectionRequested += ServerComponent_OnConnectionRequested;
                serverComponent.OnEditRequested += ServerComponent_OnEditRequested;
            }
        }

        private void UnsubscribeFromServerComponentEvents() {
            foreach (var item in StackPanel_ServerList.Children) {
                if (item is not ServerComponent serverComponent) continue;

                serverComponent.OnConnectionRequested -= ServerComponent_OnConnectionRequested;
            }
        }

        private async Task ServerComponent_OnEditRequested(ServerComponent serverSender) {
            _selectedServer = serverSender.Server;
            _selectedServerIndex = _servers.Servers.IndexOf(_selectedServer);

            RefreshServerEditContent();
        }

        internal async Task ServerComponent_OnConnectionRequested(ServerComponent serverSender) {
            foreach (ServerComponent server in StackPanel_ServerList.Children) {
                if (server != serverSender) server.State = Enumerators.EServerComponentState.Disabled;
            }

            serverSender.State = Enumerators.EServerComponentState.Connecting;

            if (await _pamello.Events.Connect(serverSender.Server.Host.ToString())) {
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

        protected void RefreshServerEditContent() {
            if (_selectedServer is null) {
                StackPanel_EditServer.Visibility = Visibility.Collapsed;
                Button_DiscradEdit.Visibility = Visibility.Collapsed;
            }
            else {
                StackPanel_EditServer.Visibility = Visibility.Visible;
                Button_DiscradEdit.Visibility = Visibility.Visible;

                TextBox_ServerName.Text = _selectedServer.Name;
                TextBox_ServerAddress.Text = _selectedServer.Host?.ToString() ?? "";
            }


            RefreshServerEditButtons();
        }
        protected void RefreshServerEditButtons() {
            if (_selectedServer is null) {
                Button_ConfirmEdit.Content = "New Server";
                return;
            }
            if (_selectedServerIndex is null) {
                Button_ConfirmEdit.Content = "Add";
            }
            else {
                Button_ConfirmEdit.Content = "Save";
            }
        }

        private void Button_ConfirmEdit_Click(object sender, RoutedEventArgs e) {
            if (_selectedServer is null) {
                _selectedServer = new SavedServer("", null);
            }
            else {
                if (_selectedServerIndex is null) _selectedServer.Name = TextBox_ServerName.Text;

                if (IPEndPoint.TryParse(TextBox_ServerAddress.Text, out var ipAddress)) {
                    if (ipAddress.Port == 0) ipAddress.Port = 51630;

                    _selectedServer.Host = ipAddress;

                    if (_selectedServerIndex is null) {
                        _servers.Add(_selectedServer);
                    }
                    else {
                        _selectedServer.Name = TextBox_ServerName.Text;
                        _servers.Servers[_selectedServerIndex.Value] = _selectedServer;
                        _servers.Save();

                        UpdateServerList();
                    }

                    _selectedServer = null;
                }
                else {
                    MessageBox.Show($"Server host format is invalid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            RefreshServerEditContent();
        }

        private void Button_DiscradEdit_Click(object sender, RoutedEventArgs e) {
            _selectedServer = null;

            RefreshServerEditContent();
        }
    }
}
