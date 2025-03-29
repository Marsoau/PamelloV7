using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Client.Components;
using PamelloV7.Client.Windows;
using PamelloV7.Wrapper;
using PamelloV7.Wrapper.Model;
using System.Windows.Controls;

namespace PamelloV7.Client.Pages
{
    /// <summary>
    /// Interaction logic for PlayersPage.xaml
    /// </summary>
    public partial class PlayersPage : Page
    {
        private readonly IServiceProvider _services;

        private readonly PamelloClient _pamello;

        private readonly MainWindow _mainWindow;

        private IEnumerable<int> _availablePlayersIds;
        private RemotePlayer? _currentPlayer;

        public PlayersPage(IServiceProvider services) {
            _services = services;

            _mainWindow = services.GetRequiredService<MainWindow>();

            _pamello = services.GetRequiredService<PamelloClient>();

            InitializeComponent();

            SubscribeToEvents();
        }

        private void Button_Back_Click(object sender, System.Windows.RoutedEventArgs e) {
            _mainWindow.SwitchPage<MainPage>();
        }

        private void Button_ApplyPlayerName_Click(object sender, System.Windows.RoutedEventArgs e) {

        }

        private async void Page_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            await Update();
        }

        private async void CheckBox_Protection_Click(object sender, System.Windows.RoutedEventArgs e) {
            RefreshOptionsPlayerProtection();

            if (_currentPlayer is null) return;
            await _pamello.Commands.PlayerProtection(!_currentPlayer.IsProtected);
        }

        private async void Button_Create_Click(object sender, System.Windows.RoutedEventArgs e) {
            await _pamello.Commands.PlayerCreate("P");
        }
    }
}
