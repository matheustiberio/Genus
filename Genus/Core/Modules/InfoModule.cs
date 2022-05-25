using Discord.Commands;

namespace GenusBot.Core.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("opa")]
        public async Task SayAsync() => await ReplyAsync("aoba");
    }
}
