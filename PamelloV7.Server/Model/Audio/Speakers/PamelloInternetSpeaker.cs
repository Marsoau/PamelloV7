using PamelloV7.Server.Model.Listeners;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace PamelloV7.Server.Model.Audio.Speakers
{
    public class PamelloInternetSpeaker : PamelloSpeaker
    {
        private readonly ConcurrentDictionary<int, PamelloInternetSpeakerListener> _listeners;

        private Process? _ffmpeg;

        private MemoryStream _pcmAudioBuffer;
        private MemoryStream _mpegAudioBuffer;

        public int Channel { get; }

        public override bool IsActive => !(_ffmpeg?.HasExited ?? true) && _listeners.Count > 0;

        public PamelloInternetSpeaker(PamelloPlayer player, int channel) : base(player) {
            _listeners = new ConcurrentDictionary<int, PamelloInternetSpeakerListener>();

            Channel = channel;

            _pcmAudioBuffer = new MemoryStream();
            _mpegAudioBuffer = new MemoryStream();
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
                        int read = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (read == 0) break; // FFmpeg closed

                        var data = buffer[..read];

                        foreach (var listener in _listeners) {
                            try {
                                await listener.Value.SendAudio(data);
                            }
                            catch {
                                // Optionally remove dead clients here
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
                if ((Player?.State == Core.Enumerators.EPlayerState.Active && Player.IsPaused == false) || _listeners.Count == 0) {
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

        public async Task<PamelloInternetSpeakerListener> AddListener(HttpResponse response) {
            var listener = new PamelloInternetSpeakerListener(response);
            await listener.InitializeConnecion();

            _listeners.TryAdd(listener.Id, listener);
            listener.OnClosed += Listener_OnClosed;

            return listener;
        }

        private void Listener_OnClosed(PamelloListener listener) {
            _listeners.Remove(listener.Id, out _);
            Console.WriteLine($"isl {listener.Id} removed from <{Channel}>");
        }
        public override Task Terminate() => throw new NotImplementedException();
    }
}
