using Discord;
using Victoria;

namespace GenusBot.Core.Services
{
    public class BaseVoiceChannelService
    {
        private readonly LavaNode _lavaNode;

        public BaseVoiceChannelService(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
        }

        public async Task JoinChannelAsync(IGuildUser guildUser)
        {
            if (_lavaNode.HasPlayer(guildUser.Guild))
                return;

            await _lavaNode.JoinAsync(guildUser.VoiceChannel);
        }

        public async Task LeaveChannelAsync(IGuildUser guildUser)
        {
            if (!_lavaNode.HasPlayer(guildUser.Guild))
                return;

            await _lavaNode.LeaveAsync(guildUser.VoiceChannel);
        }
    }
}
