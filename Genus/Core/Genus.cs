using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GenusBot.Core.Modules;
using GenusBot.Core.Services;
using GenusBot.Core.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Victoria;

namespace GenusBot.Core
{
    public class Genus
    {
        public BotSettings Settings { get; private set; }

        public async Task Initialize()
        {
            PrintLogo();

            LoggingService.Log(GetType(), LogLevel.Information, "Initializing Genus.");

            Settings = await new BotSettings().LoadAsync() ?? new BotSettings();

            if (!BotSettings.IsValid(Settings))
            {
                LoggingService.Log(GetType(), LogLevel.Error, "Failure at loading bot settings.");
                Console.ReadLine();
                return;
            }

            var services = SetupServices();
            var discordClient = services.GetRequiredService<DiscordSocketClient>();

            await SetupCommands(discordClient, services);

            await discordClient.LoginAsync(TokenType.Bot, Settings.Token);
            await discordClient.StartAsync();

            LoggingService.Log(GetType(), LogLevel.Information, "GenusBot at your service.");

            await Task.Delay(Timeout.Infinite);
        }

        IServiceProvider SetupServices()
        {
            LoggingService.Log(GetType(), LogLevel.Information, "Setting up services.");

            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddLavaNode(x => SetupLavalinkConfig())
                .BuildServiceProvider();
        }

        LavaConfig SetupLavalinkConfig()
        {
            return new LavaConfig
            {
                Hostname = Settings.LavalinkHostname,
                Authorization = Settings.LavalinkAuthorization,
                Port = Convert.ToUInt16(Settings.LavalinkPort),
                IsSsl = Settings.IsSSLLavalinkHost,
                SelfDeaf = true,
            };
        }

        async Task SetupCommands(DiscordSocketClient client, IServiceProvider services)
        {
            var commandService = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
            });

            var commandHandler = new CommandHandler(Convert.ToChar(Settings.CommandPrefix), commandService, services, client);

            await commandHandler.InstallCommandsAsync();
        }

        static void PrintLogo()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(@"
                           ______                              
                          / ____/  ___    ____   __  __   _____
                         / / __   / _ \  / __ \ / / / /  / ___/
                        / /_/ /  /  __/ / / / // /_/ /  (__  ) 
                        \____/   \___/ /_/ /_/ \__,_/  /____/  
                                       _______
                                     _/       \_
                                    / |       | \
                                   /  |__   __|  \
                                  |__/((o| |o))\__|
                                  |      | |      |
                                  |\     |_|     /|
                                  | \           / |
                                   \| /  ___  \ |/
                                    \ | / _ \ | /
                                     \_________/
                                      _|_____|_
                                 ____|_________|____
                                /                   \  -- art by: Mark Moir
            ");
        }
    }
}
