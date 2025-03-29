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
    /// Interaction logic for PlayerComponent.xaml
    /// </summary>
    public partial class PlayerComponent : UserControl
    {
        private readonly IServiceProvider _services;

        private readonly PamelloClient _pamello;

        public int PlayerId { get; }
        public RemotePlayer? Player { get; private set; }

        private bool _isSelected;

        public PlayerComponent(IServiceProvider services, int playerId) {
            _services = services;

            _pamello = _services.GetRequiredService<PamelloClient>();

            PlayerId = playerId;

            InitializeComponent();

            SubscribeToEvents();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e) {
            await Update();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e) {
            if (_isSelected) {
                Background = new SolidColorBrush(Color.FromRgb(42, 42, 42));
            }
            else {
                Background = new SolidColorBrush(Color.FromRgb(248, 248, 248));
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e) {
            if (_isSelected) {
                Background = new SolidColorBrush(Color.FromRgb(33, 33, 33));
            }
            else {
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            }
        }

        private void PaintWhite() {
            Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            TextBlock_PlayerName.Foreground = new SolidColorBrush(Color.FromRgb(33, 33, 33));
            _isSelected = false;
        }
        private void PaintBlack() {
            Background = new SolidColorBrush(Color.FromRgb(33, 33, 33));
            TextBlock_PlayerName.Foreground = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            _isSelected = true;
        }

        private async void UserControl_MouseUp(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                if (PlayerId == _pamello.Users.Current.SelectedPlayerId) {
                    await _pamello.Commands.PlayerSelect("-");
                }
                else {
                    await _pamello.Commands.PlayerSelect(PlayerId.ToString());
                }
            }
        }
    }
}
