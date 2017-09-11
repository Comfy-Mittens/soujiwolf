using Discord;
using Discord.Addons.InteractiveCommands;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Soujiwolf.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Soujiwolf
{
    class Program
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private const string TOKEN = "Ooops"; // TODO: Store in config

        #region Properties
        public DiscordSocketClient Client { get; set; }
        public CommandService Commands { get; set; }
        public ServiceCollection Services { get; set; }
        public IServiceProvider ServiceProvider { get; set; }

        public Timer StatusTimer;

        #endregion

        #region Events

        #endregion

        private async Task MainAsync()
        {
            using (var context = new Entities.SoujiwolfConnection()) {
                await context.Database.Connection.OpenAsync();
            }

            Client = new DiscordSocketClient();
            Commands = new CommandService();
            Services = new ServiceCollection();

            Client.Log += Log;

            await AsyncInitializeCommands();

            await Client.LoginAsync(TokenType.Bot, TOKEN);
            await Client.StartAsync();

            StatusTimer = new Timer(new TimerCallback(onTimer), null, TimeSpan.FromMinutes(0), TimeSpan.FromSeconds(5));

            await Task.Delay(-1);
        }

        private void onTimer(object state)
        {
            TimerElapsed().GetAwaiter().GetResult();
        }

        private async Task TimerElapsed()
        {
            if (ServiceProvider != null)
            {
                if (Client != null && Client.CurrentUser != null && Client.LoginState == LoginState.LoggedIn)
                {
                    var config = ServiceProvider.GetService<ConfigurationService>();
                    var firstConfig = config.GetConfiguration();
                    if (firstConfig != null && !string.IsNullOrWhiteSpace(firstConfig.Status))
                    {
                        await Client.SetGameAsync($"{firstConfig.Status} {TimeZoneInfo.ConvertTimeToUtc(DateTime.Now):h:mm tt}");
                    } else
                    {
                        await Client.SetGameAsync(null);
                    }
                }
            }
        }

        private async Task AsyncInitializeCommands()
        {
            ServiceProvider = ConfigureServices();
            #region Type Readers
            #endregion

            await Commands.AddModulesAsync(System.Reflection.Assembly.GetEntryAssembly());
            Client.MessageReceived += Client_MessageReceived;
        }

        private async Task Client_MessageReceived(SocketMessage arg)
        {
            // Bail out if it's a System Message.
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            int pos = 0;

            // TODO: Prefix
            if (msg.HasStringPrefix("!", ref pos) /* || msg.HasMentionPrefix(msg.Discord.CurrentUser, ref pos) */)
            {
                var context = new CommandContext(Client, msg);

                var result = await Commands.ExecuteAsync(context, pos, ServiceProvider);

                // Uncomment the following lines if you want the bot
                // to send a message if it failed (not advised for most situations).
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
                    if (result is ExecuteResult)
                    {
                        Debug.WriteLine(((ExecuteResult)result).Exception);
                    }
                }
            }
        }

        private Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        public async Task LogoutAsync()
        {
            await Client.SetStatusAsync(UserStatus.Invisible);
            await Client.LogoutAsync();
        }

        private IServiceProvider ConfigureServices()
        {
            // Configure logging
            var loggerFactory = new LoggerFactory();
            var broadcastService = new BroadcastService(Client);

            Client.MessageReceived += broadcastService.OnMessage;

            // Configure services
            Services.AddSingleton(Client)
            .AddSingleton(loggerFactory)
            .AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false, ThrowOnError = false }))
            .AddSingleton<InteractiveService>()
            .AddSingleton<WerewolfService>()
            .AddSingleton<ConfigurationService>()
            .AddSingleton<ModeratorService>()
            .AddSingleton<VotingService>()
            .AddSingleton(typeof(RoleCardService), new RoleCardService(Client))
            .AddSingleton(typeof(PlayerService), new PlayerService(Client))
            .AddSingleton(typeof(BroadcastService), broadcastService);
                
            var provider = Services.BuildServiceProvider();

            return provider;
        }
    }
}
