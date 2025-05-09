﻿using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.DTO;
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

        public bool IsCurrent {
            get { return (bool)GetValue(IsCurrentProperty); }
            set { SetValue(IsCurrentProperty, value); }
        }
        public static readonly DependencyProperty IsCurrentProperty = DependencyProperty.RegisterAttached("IsCurrent", typeof(bool), typeof(QueueSongComponent), new PropertyMetadata(false)); 

        public bool IsRequestedNext {
            get { return (bool)GetValue(IsRequestedNextProperty); }
            set { SetValue(IsRequestedNextProperty, value); }
        }
        public static readonly DependencyProperty IsRequestedNextProperty = DependencyProperty.RegisterAttached("IsRequestedNext", typeof(bool), typeof(QueueSongComponent), new PropertyMetadata(false)); 

        private TextBlock TextBlock_SongName;
        private TextBlock TextBlock_FavoriteIcon;

        public int QueuePosition { get; init; }
        public PamelloQueueEntryDTO Entry { get; init; }

        public RemoteSong? Song { get; private set; }
        public RemoteUser? Adder { get; private set; }

        public QueueSongComponent(IServiceProvider services, int queuePosition, PamelloQueueEntryDTO entry) {
            _services = services;

            _pamello = services.GetRequiredService<PamelloClient>();

            QueuePosition = queuePosition;
            Entry = entry;

            InitializeComponent();

            SubscribeToEvents();
        }

        public override void OnApplyTemplate() {
            TextBlock_SongName = (TextBlock)GetTemplateChild("TextBlock_SongName");
            TextBlock_FavoriteIcon = (TextBlock)GetTemplateChild("TextBlock_FavoriteIcon");
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e) {
            await Update();
            Console.WriteLine($"created entry at {QueuePosition}: {IsCurrent}");
        }

        private async void UserControl_MouseUp(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Middle) {
                await _pamello.Commands.PlayerQueueSongRemove(QueuePosition);
            }
            else if  (e.ChangedButton == MouseButton.Left) {
                await _pamello.Commands.PlayerGoTo(QueuePosition, false);
            }
        }

        private async void MenuItem_RequestNext_Click(object sender, RoutedEventArgs e) {
            var player = await _pamello.Users.Current.GetSelectedPlayer();
            if (player is null) return;

            await _pamello.Commands.PlayerQueueSongRequestNext(QueuePosition != player.NextPositionRequest ? QueuePosition : null);
        }

        private async void MenuItem_RequestNow_Click(object sender, RoutedEventArgs e) {
            await _pamello.Commands.PlayerGoTo(QueuePosition, false);
        }

        private async void MenuItem_Remove_Click(object sender, RoutedEventArgs e) {
            await _pamello.Commands.PlayerQueueSongRemove(QueuePosition);
        }

        private async void MenuItem_FavoriteAdd_Click(object sender, RoutedEventArgs e) {
            if (_pamello.Users.Current.FavoriteSongsIds.Contains(Entry.SongId)) {
                await _pamello.Commands.SongFavoriteRemove(Entry.SongId.ToString());
            }
            else {
                await _pamello.Commands.SongFavoriteAdd(Entry.SongId.ToString());
            }
        }
    }
}
