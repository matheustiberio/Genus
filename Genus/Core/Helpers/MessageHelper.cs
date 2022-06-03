using Discord;
using Discord.Commands;
using Victoria;

namespace GenusBot.Core.Helpers
{
    public static class MessageHelper
    {
        public static async Task DeletePreviousCommand(SocketCommandContext context)
        {
            if (await context.Channel.GetMessageAsync(context.Message.Id) == null)
                return;

            await context.Message.Channel.DeleteMessageAsync(context.Message.Id);
        }

        public async static Task<Embed> ReplyPlayingSongAsync(LavaTrack track, IGuildUser guildUser)
        {
            return await BuildPlayingEmbedMessageAsync(track, guildUser);
        }

        public async static Task<Embed> ReplySongAddedToQueueAsync(LavaTrack track, IGuildUser guildUser)
        {
            return await BuildSongAddedToQueueAsync(track, guildUser);
        }

        public static Embed ReplySongNotFound(IGuildUser guildUser)
        {
            return BuildSongNotFoundEmbedMessageAsync(guildUser);
        }

        public static Embed ReplyPlaylistAddedToQueue(int numberOfSongsAdded, IGuildUser guildUser)
        {
            return BuildPlayListEmbedMessage(numberOfSongsAdded, guildUser);
        }

        public static Embed ReplyPauseSongMessage(IGuildUser guildUser)
        {
            return BuildPauseSong(guildUser);
        }

        public static Embed ReplyResumeSongMessage(IGuildUser guildUser)
        {
            return BuildResumeSong(guildUser);
        }

        async static Task<Embed> BuildPlayingEmbedMessageAsync(LavaTrack track, IGuildUser guildUser)
        {
            var embedBuilder = BuildBaseEmbedBuilder(track.Title, $"\uD83D\uDCBF Sortei a braba: ", guildUser);

            await BuildBaseTrackEmbedBuilderAsync(track.Title, track, embedBuilder);

            AddTrackDurationField(track, embedBuilder);

            return embedBuilder.Build();
        }

        async static Task<Embed> BuildSongAddedToQueueAsync(LavaTrack track, IGuildUser guildUser)
        {
            var embedBuilder = BuildBaseEmbedBuilder(track.Title, $"\u2705 Botei a música na fila, chefia.", guildUser);

            await BuildBaseTrackEmbedBuilderAsync(track.Title, track, embedBuilder);

            AddTrackDurationField(track, embedBuilder);

            return embedBuilder.Build();
        }

        static Embed BuildSongNotFoundEmbedMessageAsync(IGuildUser guildUser)
        {
            var embedBuilder = BuildBaseEmbedBuilder(string.Empty, $"Num achei essa música ou playlist \uD83D\uDE22", guildUser);

            return embedBuilder.Build();
        }

        static Embed BuildPlayListEmbedMessage(int numberOfSongsAdded, IGuildUser guildUser)
        {
            var embedBuilder = BuildBaseEmbedBuilder(string.Empty, $"\u2705 Botei a playlist na fila, chefia. Adicionei {numberOfSongsAdded} músicas à ela.", guildUser);

            return embedBuilder.Build();
        }

        static Embed BuildResumeSong(IGuildUser guildUser)
        {
            var embedBuilder = BuildBaseEmbedBuilder(string.Empty, $"\u25B6 Sortei!", guildUser);

            return embedBuilder.Build();
        }

        static Embed BuildPauseSong(IGuildUser guildUser)
        {
            var embedBuilder = BuildBaseEmbedBuilder(string.Empty, $"\u23F8 Calmô", guildUser);

            return embedBuilder.Build();
        }

        static void AddTrackDurationField(LavaTrack track, EmbedBuilder embedBuilder)
        {
            embedBuilder.AddField("Duração: ", $"{track.Duration:mm\\:ss}");
        }

        static EmbedBuilder BuildBaseEmbedBuilder(string title, string authorName, IGuildUser guildUser)
        {
            var embed = new EmbedBuilder
            {
                Title = title,
                Color = Color.Green,
            }.WithAuthor(x => x.WithName(authorName))
            .WithFooter(x =>
            {
                x.IconUrl = guildUser.GetAvatarUrl();
                x.Text = guildUser.Username ?? guildUser.Nickname;
            });

            return embed;
        }

        static async Task BuildBaseTrackEmbedBuilderAsync(string title, LavaTrack track, EmbedBuilder embed)
        {
            embed.Title = title;
            embed.Url = track.Url;
            embed.ThumbnailUrl = await track.FetchArtworkAsync();
        }
    }
}
