using PamelloV7.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PamelloV7.Client.Components
{
    public partial class SongComponent : IRefrashable
    {
        public async Task Update() {
            Song = await _pamello.Songs.Get(SongId);

            Refresh();
        }

        public void Refresh() {
            RefreshSongName();
            RefreshSongCover();
            RefreshFavoriteState();
        }

        public void RefreshSongName() {
            Dispatcher.Invoke(() => {
                TextBlock_SongName.Text = Song?.Name;
            });
        }

        public void RefreshSongCover() {
            if (Song is null || Song.CoverUrl is null || Song.CoverUrl.Length == 0) return;

            Dispatcher.Invoke(() => {
                Image_Cover.Source = new BitmapImage(new Uri(Song.CoverUrl));
            });
        }

        public void RefreshFavoriteState() {
            Dispatcher.Invoke(() => {
                if (_pamello.Users.Current.FavoriteSongsIds.Contains(SongId)) {
                    MenuItem_FavoriteAdd.Header = "Remove from Favorite";
                    TextBlock_FavoriteIcon.Visibility = System.Windows.Visibility.Visible;
                }
                else {
                    MenuItem_FavoriteAdd.Header = "Add to Favorite";
                    TextBlock_FavoriteIcon.Visibility = System.Windows.Visibility.Collapsed;
                }
            });
        }
    }
}
