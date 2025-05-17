using System.Diagnostics;
using System.Text;
using Discord.Audio;
using Discord.WebSocket;
using PamelloV7.Core.DTO;
using PamelloV7.Core.DTO.Speakers;
using PamelloV7.Server.Model.Audio.Points;
using PamelloV7.Server.Model.Discord;

namespace PamelloV7.Server.Model.Audio.Speakers
{
    public class PamelloDiscordSpeaker : PamelloSpeaker
    {
        public readonly DiscordSocketClient Client;
        public readonly SocketGuild Guild;
        public SocketVoiceChannel Voice {
            get => Guild.GetUser(Client.CurrentUser.Id).VoiceChannel;
        }

        private AudioOutStream? _audioOutput;

        public override string Name { get; }

        public override bool IsActive {
            get => _audioOutput is not null;
        }

        public readonly AudioPushPoint Input;

        public PamelloDiscordSpeaker(IServiceProvider services,
            DiscordSocketClient client,
            ulong guildId,
            PamelloPlayer player
        ) : base(player) {
            Client = client;
            Guild = client.GetGuild(guildId);

            Client.VoiceServerUpdated += Client_VoiceServerUpdated;
            Client.UserVoiceStateUpdated += Client_UserVoiceStateUpdated;
            
            var input = new AudioPushPoint();
            input.Process += PlayBytesAsync;
            Input = input;
        }

        private async Task Client_UserVoiceStateUpdated(SocketUser user, SocketVoiceState fromVc, SocketVoiceState toVc) {
            if (user.Id != Client.CurrentUser.Id) return;
            Console.WriteLine("> UVSU <");

            if (toVc.VoiceChannel is null) await Terminate();
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
            _audioOutput = Guild.AudioClient.CreateDirectPCMStream(AudioApplication.Music);
            Console.WriteLine("creted");
            return Task.CompletedTask;
        }

        public async Task InitialConnect(ulong vcId) {
            var vc = Guild.GetVoiceChannel(vcId);
            if (vc is null) return;

            await vc.ConnectAsync();
        }

        public override async Task PlayBytesAsync(byte[] audio) {
            if (_audioOutput is null) return;

            try {
                await _audioOutput.WriteAsync(audio);
            }
            catch {
                Console.WriteLine("async x");

                await Terminate();
            }
        }

        public override async Task Terminate() {
            if (Voice is not null) await Voice.DisconnectAsync();
            if (_audioOutput is not null) await _audioOutput.DisposeAsync();
            
            _audioOutput = null;

            InvokeOnTerminated();
        }

        public override DiscordString ToDiscordString() {
            return DiscordString.Bold(new DiscordString(Client.CurrentUser) + " " + DiscordString.Code($"[{Id}]"));
        }

        public override IPamelloDTO GetDTO() {
            return new PamelloDiscordSpeakerDTO() {
                Id = Id,
                Name = Name,
            };
        }
    }
}
