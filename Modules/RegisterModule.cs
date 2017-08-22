using Discord;
using Discord.Commands;
using Soujiwolf.Preconditions;
using Soujiwolf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soujiwolf.Modules
{

    public class RegisterModule : SoujiwolfModuleBase
    {
        public ConfigurationService Service { get; set; }

        public RegisterModule(ConfigurationService svc)
        {
            Service = svc;
        }

        [Command("join")]
        public async Task Join()
        {
            var configuration = Service.GetConfiguration(Context.Guild);
            if (!string.IsNullOrWhiteSpace(configuration.Status))
                await ReplyAsync(await CreateSimpleEmbed("Signup", $"{CROSS} You cannot join a game that is already in session!"));
            else
            {
                var user = await Context.Guild.GetUserAsync(Context.Message.Author.Id);
                if (user.RoleIds.Contains(Convert.ToUInt64(configuration.PlayerRoleSnowflake)))
                    await ReplyAsync(await CreateSimpleEmbed("Signup", $"{CROSS} You are already signed up!"));
                else
                {
                    await user.AddRoleAsync(Context.Guild.GetRole(Convert.ToUInt64(configuration.PlayerRoleSnowflake)));
                    await ReplyAsync(await CreateSimpleEmbed("Signup", $"{CHECK} You have joined the game!"));
                }
            }
        }

        [Command("leave")]
        [WerewolfPlayer]
        public async Task Leave()
        {
            var configuration = Service.GetConfiguration(Context.Guild);
            if (!string.IsNullOrWhiteSpace(configuration.Status))
                await ReplyAsync(await CreateSimpleEmbed("Signup", $"{CROSS} You cannot leave a game that is already in session!"));
            else
            {
                var user = await Context.Guild.GetUserAsync(Context.Message.Author.Id);
                if (user.RoleIds.Contains(Convert.ToUInt64(configuration.PlayerRoleSnowflake)))
                {
                    await user.RemoveRoleAsync(Context.Guild.GetRole(Convert.ToUInt64(configuration.PlayerRoleSnowflake)));
                    await ReplyAsync(await CreateSimpleEmbed("Signup", $"{CROSS} You have left the game!"));
                }
                else
                    await ReplyAsync(await CreateSimpleEmbed("Signup", $"{CHECK} You are have not joined the game!"));
            }
        }

        [Command("players")]
        [WerewolfNarrator]
        public async Task ShowPlayers()
        {
            var configuration = Service.GetConfiguration(Context.Guild);
            var players = await FindMembersOfRole(Context.Guild.GetRole(Convert.ToUInt64(configuration.PlayerRoleSnowflake)));

            var desc = new StringBuilder();
            var i = 1;
            foreach(var iPlayer in players.OrderByDescending(player => player.Id))
            {
                desc.AppendLine($"{i++}) {iPlayer.Mention}");
            }
            var embed = await GetEmbedBuilder("Currently signed up players", desc);
            embed.ThumbnailUrl = "http://i.imgur.com/kXFWdUL.png";
            await ReplyAsync(embed);
        }

        [Command("disband")]
        [WerewolfNarrator]
        public async Task Disband()
        {
            var configuration = Service.GetConfiguration(Context.Guild);
            var playerRole = Context.Guild.GetRole(Convert.ToUInt64(configuration.PlayerRoleSnowflake));
            var players = await FindMembersOfRole(playerRole);
            foreach (var iPlayer in players)
                await iPlayer.RemoveRoleAsync(playerRole);

            // Dead
            var deadRole = Context.Guild.GetRole(Convert.ToUInt64(configuration.DeadRoleSnowflake));
            var dead = await FindMembersOfRole(deadRole);
            foreach (var iPlayer in dead)
                await iPlayer.RemoveRoleAsync(deadRole);

            await ReplyAsync(await CreateSimpleEmbed("Signup", "Game disbanded!"));
        }
    }
}
