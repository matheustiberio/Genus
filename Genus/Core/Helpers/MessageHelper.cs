using Discord;
using Discord.Commands;
using Victoria;

namespace GenusBot.Core.Helpers
{
    public static class MessageHelper
    {
        public async static Task<Embed> ReplayPlayingSongAsync(LavaTrack track, IGuildUser guildUser, SocketCommandContext context)
        {
            await DeletePreviousCommand(context);

            return await BuildPlayingEmbedMessageAsync(track, guildUser);
        }

        async static Task<Embed> BuildPlayingEmbedMessageAsync(LavaTrack track, IGuildUser guildUser)
        {
            var embedBuilder = await BuildBaseEmbedBuilder(track.Title, track);
            
            embedBuilder.WithAuthor(x =>
            {
                x.WithName($"Now playing: ");
                x.WithIconUrl("https://c.tenor.com/Sb0yPHMgNaUAAAAi/music-disc.gif");

            }).WithFooter(x =>
            {
                x.IconUrl = guildUser.GetAvatarUrl();
                x.Text = guildUser.Username ?? guildUser.Nickname;
            })
            .AddField("Duration: ", $"{track.Duration:mm\\:ss}");

            return embedBuilder.Build();
        }

        static async Task<EmbedBuilder> BuildBaseEmbedBuilder(string title, LavaTrack? track = null)
        {
            var embed = new EmbedBuilder
            {
                Title = title,
                Color = Color.Green,
            };

            if (track != null)
            {
                embed.Title = title;
                embed.Url = track.Url;
                embed.ThumbnailUrl = await track.FetchArtworkAsync();
            }

            return embed;
        }

        static async Task DeletePreviousCommand(SocketCommandContext context)
        {
            if (await context.Channel.GetMessageAsync(context.Message.Id) == null)
                return;

            await context.Message.Channel.DeleteMessageAsync(context.Message.Id);
        }
    }
}
