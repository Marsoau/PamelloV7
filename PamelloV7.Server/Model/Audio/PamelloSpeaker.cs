using Discord.Audio;
using Discord.WebSocket;

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

        public bool IsActive {
            get => _audioOutput is not null;
        }

        public PamelloSpeaker(IServiceProvider services,
            DiscordSocketClient client,
            ulong guildId
        ) {
            Client = client;
            Guild = client.GetGuild(guildId);

            Client.VoiceServerUpdated += Client_VoiceServerUpdated;
        }

        private async Task Client_VoiceServerUpdated(SocketVoiceServer voiceServer) {
            //Console.WriteLine($"voice server changed, audio client: {Guild.AudioClient?.ConnectionState.ToString() ?? "No audio client"}; Audio stream is not null: {_audioOutput is not null};");
            if (Guild.AudioClient is not null) {
                Guild.AudioClient.Connected += AudioClient_Connected;
                return;
            }

            if (Voice is not null) await Voice.DisconnectAsync();
            _audioOutput = null;
        }

        private Task AudioClient_Connected() {
            _audioOutput = Guild.AudioClient.CreatePCMStream(AudioApplication.Music);
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

                if (Voice is not null) await Voice.DisconnectAsync();
                _audioOutput = null;
            }
        }
    }
}
