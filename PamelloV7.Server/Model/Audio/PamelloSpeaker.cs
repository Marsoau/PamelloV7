﻿using Discord.Audio;
using Discord.WebSocket;
using PamelloV7.Server.Model.Discord;

namespace PamelloV7.Server.Model.Audio
{
    public class PamelloSpeaker
    {
        public readonly DiscordSocketClient Client;
        public readonly SocketGuild Guild;
        public SocketVoiceChannel Voice {
            get => Guild.GetUser(Client.CurrentUser.Id).VoiceChannel;
        }

        private AudioOutStream? _audioOutput;

        public int Channel { get; }

        public bool IsActive {
            get => _audioOutput is not null;
        }

        public event Action<PamelloSpeaker>? Terminated;

        public PamelloSpeaker(IServiceProvider services,
            DiscordSocketClient client,
            ulong guildId,
            int channel
        ) {
            Client = client;
            Guild = client.GetGuild(guildId);

            Channel = channel;

            Client.VoiceServerUpdated += Client_VoiceServerUpdated;
            Client.UserVoiceStateUpdated += Client_UserVoiceStateUpdated;
        }

        private async Task Client_UserVoiceStateUpdated(SocketUser user, SocketVoiceState fromVc, SocketVoiceState toVc) {
            if (user.Id != Client.CurrentUser.Id) return;
            Console.WriteLine("> UVSU <");

            if (toVc.VoiceChannel is null) {
                await Terminate();
            }
        }

        private async Task Client_VoiceServerUpdated(SocketVoiceServer voiceServer) {
            if (voiceServer.Guild.Id != Guild.Id) return;
            Console.WriteLine("> VSU <");

            //Console.WriteLine($"voice server changed, audio client: {Guild.AudioClient?.ConnectionState.ToString() ?? "No audio client"}; Audio stream is not null: {_audioOutput is not null};");
            if (Guild.AudioClient is null) {
                await Terminate();
                return;
            }

            Guild.AudioClient.Connected += AudioClient_Connected;
        }

        private Task AudioClient_Connected() {
			Console.WriteLine("creating stream");
            _audioOutput = Guild.AudioClient.CreatePCMStream(AudioApplication.Music);
			Console.WriteLine("creted");
            return Task.CompletedTask;
        }

        public async Task InitialConnect(ulong vcId) {
            var vc = Guild.GetVoiceChannel(vcId);
            if (vc is null) return;

            await vc.ConnectAsync();
        }

        public async Task PlayBytesAsync(byte[] audio) {
            if (_audioOutput is null) return;

            try {
                await _audioOutput.WriteAsync(audio);
            } catch {
                Console.WriteLine("async x");

                await Terminate();
            }
        }

        public async Task Terminate() {
            if (Voice is not null) await Voice.DisconnectAsync();
            _audioOutput = null;

            Terminated?.Invoke(this);
        }

        public string ToDiscordString() {
            return "";
        }
    }
}
