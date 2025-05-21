using PamelloV7.Server.Model.Listeners;
using System.Collections.Concurrent;
using System.Diagnostics;
using PamelloV7.Core.DTO;
using PamelloV7.Core.DTO.Speakers;
using PamelloV7.Server.Model.Audio.Interfaces;
using PamelloV7.Server.Model.Audio.Modules.Basic;
using PamelloV7.Server.Model.Audio.Modules.Inputs;
using PamelloV7.Server.Model.Audio.Modules.Pipes;
using PamelloV7.Server.Model.Audio.Points;
using PamelloV7.Server.Model.Discord;

namespace PamelloV7.Server.Model.Audio.Speakers
{
    public class PamelloInternetSpeaker : PamelloSpeaker, IAudioModuleWithInputs<AudioPushPoint>, IAudioModuleWithModel
    {
        public bool IsPublic { get; set; }

        public string Channel { get; }

        public override bool IsActive => Listeners.Count > 0;

        public int MinInputs => 1;
        public int MaxInputs => 1;
    
        public AudioPushPoint Input;

        public AudioModel Model { get; }
    
        private AudioBuffer _buffer;
        private AudioSilence _silence;
        private AudioChoise _choise;
        private AudioPump _pump;
        private AudioFFmpeg _ffmpeg;
        private AudioCopy _copy;

        public int ListenersCount => _copy.Outputs.Count;
        public List<PamelloInternetSpeakerListener> Listeners
        {
            get => _copy.Outputs.Select(kvp => kvp.Value.FrontPoint?.ParentModule).OfType<PamelloInternetSpeakerListener>().ToList();
        }

        public bool IsDisposed { get; private set; }

        public PamelloInternetSpeaker(PamelloPlayer player, string channel, bool isPublic) : base(player) {
            Channel = channel;

            IsPublic = isPublic;
            
            Model = new AudioModel();
        }

        public void InitModel() {
            Model.AddModules([
                _buffer = new AudioBuffer(48000),
                _silence = new AudioSilence(),
                _choise = new AudioChoise(),
                _pump = new AudioPump(4800),
                _ffmpeg = new AudioFFmpeg(),
                _copy = new AudioCopy()
            ]);
        }
        
        public AudioPushPoint CreateInput() {
            Input = new AudioPushPoint(this);
        
            return Input;
        }

        public void InitModule() {
            // _choise.CreateInput().ConnectBack(_buffer.Output);
            // _choise.CreateInput().ConnectBack(_silence.Output);
            
            Input.ConnectFront(_ffmpeg.Input);
            
            /*
            _pump.Input.ConnectBack(_buffer.Output);
            _pump.Output.ConnectFront(_copy.Input);
            _pump.Condition = () => Task.FromResult(ListenersCount > 0);
            */
            
            _copy.Input.ConnectBack(_ffmpeg.Output);

            // _ = _pump.Start();
        }

        public override string Name { get; }

        public async Task<PamelloInternetSpeakerListener> AddListener(HttpResponse response, CancellationToken cancellationToken, PamelloUser? user) {
            var listener = new PamelloInternetSpeakerListener(response, cancellationToken, user);
            await listener.InitializeConnecion();

            Model.AddModule(listener);
            var output = _copy.CreateOutput();
            output.ConnectFront(listener.Input);
            
            Listeners.Add(listener);
            Console.WriteLine($"Added listener. Total listeners: {Listeners.Count}");

            return listener;
        }

        public override Task PlayBytesAsync(byte[] audio) {
            return Task.CompletedTask;
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
                ListenersCount = Listeners.Count
            };
        }

        public override void Dispose()
        {
            IsDisposed = true;
            
            Input.Dispose();
            Model.Dispose();
        }

        public override ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
