using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Wrapper;
using PamelloV7.Wrapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PamelloV7.Client.Components
{
    /// <summary>
    /// Interaction logic for SongComponent.xaml
    /// </summary>
    public partial class SongComponent : UserControl
    {
        private readonly IServiceProvider _services;

        private readonly PamelloClient _pamello;

        public int SongId { get; }
        public RemoteSong? Song { get; private set; }

        public SongComponent(IServiceProvider services, int songId) {
            _services = services;

            _pamello = _services.GetRequiredService<PamelloClient>();

            SongId = songId;

            InitializeComponent();

            SubscribeToEvents();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e) {
            await Update();
        }

        private async void MenuItem_AddToQueue_Click(object sender, RoutedEventArgs e) {
            await _pamello.Commands.PlayerQueueSongAdd(SongId.ToString());
        }

        private async void MenuItem_FavoriteAdd_Click(object sender, RoutedEventArgs e) {
            if (_pamello.Users.Current.FavoriteSongsIds.Contains(SongId)) {
                await _pamello.Commands.SongFavoriteRemove(SongId.ToString());
            }
            else {
                await _pamello.Commands.SongFavoriteAdd(SongId.ToString());
            }
        }
    }
}
