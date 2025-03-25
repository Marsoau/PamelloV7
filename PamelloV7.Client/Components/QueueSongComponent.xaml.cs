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
    public partial class QueueSongComponent : UserControl
    {
        private readonly IServiceProvider _services;

        private readonly PamelloClient _pamello;

        public bool IsCurrent { get; set; } = true;

        public int? SongId { get; init; } = null;
        public RemoteSong? Song { get; private set; }

        public QueueSongComponent(IServiceProvider services) {
            _services = services;

            _pamello = services.GetRequiredService<PamelloClient>();

            InitializeComponent();
        }

        public async Task Refresh() {
            if (SongId is null) {
                Song = null;
            }
            else {
                Song = await _pamello.Songs.Get(SongId.Value);
            }

            TextBlock_SongName.Text = Song?.Name;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e) {
            await Refresh();
        }
    }
}
