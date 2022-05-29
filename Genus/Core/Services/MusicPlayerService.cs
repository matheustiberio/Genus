using Discord;
using Discord.Commands;
using GenusBot.Core.Helpers;
using System;
using Victoria;
using Victoria.Enums;
using Victoria.Responses.Search;

namespace GenusBot.Core.Services
{
    public class MusicPlayerService
    {
        private readonly LavaNode _lavaNode;

        public MusicPlayerService(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
        }

        public async Task<Embed> PlaySoundAsync(string query, IGuildUser guildUser, SocketCommandContext context)
        {
            LavaPlayer musicPlayer = await GetMusicPlayer(guildUser.Guild, guildUser.VoiceChannel);

            var searchResult = await _lavaNode.SearchAsync(SearchType.YouTube, query);

            var firstResulTrack = searchResult.Tracks.First();

            if (musicPlayer.PlayerState == PlayerState.Playing || musicPlayer.PlayerState == PlayerState.Paused)
            {
                if (IsPlaylist(searchResult))
                    AddPlaylistTracksToQueue(musicPlayer, searchResult);
                else
                    musicPlayer.Queue.Enqueue(firstResulTrack);

                return null;
            }

            await musicPlayer.PlayAsync(firstResulTrack);

            return await MessageHelper.ReplayPlayingSongAsync(firstResulTrack, guildUser, context);
        }

        public async Task PauseSoundAsync(IGuildUser guildUser)
        {
            var musicPlayer = await GetMusicPlayer(guildUser.Guild, guildUser.VoiceChannel);

            await musicPlayer.PauseAsync();
        }

        public async Task ResumeSoundAsync(IGuildUser guildUser)
        {
            var musicPlayer = await GetMusicPlayer(guildUser.Guild, guildUser.VoiceChannel);

            await musicPlayer.ResumeAsync();
        }

        public async Task StopSoundAsync(IGuildUser guildUser)
        {
            var musicPlayer = await GetMusicPlayer(guildUser.Guild, guildUser.VoiceChannel);

            await musicPlayer.StopAsync();
        }

        public async Task SkipSoundAsync(IGuildUser guildUser)
        {
            var musicPlayer = await GetMusicPlayer(guildUser.Guild, guildUser.VoiceChannel);

            await musicPlayer.SkipAsync();
        }

        static bool IsPlaylist(SearchResponse searchResult)
        {
            return !string.IsNullOrEmpty(searchResult.Playlist.Name);
        }

        static void AddPlaylistTracksToQueue(LavaPlayer musicPlayer, SearchResponse searchResult)
        {
            foreach (var playListTrack in searchResult.Tracks)
                musicPlayer.Queue.Enqueue(playListTrack);
        }

        async Task<LavaPlayer> GetMusicPlayer(IGuild guild, IVoiceChannel voiceChannel)
        {
            LavaPlayer musicPlayer;

            if (!_lavaNode.HasPlayer(guild))
                musicPlayer = await _lavaNode.JoinAsync(voiceChannel);
            else
                musicPlayer = _lavaNode.GetPlayer(guild);

            await SetVolume(musicPlayer);

            return musicPlayer;
        }

        static async Task SetVolume(LavaPlayer musicPlayer, ushort volume = 2)
        {
            await musicPlayer.UpdateVolumeAsync(volume);
        }
    }
}
