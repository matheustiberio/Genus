﻿using Discord;
using Discord.Commands;
using GenusBot.Core.Services;

namespace GenusBot.Core.Modules
{
    public class MusicPlayerModule : ModuleBase<SocketCommandContext>
    {
        private readonly MusicPlayerService _musicPlayerService;

        public MusicPlayerModule(MusicPlayerService musicPlayerService)
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

            await _musicPlayerService.PlaySoundAsync(query, (IGuildUser)Context.User);
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

            await _musicPlayerService.PauseSoundAsync((IGuildUser)Context.User);
        }


        [Command("resume")]
        [Alias("r")]
        public async Task Resume(string query)
        {
            if (UserIsNotInVoiceChannel())
            {
                await UserIsNotInVoiceChannelMessage();
                return;
            }

            await _musicPlayerService.ResumeSoundAsync((IGuildUser)Context.User);
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

            await _musicPlayerService.StopSoundAsync((IGuildUser)Context.User);
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

            await _musicPlayerService.SkipSoundAsync((IGuildUser)Context.User);
        }

        private bool UserIsNotInVoiceChannel() => ((IGuildUser)Context.User)?.VoiceChannel == null;
        private async Task UserIsNotInVoiceChannelMessage() => await ReplyAsync("cê não ta num canal de voz.");
    }
}