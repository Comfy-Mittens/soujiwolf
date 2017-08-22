using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Soujiwolf.Preconditions;
using Soujiwolf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soujiwolf.Modules
{

    public class TimeModule : SoujiwolfModuleBase
    {
        public const string DAY = "☀";
        public const string NIGHT = "☽";

        public TimeModule(ConfigurationService svc)
        {
            Service = svc;
        }

        public ConfigurationService Service { get; private set; }

        [Command("settime day")]
        [WerewolfNarrator]
        public async Task Day()
        {
            var configuration = Service.GetConfiguration(Context.Guild);
            var village = await Context.Guild.GetTextChannelAsync(Convert.ToUInt64(configuration.VillageChannelSnowflake));
            var werewolf = await Context.Guild.GetTextChannelAsync(Convert.ToUInt64(configuration.WerewolfChannelSnowflake));
            if (village.GetPermissionOverwrite(Context.Guild.EveryoneRole).HasValue)
                await village.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, village.GetPermissionOverwrite(Context.Guild.EveryoneRole).Value.Modify(null, null, null, null, PermValue.Inherit));
            else
                await village.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit));

            if (werewolf.GetPermissionOverwrite(Context.Guild.EveryoneRole).HasValue)
                await werewolf.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, werewolf.GetPermissionOverwrite(Context.Guild.EveryoneRole).Value.Modify(null, null, null, null, PermValue.Deny));
            else
                await werewolf.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Deny));

            Service.SetStatus(Context.Guild, DAY);
            await ((DiscordSocketClient)Context.Client).SetGameAsync($"{DAY} {TimeZoneInfo.ConvertTimeToUtc(DateTime.Now):h:mm tt}");
        }

        [Command("settime night")]
        [WerewolfNarrator]
        public async Task Night()
        {
            var configuration = Service.GetConfiguration(Context.Guild);
            var village = await Context.Guild.GetTextChannelAsync(Convert.ToUInt64(configuration.VillageChannelSnowflake));
            var werewolf = await Context.Guild.GetTextChannelAsync(Convert.ToUInt64(configuration.WerewolfChannelSnowflake));
            if (village.GetPermissionOverwrite(Context.Guild.EveryoneRole).HasValue)
                await village.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, village.GetPermissionOverwrite(Context.Guild.EveryoneRole).Value.Modify(null, null, null, null, PermValue.Deny));
            else
                await village.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Deny));
            if (werewolf.GetPermissionOverwrite(Context.Guild.EveryoneRole).HasValue)
                await werewolf.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, werewolf.GetPermissionOverwrite(Context.Guild.EveryoneRole).Value.Modify(null, null, null, null, PermValue.Inherit));
            else
                await werewolf.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit));

            Service.SetStatus(Context.Guild, NIGHT);
            await ((DiscordSocketClient)Context.Client).SetGameAsync($"{NIGHT} {TimeZoneInfo.ConvertTimeToUtc(DateTime.Now):h:mm: tt}");
        }

        [Command("settime none")]
        [WerewolfNarrator]
        public async Task None()
        {
            var configuration = Service.GetConfiguration(Context.Guild);
            var village = await Context.Guild.GetTextChannelAsync(Convert.ToUInt64(configuration.VillageChannelSnowflake));
            var werewolf = await Context.Guild.GetTextChannelAsync(Convert.ToUInt64(configuration.WerewolfChannelSnowflake));

            if (village.GetPermissionOverwrite(Context.Guild.EveryoneRole).HasValue)
                await village.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, village.GetPermissionOverwrite(Context.Guild.EveryoneRole).Value.Modify(null, null, null, null, PermValue.Deny));
            else 
                await village.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Deny));
            if (werewolf.GetPermissionOverwrite(Context.Guild.EveryoneRole).HasValue)
                await werewolf.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, werewolf.GetPermissionOverwrite(Context.Guild.EveryoneRole).Value.Modify(null, null, null, null, PermValue.Deny));
            else
                await werewolf.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Deny));

            Service.SetStatus(Context.Guild, string.Empty);
            await ((DiscordSocketClient)Context.Client).SetGameAsync(null);
        }
    }
}
