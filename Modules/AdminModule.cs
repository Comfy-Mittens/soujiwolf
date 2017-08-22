using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Soujiwolf.Extensions;
using Soujiwolf.Preconditions;
using Soujiwolf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soujiwolf.Modules
{

    public class AdminModule : SoujiwolfModuleBase
    {
        public ConfigurationService ConfigSvc { get; set; }
        public AdminModule(ConfigurationService config)
        {
            ConfigSvc = config;
        }

        [Command("exit")]
        [RequireOwner]
        public async Task Shutdown()
        {
            var client = (Context.Client as DiscordSocketClient);
            await client.LogoutAsync();
            Environment.Exit(0);
        }

        [Command("depower")]
        [WerewolfConfigExists]
        [WerewolfNarrator]
        public async Task Depower(IGuildUser user)
        {
            var config = ConfigSvc.GetConfiguration(user.Guild);
            await user.RemoveRoleAsync(config.NarratorRoleSnowflake.ToIRole(user.Guild));
        }

        [Command("depower")]
        [WerewolfConfigExists]
        [WerewolfNarrator]
        public async Task Depower()
        {
            var config = ConfigSvc.GetConfiguration(Context.Guild);
            var user = await Context.Guild.GetUserAsync(Context.Message.Author.Id);
            await user.RemoveRoleAsync(config.NarratorRoleSnowflake.ToIRole(user.Guild));
        }
    }
}
