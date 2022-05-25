using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace GenusBot.Core.Modules
{
    public class CommandHandler
    {
        private readonly char _commandPrefix;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly DiscordSocketClient _client;

        public CommandHandler(char commandPrefix, CommandService commands, IServiceProvider services, DiscordSocketClient client)
        {
            _commandPrefix = commandPrefix;
            _commands = commands;
            _services = services;
            _client = client;
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        async Task HandleCommandAsync(SocketMessage messageParam)
        {
            int argPos = 0;

            if (
                messageParam is not SocketUserMessage message ||
                !(message.HasCharPrefix(_commandPrefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot
               )
                return;

            var context = new SocketCommandContext(_client, message);

            await _commands.ExecuteAsync(context, argPos, _services);
        }
    }
}
