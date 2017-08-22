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

    public class WerewolfNarratorAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var config = (ConfigurationService)services.GetService(typeof(ConfigurationService));
            if (config.HasConfiguration(context.Guild))
            {
                // Override for devs/tester
                if (context.Message.Author.Id == 83886770768314368 || context.Message.Author.Id == 197216567275028480)
                    return PreconditionResult.FromSuccess();

                var guildUser = await context.Guild.GetUserAsync(context.Message.Author.Id);
                var werewolfConfig = config.GetConfiguration(context.Guild);
                if (guildUser.RoleIds.Contains(Convert.ToUInt64(werewolfConfig.NarratorRoleSnowflake)));
                    return PreconditionResult.FromSuccess();
            }
            return PreconditionResult.FromError("This server is not configured.");
        }
    }
}
