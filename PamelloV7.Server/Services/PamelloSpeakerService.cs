using Discord.WebSocket;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Repositories;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model.Audio.Speakers;

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

        private async Task<bool> AddDiscord(PamelloPlayer player, DiscordSocketClient client, ulong guildId, ulong vcId) {
            var guild = client.GetGuild(guildId);
            if (guild is null) return false;

            var speaker = new PamelloDiscordSpeaker(_services, client, guild.Id, player);
            await speaker.InitialConnect(vcId);

            _speakers.Add(speaker);
            speaker.OnTerminated += Speaker_Terminated;

            return true;
        }

        private void Speaker_Terminated(PamelloSpeaker speaker) {
            _speakers.Remove(speaker);
        }

        public async Task ConnectDiscord(PamelloPlayer player, ulong guildId, ulong vcId) {
            bool clientAvailable;
            foreach (var speakerClient in _discordClients.DiscordClients) {
                clientAvailable = true;

                foreach (var speaker in _speakers) {
                    if (speaker is not PamelloDiscordSpeaker discordSpeaker) continue;

                    if (discordSpeaker.Client.CurrentUser.Id == speakerClient.CurrentUser.Id && discordSpeaker.Guild.Id == guildId) {
                        clientAvailable = false;
                        break;
                    }
                }

                if (clientAvailable) {
                    if (await AddDiscord(player, speakerClient, guildId, vcId)) return;
                }
            }

            throw new PamelloException("No available speakers left");
        }
        public async Task DisconnectDiscord(PamelloPlayer player, ulong vcId) {
            foreach (var speaker in _speakers) {
                if (speaker is not PamelloDiscordSpeaker discordSpeaker) continue;

                if (discordSpeaker.Player == player && discordSpeaker.Voice.Id == vcId) {
                    await speaker.Terminate();
                    return;
                }
            }
        }

        public async Task<PamelloInternetSpeaker> ConnectInternet(PamelloPlayer player, int? channel = null) {
            if (channel is null) {
                channel = 0;
                while (!IsInternetChannelAvailable((++channel).Value));
            }
            else {
                if (!IsInternetChannelAvailable(channel.Value)) {
                    throw new Exception($"Channel {channel} is unavailable");
                }
            }

            if (channel <= 0) throw new PamelloException("Use of channels <= 0 is not available");

            var internetSpeaker = new PamelloInternetSpeaker(player, channel.Value);
            await internetSpeaker.InitialConnection();

            _speakers.Add(internetSpeaker);
            internetSpeaker.OnTerminated += Speaker_Terminated;

            return internetSpeaker;
        }

        public bool DoesPlayerHasSpeakers(PamelloPlayer player) {
            foreach (var speaker in _speakers) {
                if (speaker.Player == player && speaker.IsActive) {
                    return true;
                }
            }

            return false;
        }

        public bool IsInternetChannelAvailable(int channel) {
            foreach(var speaker in _speakers) {
                if (speaker is PamelloInternetSpeaker internetSpeaker) {
                    if (internetSpeaker.Channel == channel) {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task BroadcastBytes(PamelloPlayer player, byte[] audio) {
            foreach (var speaker in _speakers) {
                if (speaker.Player == player) {
                    await speaker.PlayBytesAsync(audio);
                }
            }
        }

        public List<PamelloPlayer> GetVoicePlayers(ulong vcId) {
            var players = new HashSet<PamelloPlayer>();

            foreach (var speaker in _speakers) {
                if (speaker is not PamelloDiscordSpeaker discordSpeaker) continue;

                if (discordSpeaker.Voice.Id == vcId) {
                    players.Add(speaker.Player);
                }
            }

            return players.ToList();
        }

        public PamelloInternetSpeaker? GetInternetSpeaker(int channel) {
            foreach (var speaker in _speakers) {
                if (speaker is PamelloInternetSpeaker internetSpeaker) {
                    if (internetSpeaker.Channel == channel) return internetSpeaker;
                }
            }

            return null;
        }
    }
}
