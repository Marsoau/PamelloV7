using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Client.Config;
using PamelloV7.Client.Windows;
using PamelloV7.Core.Audio;
using PamelloV7.Core.DTO;
using PamelloV7.Wrapper;
using PamelloV7.Wrapper.Model;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PamelloV7.Client.Pages
{
    public enum ESearchCategory {
        Songs,
        Playlists,
        Users,
    }
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

        private ESearchCategory _searchCategory;

        private IEnumerable<int> _songsSearchResult;
        private IEnumerable<int> _playlistsSearchResult;
        private IEnumerable<int> _usersSearchResult;

        private int _searchPage;
        private int _searchResultCount {
            get {
                switch (_searchCategory) {
                    case ESearchCategory.Songs:
                        return _songsSearchResult.Count();
                    case ESearchCategory.Playlists:
                        return _playlistsSearchResult.Count();
                    case ESearchCategory.Users:
                        return _usersSearchResult.Count();
                    default:
                        return 0;
                }
            }
        }

        public MainPage(IServiceProvider services) {
            _services = services;

            _pamello = _services.GetRequiredService<PamelloClient>();

            _mainWindow = _services.GetRequiredService<MainWindow>();

            _songsSearchResult = [];
            _playlistsSearchResult = [];
            _usersSearchResult = [];

            InitializeComponent();

            SubscribeToEvents();
        }



        private async void Page_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            Console.WriteLine("ASDAYISGDYUJIHAGSJDGASJDH");

            await Update();
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
            await _pamello.Commands.SpeakerConnectDiscord();
        }

        private async void TextBlock_CurrentSongAddedBy_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            var userPage = _mainWindow.SwitchPage<UserPage>();

            var currentEntry = _player?.QueueEntriesDTOs.ElementAtOrDefault(_player.QueuePosition);
            if (currentEntry is null) return;

            await userPage.Update(currentEntry.AdderId);
        }

        private async void Slider_CurrentSongTime_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e) {
        }

        private async void Button_AddByValue_Click(object sender, System.Windows.RoutedEventArgs e) {
            var value = TextBox_SongValue.Text;
            await _pamello.Commands.PlayerQueueSongAdd(value);
            TextBox_SongValue.Text = null;
        }

        private async void Button_Clear_Click(object sender, System.Windows.RoutedEventArgs e) {
            await _pamello.Commands.PlayerQueueClear();
        }

        private async void Button_Logout_Click(object sender, System.Windows.RoutedEventArgs e) {
            await _pamello.Authorization.Unauthorize();
        }

        private async void TextBox_SongValue_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == System.Windows.Input.Key.Enter) {
                var value = TextBox_SongValue.Text;
                await _pamello.Commands.PlayerQueueSongAdd(value);
                TextBox_SongValue.Text = null;
            }
        }

        private void Button_PlayersMore_Click(object sender, System.Windows.RoutedEventArgs e) {
            _mainWindow.SwitchPage<PlayersPage>();
        }

        private async void Button_EpisodePrevious_Click(object sender, System.Windows.RoutedEventArgs e) {
            await _pamello.Commands.PlayerPrevEpisode();
        }

        private async void Button_EpisodeNext_Click(object sender, System.Windows.RoutedEventArgs e) {
            await _pamello.Commands.PlayerNextEpisode();
        }

        private void Button_Settings_Click(object sender, System.Windows.RoutedEventArgs e) {
            _mainWindow.SwitchPage<SettingsPage>();
        }

        private void ComboBox_SearchCategory_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var selectedItem = (ComboBoxItem)ComboBox_SearchCategory.SelectedItem;
            if (!int.TryParse(selectedItem.Tag.ToString(), out var selectedValue)) return;
            var selectedCategory = (ESearchCategory)selectedValue;

            _searchCategory = selectedCategory;

            Console.WriteLine($"changed search category to: {_searchCategory}");
        }

        private async void Button_Search_Click(object sender, System.Windows.RoutedEventArgs e) {
            await Search(TextBox_SearchQuerry.Text);
        }

        public async Task Search(ESearchCategory category, string querry = "") {
            _searchCategory = category;
            await Search(querry);
        }
        public async Task Search(string querry = "") {
            switch (_searchCategory) {
                case ESearchCategory.Songs:
                    _songsSearchResult = await _pamello.Songs.Search(querry);
                    break;
                case ESearchCategory.Playlists:
                    _playlistsSearchResult = await _pamello.Songs.Search(querry);
                    break;
                case ESearchCategory.Users:
                    _usersSearchResult = await _pamello.Songs.Search(querry);
                    break;
            }

            RefreshSearchResults();
        }

        private void Button_SearchPrevPage_Click(object sender, System.Windows.RoutedEventArgs e) {
            if (_searchPage != 0) _searchPage--;
            RefreshSearchResults();
        }

        private void Button_SearchNextPage_Click(object sender, System.Windows.RoutedEventArgs e) {
            if (_searchResultCount == 0) _searchPage = 0;
            else if (_searchPage < _searchResultCount / PamelloClientConfig.SearchPageSize + (_searchResultCount % PamelloClientConfig.SearchPageSize != 0 ? 1 : 0)) {
                _searchPage++;
            }

            RefreshSearchResults();
        }
    }
}
