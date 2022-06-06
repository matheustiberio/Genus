using Discord;
using Discord.Commands;
using GenusBot.Core.Services;

namespace GenusBot.Core.Modules
{
    public class MusicPlayerModule : BaseVoiceChannelModule
    {
        public readonly MusicPlayerService _musicPlayerService;

        public MusicPlayerModule(MusicPlayerService musicPlayerService, BaseVoiceChannelService baseVoiceChannelService) : base(baseVoiceChannelService)
        {
            _musicPlayerService = musicPlayerService;
        }

        [Command("play")]
        [Alias("p")]
        public async Task Play([Remainder] string query)
        {
            if (UserIsNotInVoiceChannel())
            {
                await UserIsNotInVoiceChannelMessage();
                return;
            }

            SetCommandContextToService();

            var embedMessage = await _musicPlayerService.PlaySoundAsync(query);

            await ReplyAsync(null, false, embedMessage);
        }

        [Command("pause")]
        [Alias("ps")]
        public async Task Pause()
        {
            if (UserIsNotInVoiceChannel())
            {
                await UserIsNotInVoiceChannelMessage();
                return;
            }

            SetCommandContextToService();

            var embedMessage = await _musicPlayerService.PauseSoundAsync();

            await ReplyAsync(null, false, embedMessage);
        }

        [Command("resume")]
        [Alias("r")]
        public async Task Resume()
        {
            if (UserIsNotInVoiceChannel())
            {
                await UserIsNotInVoiceChannelMessage();
                return;
            }

            SetCommandContextToService();

            var embedMessage = await _musicPlayerService.ResumeSoundAsync();

            await ReplyAsync(null, false, embedMessage);

        }

        [Command("stop")]
        [Alias("st")]
        public async Task Stop()
        {
            if (UserIsNotInVoiceChannel())
            {
                await UserIsNotInVoiceChannelMessage();
                return;
            }

            SetCommandContextToService();

            await _musicPlayerService.StopSoundAsync();
        }

        [Command("skip")]
        [Alias("sk")]
        public async Task Skip()
        {
            if (UserIsNotInVoiceChannel())
            {
                await UserIsNotInVoiceChannelMessage();
                return;
            }

            SetCommandContextToService();

            var embedMessage = await _musicPlayerService.SkipSoundAsync();

            await ReplyAsync(null, false, embedMessage);
        }

        [Command("vol")]
        public async Task SetVolumeAsync(ushort volume)
        {
            await _musicPlayerService.SetVolume(volume);
        }

        void SetCommandContextToService() => _musicPlayerService.context = Context;
    }
}
