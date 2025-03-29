using PamelloV7.Client.Interfaces;
using PamelloV7.Client.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PamelloV7.Client.Pages
{
    public partial class PlayersPage : IRefrashable
    {
        public async Task Update() {
            await UpdateCurrentPlayer();
            await UpdateAvailablePlayers();

            Refresh();
        }
        public async Task UpdateCurrentPlayer() {
            _currentPlayer = await _pamello.Users.Current.GetSelectedPlayer();
        }
        public async Task UpdateAvailablePlayers() {
            _availablePlayersIds = await _pamello.Players.Search();
            Console.WriteLine("avialable players:");
            foreach (var playerId in _availablePlayersIds) {
                Console.WriteLine(playerId);
            }
        }

        public void Refresh() {
            RefreshPlayers();
            RefreshOptions();
        }

        private void RefreshPlayers() {
            RefreshCurrentPlayerComponent();
            RefreshAvailablePlayers();
        }
        private void RefreshCurrentPlayerComponent() {
            Dispatcher.Invoke(() => {
                if (_currentPlayer is null) {
                    Grid_CurrentPlayer.Visibility = Visibility.Collapsed;
                    Grid_NoPlayer.Visibility = Visibility.Visible;

                    return;
                }

                Grid_CurrentPlayer.Visibility = Visibility.Visible;
                Grid_NoPlayer.Visibility = Visibility.Collapsed;

                Grid_CurrentPlayer.Children.Clear();
                Grid_CurrentPlayer.Children.Add(new PlayerComponent(_services, _currentPlayer.Id) {
                    Margin = new Thickness(4),
                });
            });
        }
        private void RefreshAvailablePlayers() {
            Dispatcher.Invoke(() => {
                StackPanel_AvailablePlayers.Children.Clear();

                foreach (var playerId in _availablePlayersIds) {
                    if (playerId == _currentPlayer?.Id) continue;
                    StackPanel_AvailablePlayers.Children.Add(new PlayerComponent(_services, playerId) {
                        Margin = new Thickness(2),
                    });
                }
            });
        }

        private void RefreshOptions() {
            RefreshOptionsPlayerProtection();
            RefreshOptionsPlayerName();
            RefreshOptionsState();
        }
        private void RefreshOptionsPlayerName() {
            Dispatcher.Invoke(() => {
                TextBox_PlayerName.Text = _currentPlayer?.Name;
            });
        }
        private void RefreshOptionsPlayerProtection() {
            Dispatcher.Invoke(() => {
                CheckBox_Protection.IsChecked = _currentPlayer?.IsProtected ?? false;
            });
        }
        private void RefreshOptionsState() {
            Dispatcher.Invoke(() => {
                StackPanel_Options.IsEnabled = _currentPlayer?.OwnerId == _pamello.Users.Current.Id;
            });
        }
    }
}
