using PamelloV7.Server.Model.Listeners;
using System.Collections.Concurrent;
using System.Diagnostics;
using PamelloV7.Core.DTO;
using PamelloV7.Core.DTO.Speakers;
using PamelloV7.Server.Model.Discord;

namespace PamelloV7.Server.Model.Audio.Speakers
{
    public class PamelloInternetSpeaker : PamelloSpeaker
    {
        private readonly ConcurrentDictionary<int, PamelloInternetSpeakerListener> _listeners;

        private Process? _ffmpeg;

        private MemoryStream _pcmAudioBuffer;
        private MemoryStream _mpegAudioBuffer;
        
        public bool IsPublic { get; set; }

        public string Channel { get; }

        public override bool IsActive => !(_ffmpeg?.HasExited ?? true) && _listeners.Count > 0;

        public PamelloInternetSpeaker(PamelloPlayer player, string channel, bool isPublic) : base(player) {
            _listeners = new ConcurrentDictionary<int, PamelloInternetSpeakerListener>();

            Channel = channel;

            _pcmAudioBuffer = new MemoryStream();
            _mpegAudioBuffer = new MemoryStream();

            IsPublic = isPublic;
        }

        public async Task InitialConnection() {
            _ffmpeg = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = "ffmpeg",
                    Arguments = "-f s16le -ac 2 -ar 48000 -re -i pipe:0 " +
                        "-acodec libmp3lame -b:a 128k -f mp3 pipe:1",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            _ffmpeg.Start();

            _ = Task.Run(async () => {
                var buffer = new byte[4096];
                var stream = _ffmpeg.StandardOutput.BaseStream;

                try {
                    while (true) {
                        var read = await stream.ReadAsync(buffer);
                        if (read == 0) break;

                        var data = buffer[..read];

                        foreach (var listener in _listeners) {
                            try {
                                await listener.Value.SendAudio(data);
                            }
                            catch {
                                //
                            }
                        }
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine($"Stream error: {ex}");
                }
            });
            _ = Task.Run(SlienceFiller);
        }

        public override string Name { get; }

        public override async Task PlayBytesAsync(byte[] audio) {
            await _pcmAudioBuffer.WriteAsync(audio);
            await FlushBuffer();
        }

        private async Task FlushBuffer() {
            if (_ffmpeg is null) return;
            if (_pcmAudioBuffer.Length < 4096) return;

            await _ffmpeg.StandardInput.BaseStream.WriteAsync(_pcmAudioBuffer.ToArray());
            await _ffmpeg.StandardInput.BaseStream.FlushAsync();

            _pcmAudioBuffer.SetLength(0);

            //Console.WriteLine("flush");
        }

        private async Task SlienceFiller() {
            Console.WriteLine("start");
            while (true) {
                if (Player is { State: Core.Enumerators.EPlayerState.Active, IsPaused: false } || _listeners.IsEmpty) {
                    //Console.WriteLine("wait");
                    await Task.Delay(100);
                    continue;
                }
                if (_pcmAudioBuffer.Length != 0) await FlushBuffer();

                //Console.WriteLine("fill");
                await _pcmAudioBuffer.WriteAsync(new byte[4096]);
                await FlushBuffer();
            }
        }

        public async Task<PamelloInternetSpeakerListener> AddListener(HttpResponse response, PamelloUser? user) {
            var listener = new PamelloInternetSpeakerListener(response, user);
            await listener.InitializeConnecion();

            _listeners.TryAdd(listener.Id, listener);
            listener.OnClosed += Listener_OnClosed;

            return listener;
        }

        private void Listener_OnClosed(PamelloListener listener) {
            _listeners.Remove(listener.Id, out _);
            Console.WriteLine($"ISL-{listener.Id} removed from <{Channel}>");
        }

        public override async Task Terminate() {
            if (_ffmpeg is not null) {
                _ffmpeg.Kill();
                _ffmpeg.Dispose();
                _ffmpeg = null;
            }

            InvokeOnTerminated();
        }
        
        public override DiscordString ToDiscordString() {
            return DiscordString.Code($"<{Channel}> [{Id}]");
        }

        public override IPamelloDTO GetDTO() {
            return new PamelloInternetSpeakerDTO() {
                Id = Id,
                Name = Name,
                Channel = Channel,
                IsPublic = IsPublic,
                ListenersCount = _listeners.Count
            };
        }
    }
}
