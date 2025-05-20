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

        public override bool IsActive => Listenets.Count > 0;

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

        public readonly List<PamelloInternetSpeakerListener> Listenets;

        public PamelloInternetSpeaker(PamelloPlayer player, string channel, bool isPublic) : base(player) {
            Channel = channel;

            IsPublic = isPublic;

            Listenets = [];
            
            Model = new AudioModel();
        }

        public void InitModel() {
            Model.AddModules([
                _buffer = new AudioBuffer(48000),
                _silence = new AudioSilence(),
                _choise = new AudioChoise(),
                _pump = new AudioPump(),
                _ffmpeg = new AudioFFmpeg(),
                _copy = new AudioCopy()
            ]);
        }
        
        public AudioPushPoint CreateInput() {
            Input = new AudioPushPoint();
            
            Input.ConnectFront(_ffmpeg.Input);
        
            return Input;
        }

        public void InitModule() {
            // _choise.CreateInput().ConnectBack(_buffer.Output);
            // _choise.CreateInput().ConnectBack(_silence.Output);
            
            _copy.Input.ConnectBack(_ffmpeg.Output);
        
            // _pump.Input.ConnectFront(_buffer.Output);
            // _pump.Output.ConnectBack(_copy.Input);
        }

        public override string Name { get; }

        public async Task<PamelloInternetSpeakerListener> AddListener(HttpResponse response, PamelloUser? user) {
            var listener = new PamelloInternetSpeakerListener(response, user);
            await listener.InitializeConnecion();

            Model.AddModule(listener);
            var output = _copy.CreateOutput();
            output.ConnectFront(listener.Input);
            
            Listenets.Add(listener);
            Console.WriteLine($"Added listener. Total listeners: {Listenets.Count}");

            return listener;
        }

        public override Task PlayBytesAsync(byte[] audio) {
            return Task.CompletedTask;
        }

        public override async Task Terminate() {
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
                ListenersCount = Listenets.Count
            };
        }
    }
}
