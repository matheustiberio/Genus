using Discord;
using Discord.Commands;
using GenusBot.Core.Services;

namespace GenusBot.Core.Modules
{
    public class BaseVoiceChannelModule : ModuleBase<SocketCommandContext>
    {
        private readonly BaseVoiceChannelService _voiceChannelService;

        public BaseVoiceChannelModule(BaseVoiceChannelService voiceChannelService)
        {
            _voiceChannelService = voiceChannelService;
        }

        [Command("opa")]
        [Alias("join")]
        public async Task JoinVoiceChannelAsync()
        {
            if (UserIsNotInVoiceChannel())
            {
                await UserIsNotInVoiceChannelMessage();
                return;
            }

            await _voiceChannelService.JoinChannelAsync((IGuildUser)Context.User);
        }

        [Command("valeu")]
        [Alias("q")]
        public async Task LeaveVoiceChannelAsync()
        {
            if (UserIsNotInVoiceChannel())
            {
                await UserIsNotInVoiceChannelMessage();
                return;
            }

            await _voiceChannelService.LeaveChannelAsync((IGuildUser)Context.User);
        }

        protected bool UserIsNotInVoiceChannel() => ((IGuildUser)Context.User)?.VoiceChannel == null;
        protected async Task UserIsNotInVoiceChannelMessage() => await ReplyAsync("cê não ta num canal de voz.");
    }
}
