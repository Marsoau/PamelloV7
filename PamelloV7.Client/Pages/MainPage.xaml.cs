using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Client.Windows;
using PamelloV7.Core.Audio;
using PamelloV7.Core.DTO;
using PamelloV7.Wrapper;
using PamelloV7.Wrapper.Model;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PamelloV7.Client.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly IServiceProvider _services;

        private readonly PamelloClient _pamello;
        private readonly MainWindow _mainWindow;

        private RemotePlayer? _player;
        private RemoteSong? _song;

        public MainPage(IServiceProvider services) {
            _services = services;

            _pamello = _services.GetRequiredService<PamelloClient>();

            _mainWindow = _services.GetRequiredService<MainWindow>();

            InitializeComponent();

            SubscribeToEvents();
        }



        private async void Page_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            await Update();
        }

        public async Task Update() {
            await UpdatePlayer();
            await UpdateSong();

            Refresh();
        }

        private async Task UpdatePlayer() {
            if (_pamello.Users.Current?.SelectedPlayerId is null) {
                _player = null;
            }
            else {
                _player = await _pamello.Players.GetNew(_pamello.Users.Current.SelectedPlayerId.Value, true);
            }

            Console.WriteLine("player updated");
        }
        private async Task UpdateSong() {
            if (_player is null || _player.CurrentSongId is null) {
                _song = null;
            }
            else {
                _song = await _pamello.Songs.GetNew(_player.CurrentSongId.Value, true);
            }

            Console.WriteLine($"song updated to {_song?.Name ?? "none-"} ({_player?.CurrentSongId ?? -100})");
        }

        private async void Button_Skip_Click(object sender, System.Windows.RoutedEventArgs e) {
            await _pamello.Commands.PlayerSkip();
        }

        private async void Button_Previous_Click(object sender, System.Windows.RoutedEventArgs e) {

        }

        private async void Button_ResumePause_Click(object sender, System.Windows.RoutedEventArgs e) {
            if (_player is null) return;

            if (_player.IsPaused) {
                await _pamello.Commands.PlayerResume();
            }
            else {
                await _pamello.Commands.PlayerPause();
            }
        }

        private async void Button_Next_Click(object sender, System.Windows.RoutedEventArgs e) {

        }

        private async void CheckBox_Random_Click(object sender, System.Windows.RoutedEventArgs e) {
            RefreshPlayerQueueIsRandom();

            if (_player is null) return;
            await _pamello.Commands.PlayerQueueRandom(!_player.QueueIsRandom);
        }

        private async void CheckBox_Reversed_Click(object sender, System.Windows.RoutedEventArgs e) {
            RefreshPlayerQueueIsReversed();

            if (_player is null) return;
            await _pamello.Commands.PlayerQueueReversed(!_player.QueueIsReversed);
        }

        private async void CheckBox_NoLeftovers_Click(object sender, System.Windows.RoutedEventArgs e) {
            RefreshPlayerQueueIsNoLeftovers();

            if (_player is null) return;
            await _pamello.Commands.PlayerQueueNoLeftovers(!_player.QueueIsNoLeftovers);
        }

        private async void CheckBox_FeedRandom_Click(object sender, System.Windows.RoutedEventArgs e) {
            RefreshPlayerQueueIsFeedRandom();

            if (_player is null) return;
            await _pamello.Commands.PlayerQueueFeedRandom(!_player.QueueIsFeedRandom);
        }

        private void Grid_User_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            var userPage = _mainWindow.SwitchPage<UserPage>();
            userPage.User = _pamello.Users.Current;
        }

        private async void Button_ConnectSpeaker_Click(object sender, System.Windows.RoutedEventArgs e) {
            await _pamello.Commands.SpeakerConnect();
        }

        private async void TextBlock_CurrentSongAddedBy_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            var userPage = _mainWindow.SwitchPage<UserPage>();

            var currentEntry = _player?.QueueEntriesDTOs.ElementAtOrDefault(_player.QueuePosition);
            if (currentEntry is null) return;

            await userPage.Update(currentEntry.AdderId);
        }

        private async void Slider_CurrentSongTime_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e) {
        }
    }
}
