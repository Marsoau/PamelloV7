using Discord.WebSocket;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Repositories;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.Server.Services
{
    public class PamelloSpeakerService
    {
        private readonly IServiceProvider _services;

        private readonly DiscordClientService _discordClients;

        private readonly PamelloPlayerRepository _players;

        private readonly List<PamelloSpeaker> _speakers;

        public PamelloSpeakerService(IServiceProvider services) {
            _services = services;

            _discordClients = services.GetRequiredService<DiscordClientService>();

            _players = services.GetRequiredService<PamelloPlayerRepository>();

            _speakers = new List<PamelloSpeaker>();
        }

        private async Task<bool> AddSpeaker(DiscordSocketClient client, int channel, ulong guildId, ulong vcId) {
            var guild = client.GetGuild(guildId);
            if (guild is null) return false;

            var speaker = new PamelloSpeaker(_services, client, guild.Id, channel);
            await speaker.InitialConnect(vcId);

            speaker.Terminated += Speaker_Terminated;
            _speakers.Add(speaker);

            return true;
        }

        private void Speaker_Terminated(PamelloSpeaker speaker) {
            _speakers.Remove(speaker);
        }

        public async Task ConnectSpeaker(PamelloPlayer player, ulong guildId, ulong vcId) {
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
                    if (await AddSpeaker(speakerClient, player.Id, guildId, vcId)) return;
                }
            }

            throw new PamelloException("No available speakers left");
        }
        public async Task DisconnectSpeaker(PamelloPlayer player, ulong vcId) {
            foreach (var speaker in _speakers) {
                if (speaker.Channel == player.Id && speaker.Voice.Id == vcId) {
                    await speaker.Terminate();
                    return;
                }
            }
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

        public List<PamelloPlayer> GetVoicePlayers(ulong vcId) {
            HashSet<int> channels = new HashSet<int>();

            foreach (var speaker in _speakers) {
                if (speaker.Voice.Id == vcId) {
                    channels.Add(speaker.Channel);
                }
            }

            return channels.Select(_players.GetRequired).ToList();
        }
    }
}
