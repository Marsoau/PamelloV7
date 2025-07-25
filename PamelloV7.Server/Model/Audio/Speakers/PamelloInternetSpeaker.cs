﻿using PamelloV7.Server.Model.Listeners;
using System.Collections.Concurrent;
using System.Diagnostics;
using PamelloV7.Core.Audio;
using PamelloV7.Core.DTO;
using PamelloV7.Core.DTO.Speakers;
using PamelloV7.Core.Model.Audio;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Server.Model.Audio.Interfaces;
using PamelloV7.Server.Model.Audio.Modules.Basic;
using PamelloV7.Server.Model.Audio.Modules.Inputs;
using PamelloV7.Server.Model.Audio.Modules.Pamello;
using PamelloV7.Server.Model.Audio.Modules.Pipes;
using PamelloV7.Server.Model.Audio.Points;
using PamelloV7.Server.Model.Discord;

namespace PamelloV7.Server.Model.Audio.Speakers
{
    public class PamelloInternetSpeaker : PamelloSpeaker, IPamelloInternetSpeaker, IAudioModuleWithInputs<AudioPushPoint>, IAudioModuleWithModel
    {
        public override bool IsActive => Listeners.Count > 0;

        public int MinInputs => 1;
        public int MaxInputs => 1;
    
        public AudioPushPoint Input;

        public override AudioModel ParentModel { get; }
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

        public sealed override string Name { get; set; }

        public override bool IsDisposed { get; protected set; }

        public PamelloInternetSpeaker(
            AudioModel parentModel,
            IPamelloPlayer player,
            string? name
        ) : base(player) {
            ParentModel = parentModel;

            Name = name ?? Guid.NewGuid().ToString();
            
            Model = new AudioModel();
        }

        public void InitModel() {
            Model.AddModules([
                _buffer = new AudioBuffer(Model, 256000),
                _silence = new AudioSilence(Model),
                _choise = new AudioChoise(Model),
                _pump = new AudioPump(Model, 4096),
                _ffmpeg = new AudioFFmpeg(Model),
                _copy = new AudioCopy(Model, true)
            ]);
            
            _choise.CreateInput().ConnectBack(_buffer.Output);
            _choise.CreateInput().ConnectBack(_silence.Output);
            
            _pump.Input.ConnectBack(_choise.Output);
            _pump.Output.ConnectFront(_ffmpeg.Input);
            
            _copy.Input.ConnectBack(_ffmpeg.Output);
        }
        
        public override AudioPushPoint CreateInput() {
            Input = new AudioPushPoint(this);

            Input.Process = ProcessInput;
        
            return Input;
        }

        private async Task<bool> ProcessInput(byte[] audio, bool wait, CancellationToken token)
        {
            if (ListenersCount > 0) return await _buffer.Input.Push(audio, wait, token);
            return false;
        }

        public override void InitModule() {
            _pump.Start();
        }

        public async Task<IPamelloInternetSpeakerListener> AddListener(object responseObject, CancellationToken cancellationToken, IPamelloUser? user) {
            var response = (HttpResponse)responseObject;
            
            var listener = Model.AddModule(new PamelloInternetSpeakerListener(Model, response, cancellationToken, user));
            await listener.InitializeConnection();
            
            var output = _copy.CreateOutput();
            output.ConnectFront(listener.Input);
            
            Listeners.Add(listener);
            Console.WriteLine($"Added listener. Total listeners: {Listeners.Count}");

            return listener;
        }
        
        public override DiscordString ToDiscordString() {
            return DiscordString.Code($"{Name}") + " " + DiscordString.Code($"[{Id}]");
        }

        public override IPamelloDTO GetDto() {
            return new PamelloInternetSpeakerDTO() {
                Id = Id,
                Name = Name,
                ListenersCount = Listeners.Count
            };
        }

        public override void Dispose()
        {
            if (!IsDisposed) IsDisposed = true;
            else return;
            
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
