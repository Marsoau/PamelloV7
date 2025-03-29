using PamelloV7.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Client.Components
{
    public partial class PlayerComponent : IRefrashable
    {
        public async Task Update() {
            Player = await _pamello.Players.Get(PlayerId);

            Refresh();
        }

        public void Refresh() {
            RefreshPlayerName();
            RefreshSelection();
        }

        private void RefreshPlayerName() {
            TextBlock_PlayerName.Text = Player?.Name;
        }

        private void RefreshSelection() {
            if (PlayerId == _pamello.Users.Current.SelectedPlayerId) {
                PaintBlack();
            }
            else {
                PaintWhite();
            }
        }
    }
}
