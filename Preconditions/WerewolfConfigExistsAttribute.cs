using Discord.Commands;
using Soujiwolf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soujiwolf.Preconditions
{
    public class WerewolfConfigExistsAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var config = (ConfigurationService)services.GetService(typeof(ConfigurationService));
            if (config.HasConfiguration(context.Guild))
                return Task.FromResult(PreconditionResult.FromSuccess());
                return Task.FromResult(PreconditionResult.FromError("This server is not configured."));
        }
    }
}
