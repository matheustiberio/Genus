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
        private IServiceProvider ServiceProvider { get; set; }

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

            SetupServices();
            var discordClient = ServiceProvider.GetRequiredService<DiscordSocketClient>();

            await SetupCommands(discordClient, ServiceProvider);

            await discordClient.LoginAsync(TokenType.Bot, Settings.Token);
            await discordClient.StartAsync();

            discordClient.Ready += OnReadyAsync;

            await Task.Delay(Timeout.Infinite);
        }

        void SetupServices()
        {
            LoggingService.Log(GetType(), LogLevel.Information, "Setting up services.");

            ServiceProvider = new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddLavaNode()
                .AddSingleton(SetupLavalinkConfig())
                .AddSingleton<BaseVoiceChannelService>()
                .AddSingleton<MusicPlayerService>()
                .BuildServiceProvider();
        }

        async Task SetupCommands(DiscordSocketClient client, IServiceProvider services)
        {
            var commandService = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
            });

            var commandHandler = new CommandHandler(Convert.ToChar(Settings.CommandPrefix), commandService, services, client);

            await commandHandler.InstallCommandsAsync();
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

        private async Task OnReadyAsync()
        {
            try
            {
                var lavaNode = ServiceProvider.GetRequiredService<LavaNode>();
                lavaNode.OnLog += LoggingService.OnLavaLog;

                LoggingService.Log(GetType(), LogLevel.Information, "Connecting to Lavalink node.");
                
                await lavaNode.ConnectAsync();
                
                LoggingService.Log(GetType(), LogLevel.Information, "Genus at your service.");
            }
            catch (Exception ex)
            {
                LoggingService.LogCritical(GetType(), "Failed to connect to Lavalink node.", ex);
            }
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
