using Discord.WebSocket;
using PamelloV7.Server.Model.Audio;

namespace PamelloV7.Server.Services
{
    public class PamelloSpeakerService
    {
        private readonly IServiceProvider _services;

        private readonly DiscordClientService _discordClients;

        private readonly List<PamelloSpeaker> _speakers;

        public PamelloSpeakerService(IServiceProvider services) {
            _services = services;

            _discordClients = services.GetRequiredService<DiscordClientService>();

            _speakers = new List<PamelloSpeaker>();
        }

        private async Task<bool> AddSpeaker(DiscordSocketClient client, int channel, ulong guildId, ulong vcId) {
            var guild = client.GetGuild(guildId);
            if (guild is null) return false;

            var speaker = new PamelloSpeaker(_services, client, guild.Id, channel);
            await speaker.InitialConnect(vcId);

            _speakers.Add(speaker);
            return true;
        }

        public async Task<bool> ConnectSpeaker(PamelloPlayer player, ulong guildId, ulong vcId) {
            bool clientAvailable;
            foreach (var speakerClient in _discordClients.DiscordClients) {
                clientAvailable = true;

                foreach (var speaker in _speakers) {
                    if (speaker.Client.CurrentUser.Id == speakerClient.CurrentUser.Id && speaker.Guild.Id == guildId) {
                        clientAvailable = false;
                        break;
                    }
                }

                if (clientAvailable) {
                    if (await AddSpeaker(speakerClient, player.Id, guildId, vcId)) return true;
                }
            }

            return false;
        }

        public bool IsChannelActive(int channel) {
            foreach (var speaker in _speakers) {
                if (speaker.Channel == channel && speaker.IsActive) {
                    return true;
                }
            }

            return false;
        }

        public async Task BroadcastBytes(int channel, byte[] audio) {
            foreach (var speaker in _speakers) {
                if (speaker.Channel == channel) {
                    await speaker.PlayBytesAsync(audio);
                }
            }
        }
    }
}
