using Microsoft.Extensions.DependencyInjection;
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
        }

        public override void OnApplyTemplate() {
            TextBlock_SongName = (TextBlock)GetTemplateChild("TextBlock_SongName");
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
            await _pamello.Commands.PlayerQueueSongRequestNext(QueuePosition);
        }
    }
}
