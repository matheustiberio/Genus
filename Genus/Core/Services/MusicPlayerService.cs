using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GenusBot.Core.Helpers;
using System;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;
using Victoria.Responses.Search;

namespace GenusBot.Core.Services
{
    public class MusicPlayerService
    {
        private readonly LavaNode _lavaNode;

        public SocketCommandContext context;

        public MusicPlayerService(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
        }

        public async Task<Embed> PlaySoundAsync(string query)
        {
            LavaPlayer musicPlayer = await GetMusicPlayer(context);

            var searchResult = await _lavaNode.SearchAsync(SearchType.YouTube, query);

            if (!searchResult.Tracks.Any())
                return MessageHelper.ReplySongNotFound((IGuildUser)context.Guild);

            var firstResulTrack = searchResult.Tracks.First();
            Embed message;

            if (musicPlayer.PlayerState == PlayerState.Playing || musicPlayer.PlayerState == PlayerState.Paused)
            {

                if (IsPlaylist(searchResult))
                {
                    AddPlaylistTracksToQueue(musicPlayer, searchResult);
                    await MessageHelper.DeletePreviousCommand(context);
                    message = MessageHelper.ReplyPlaylistAddedToQueue(searchResult.Tracks.Count, GetGuildUser(context));
                }
                else
                {
                    musicPlayer.Queue.Enqueue(firstResulTrack);
                    await MessageHelper.DeletePreviousCommand(context);
                    message = await MessageHelper.ReplySongAddedToQueueAsync(firstResulTrack, GetGuildUser(context));
                }

                return message;
            }

            await musicPlayer.PlayAsync(firstResulTrack);
            await MessageHelper.DeletePreviousCommand(context);
            message = await MessageHelper.ReplyPlayingSongAsync(firstResulTrack, GetGuildUser(context));

            return message;
        }

        public async Task<Embed> PauseSoundAsync()
        {
            var musicPlayer = await GetMusicPlayer(context);

            await musicPlayer.PauseAsync();

            return MessageHelper.ReplyPauseSongMessage(GetGuildUser(context));
        }

        public async Task<Embed> ResumeSoundAsync()
        {
            var musicPlayer = await GetMusicPlayer(context);

            await musicPlayer.ResumeAsync();
            
            await MessageHelper.DeletePreviousCommand(context);
            return MessageHelper.ReplyResumeSongMessage(GetGuildUser(context));
        }

        public async Task StopSoundAsync()
        {
            var musicPlayer = await GetMusicPlayer(context);

            await musicPlayer.StopAsync();
        }

        public async Task<Embed> SkipSoundAsync()
        {
            var musicPlayer = await GetMusicPlayer(context);

            var (skippedSong, currentSong) = await musicPlayer.SkipAsync(TimeSpan.FromMilliseconds(200));
            
            await MessageHelper.DeletePreviousCommand(context);
            return await MessageHelper.ReplyPlayingSongAsync(currentSong, GetGuildUser(context));
        }

        public async Task SetVolume(ushort volume)
        {
            var musicPlayer = await GetMusicPlayer(context);

            await musicPlayer.UpdateVolumeAsync(volume);
        }

        public async Task OnTrackEnded(TrackEndedEventArgs args)
        {
            if (args.Reason != TrackEndReason.Finished)
                return;

            var player = args.Player;
            if (!player.Queue.TryDequeue(out var queueable))
            {
                await player.TextChannel.SendMessageAsync("Cabô as músicas, patrão. Sextou? \uD83D\uDE0E");
                return;
            }

            if (queueable is not LavaTrack track)
            {
                await player.TextChannel.SendMessageAsync("Ixi, a próxima música deu xabu.");
                return;
            }

            await args.Player.PlayAsync(track);
            await args.Player.TextChannel.SendMessageAsync(null, false, await MessageHelper.ReplyPlayingSongAsync(track, GetGuildUser(context)));
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

        async Task<LavaPlayer> GetMusicPlayer(SocketCommandContext context)
        {
            LavaPlayer musicPlayer;
            IGuildUser guildUser = GetGuildUser(context);

            if (!_lavaNode.HasPlayer(context.Guild))
            {
                musicPlayer = await _lavaNode.JoinAsync(guildUser.VoiceChannel, (ITextChannel)context.Channel);
                await musicPlayer.UpdateVolumeAsync(2);
            }
            else
                musicPlayer = _lavaNode.GetPlayer(context.Guild);


            return musicPlayer;
        }

        private static IGuildUser GetGuildUser(SocketCommandContext context) => (IGuildUser)context.User;
    }
}
