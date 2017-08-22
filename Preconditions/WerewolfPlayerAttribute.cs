using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Soujiwolf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soujiwolf.Preconditions
{

    public class WerewolfPlayerAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var config = (ConfigurationService)services.GetService(typeof(ConfigurationService));
            if (config.HasConfiguration(context.Guild))
            {
                var guildUser = await context.Guild.GetUserAsync(context.Message.Author.Id);
                var werewolfConfig = config.GetConfiguration(context.Guild);
                if (guildUser.RoleIds.Any(roleId => roleId == Convert.ToUInt64(werewolfConfig.PlayerRoleSnowflake)))
                    return PreconditionResult.FromSuccess();
                return PreconditionResult.FromError("I'm sorry, you're not a player.");
            }
            return PreconditionResult.FromError("This server is not configured.");
        }
    }
}
