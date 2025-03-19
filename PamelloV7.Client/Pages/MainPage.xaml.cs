using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Audio;
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

        private RemotePlayer? _player;
        private RemoteSong? _song;

        public MainPage(IServiceProvider services) {
            _services = services;

            _pamello = _services.GetRequiredService<PamelloClient>();

            InitializeComponent();

            SubscribeToEvents();
        }

        private void SubscribeToEvents() {

            //Song
            _pamello.Events.OnPlayerCurrentSongIdUpdated += Events_OnPlayerCurrentSongIdUpdated;

            _pamello.Events.OnPlayerIsPausedUpdated += Events_OnPlayerIsPausedUpdated;

            _pamello.Events.OnPlayerCurrentSongTimePassedUpdated += Events_OnPlayerCurrentSongTimePassedUpdated;
            _pamello.Events.OnPlayerCurrentSongTimeTotalUpdated += Events_OnPlayerCurrentSongTimeTotalUpdated; ;

            _pamello.Events.OnPlayerStateUpdated += Events_OnPlayerStateUpdated;

            _pamello.Events.OnPlayerQueueIsRandomUpdated += Events_OnPlayerQueueIsRandomUpdated;
            _pamello.Events.OnPlayerQueueIsReversedUpdated += Events_OnPlayerQueueIsReversedUpdated;
            _pamello.Events.OnPlayerQueueIsNoLeftoversUpdated += Events_OnPlayerQueueIsNoLeftoversUpdated;
            _pamello.Events.OnPlayerQueueIsFeedRandomUpdated += Events_OnPlayerQueueIsFeedRandomUpdated;
        }

        private async Task Events_OnPlayerQueueIsFeedRandomUpdated(Core.Events.PlayerQueueIsFeedRandomUpdated arg) {
            Console.WriteLine($"1 {_player.QueueIsFeedRandom}");
            RefreshPlayerQueueIsFeedRandom();
        }
        private async Task Events_OnPlayerQueueIsRandomUpdated(Core.Events.PlayerQueueIsRandomUpdated arg) {
            Console.WriteLine($"2 {_player.QueueIsRandom}");
            RefreshPlayerQueueIsRandom();
        }
        private async Task Events_OnPlayerQueueIsNoLeftoversUpdated(Core.Events.PlayerQueueIsNoLeftoversUpdated arg) {
            Console.WriteLine($"3 {_player.QueueIsNoLeftovers}");
            RefreshPlayerQueueIsNoLeftovers();
        }
        private async Task Events_OnPlayerQueueIsReversedUpdated(Core.Events.PlayerQueueIsReversedUpdated arg) {
            Console.WriteLine($"4 {_player.QueueIsReversed}");
            RefreshPlayerQueueIsReversed();
        }

        private async Task Events_OnPlayerCurrentSongIdUpdated(Core.Events.PlayerCurrentSongIdUpdated arg) {
            await UpdateSong();

            RefreshPlayerCurrentSong();
        }

        private async Task Events_OnPlayerStateUpdated(Core.Events.PlayerStateUpdated arg) {
            RefreshPlayerState();
        }

        private async Task Events_OnPlayerCurrentSongTimeTotalUpdated(Core.Events.PlayerCurrentSongTimeTotalUpdated arg) {
            RefreshPlayerCurrentSongTimeTotal();
        }

        private async Task Events_OnPlayerCurrentSongTimePassedUpdated(Core.Events.PlayerCurrentSongTimePassedUpdated arg) {
            RefreshPlayerCurrentSongTimePassed();
        }

        private async Task Events_OnPlayerIsPausedUpdated(Core.Events.PlayerIsPausedUpdated arg) {
            RefreshPlayerIsPaused();
        }

        public void Refresh() {
            TextBlock_Username.Text = _pamello.Users.Current.Name;
            TextBlock_Userids.Text = $"{_pamello.Users.Current.Id} | {_pamello.Users.Current.DiscordId}";

            Image_Avatar.Source = new BitmapImage(new Uri(_pamello.Users.Current.AvatarUrl ?? ""));

            RefreshPlayer();
        }

        //Refresh Player
        private void RefreshPlayer() {
            RefreshPlayerCurrentSong();

            RefreshPlayerIsPaused();

            RefreshPlayerCurrentSongTimePassed();
            RefreshPlayerCurrentSongTimeTotal();

            RefreshPlayerState();

            RefreshPlayerQueue();
        }

        private void RefreshPlayerIsPaused() {
            Dispatcher.Invoke(() => {
                Button_ResumePause1.Content = (_player?.IsPaused ?? false) ? "R" : "P";
                Button_ResumePause2.Content = (_player?.IsPaused ?? false) ? "R" : "P";
            });
        }
        private void RefreshPlayerState() {
            Dispatcher.Invoke(() => {
                TextBlock_PlayerState.Text = _player?.State.ToString();
            });
        }
        private void RefreshPlayerCurrentSongAddedBy() {

        }
        private void RefreshPlayerCurrentSongTimePassed() {
            var time = new AudioTime(_player?.CurrentSongTimePassed ?? 0);
            var progress = 0.0;
            if (_player is not null) {
                progress = (double)_player.CurrentSongTimePassed / _player.CurrentSongTimeTotal;
                if (double.IsNaN(progress)) progress = 0.0;
            }

            Dispatcher.Invoke(() => {
                TextBlock_CurrentSongTimePassed.Text = time.ToShortString();
                Slider_CurrentSongTime.Value = progress;
            });
        }
        private void RefreshPlayerCurrentSongTimeTotal() {
            var time = new AudioTime(_player?.CurrentSongTimeTotal ?? 0);

            Dispatcher.Invoke(() => {
                TextBlock_CurrentSongTimeTotal.Text = time.ToShortString();
            });
        }

        //Refresh Player Current Song
        private void RefreshPlayerCurrentSong() {
            RefreshPlayerCurrentSongName();
            RefreshPlayerCurrentSongCover();
        }

        private void RefreshPlayerCurrentSongCover() {
            Dispatcher.Invoke(() => {
                Image_CurrentSongCover.Source = new BitmapImage(new Uri(_song?.CoverUrl ?? "https://static.vecteezy.com/system/resources/thumbnails/022/059/000/small_2x/no-image-available-icon-vector.jpg"));
            });
        }
        private void RefreshPlayerCurrentSongName() {
            Dispatcher.Invoke(() => {
                TextBlock_CurrentSongName.Text = _song?.Name;
            });
        }
        
        //Refresh Player Queue
        private void RefreshPlayerQueue() {
            RefreshPlayerQueueModes();
        }

        private void RefreshPlayerQueueModes() {
            RefreshPlayerQueueIsRandom();
            RefreshPlayerQueueIsReversed();
            RefreshPlayerQueueIsNoLeftovers();
            RefreshPlayerQueueIsFeedRandom();
        }

        private void RefreshPlayerQueueIsRandom() {
            Dispatcher.Invoke(() => {
                CheckBox_Random.IsChecked = _player?.QueueIsRandom ?? false;
            });
        }
        private void RefreshPlayerQueueIsReversed() {
            Dispatcher.Invoke(() => {
                CheckBox_Reversed.IsChecked = _player?.QueueIsReversed ?? false;
            });
        }
        private void RefreshPlayerQueueIsNoLeftovers() {
            Dispatcher.Invoke(() => {
                CheckBox_NoLeftovers.IsChecked = _player?.QueueIsNoLeftovers ?? false;
            });
        }
        private void RefreshPlayerQueueIsFeedRandom() {
            Dispatcher.Invoke(() => {
                CheckBox_FeedRandom.IsChecked = _player?.QueueIsFeedRandom ?? false;
            });
        }

        private async void Page_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            await UpdatePlayer();
            await UpdateSong();

            Refresh();
        }

        private async Task UpdatePlayer() {
            if (_pamello.Users.Current?.SelectedPlayerId is null) {
                _player = null;
            }
            else {
                _player = await _pamello.Players.Get(_pamello.Users.Current.SelectedPlayerId.Value);
            }
        }
        private async Task UpdateSong() {
            if (_player is null || _player.CurrentSongId is null) {
                _song = null;
            }
            else {
                _song = await _pamello.Songs.Get(_player.CurrentSongId.Value);
            }

            Console.WriteLine($"song changed to {_song?.Name ?? "none-"}");
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
    }
}
