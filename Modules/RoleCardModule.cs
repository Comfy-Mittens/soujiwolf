using Discord;
using Discord.Commands;
using Soujiwolf.Entities;
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
    [Group("role")]
    public class RoleCardModule : SoujiwolfModuleBase
    {
        public RoleCardService RoleSvc { get; private set; }
        public PlayerService PlayerSvc { get; private set; }

        public RoleCardModule(RoleCardService svc, PlayerService playerSvc)
        {
            RoleSvc = svc;
            PlayerSvc = playerSvc;
        }

        [Command("")]
        public async Task Display([Remainder]string name)
        {
            var role = RoleSvc.GetRole(Context.Guild, name);
            if (role == null)
                await ReplyAsync(await CreateSimpleEmbed("Role Management", "No role could be found."));
            else
                await ReplyAsync(await embedRole(role));
        }

        [Command("list"), Priority(1)]
        [WerewolfNarrator]
        public async Task List(bool activeOnly = false)
        {
            var roles = RoleSvc.GetRoles(Context.Guild, activeOnly);
            var embed = await GetEmbedBuilder("Role Manager", "Here are the roles");
            foreach (var entry in roles)
                embed.AddInlineField($"The {entry.Name}", entry.Rate ?? 0d);
            await ReplyAsync(embed);
        }

        private async Task<EmbedBuilder> embedRole(GameRole role)
        {
            var testEmbed = await GetEmbedBuilder($"Role: The {role.Name}", role.Description);
            testEmbed.WithColor(new Color(Convert.ToUInt32(role.Color)));
            testEmbed.WithFooter(new EmbedFooterBuilder() { IconUrl = "http://i.imgur.com/Q78Eqp7.png", Text = $"Use !role {role.Name} to prompt this rolecard" });
            testEmbed.ThumbnailUrl = role.ThumbnailImage;
            return testEmbed;
        }
    }
}
