using Discord;
using Discord.Commands;
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

    public class VotingModule : SoujiwolfModuleBase
    {
        public VotingService Service { get; set; }
        public ConfigurationService ConfigService { get; set; }

        public VotingModule(ConfigurationService config, VotingService svc)
        { Service = svc;
            ConfigService = config;
        }

        [Command("vote")]
        [WerewolfConfigExists]
        [WerewolfPlayer]
        public async Task Vote(IGuildUser user)
        {
            var config = ConfigService.GetConfiguration(Context.Guild);
            if (!user.RoleIds.Contains(Convert.ToUInt64(config.PlayerRoleSnowflake)))
            {
                await ReplyAsync(await CreateSimpleEmbed("Vote", $"{CROSS} {Context.Message.Author.Mention}, {user.Mention} is not playing."));
                return;
            }

            if (Context.Message.Author.Id == user.Id)
            {
                await ReplyAsync(await CreateSimpleEmbed("Vote", $"{CROSS} {Context.Message.Author.Mention}, you cannot vote for yourself."));
                return;
            }

            if (!Service.CanVote(Context.Guild, Context.Message.Author))
                await ReplyAsync(await CreateSimpleEmbed("Vote", $"{CROSS} {Context.Message.Author.Mention}, you are only permitted to change your vote once."));
            else
            {
                Service.Vote(Context.Guild, Context.Message.Author, user);
                await ReplyAsync(await CreateSimpleEmbed("Vote", $"{CHECK} {Context.Message.Author.Mention}, your vote has been recorded."));
            }
        }

        [Command("vote display"), Priority(1)]
        public async Task Display()
        {
            var results = Service.GetResults(Context.Guild);
            var configuration = ConfigService.GetConfiguration(Context.Guild);
            var players = await FindMembersOfRole(Context.Guild.GetRole(Convert.ToUInt64(configuration.PlayerRoleSnowflake)));

            var embed = await GetEmbedBuilder("Vote");
            if (results.Count != 0)
            {
                var total = 0;
                foreach (var entry in results)
                {
                    StringBuilder voters = new StringBuilder();
                    foreach (var voter in entry.Value)
                        voters.AppendLine($"<@!{voter}> " + (!Service.CanVote(Context.Guild.Id, voter) ? ":lock:" : ""));

                    var fullUser = (await entry.Key.ToIGuildUser(Context.Guild));
                    if (fullUser != null)
                        embed.AddInlineField($"{fullUser.GetDisplayName()} ({entry.Value.Count})", voters.ToString().Trim());
                    else
                        embed.AddInlineField($"{entry.Key} ({entry.Value.Count})", voters.ToString().Trim());

                    total += entry.Value.Count;
                }
                embed.Description = $"Out of {players.Count()} players, {total} have voted.";
            }
            else
                embed.Description = Environment.NewLine + "No votes have been cast.";
            await ReplyAsync(embed);
        }

        [Command("vote missing"), Priority(1)]
        [WerewolfNarrator]
        public async Task Missing()
        {
            var results = Service.GetResults(Context.Guild);
            var configuration = ConfigService.GetConfiguration(Context.Guild);
            var players = await FindMembersOfRole(Context.Guild.GetRole(Convert.ToUInt64(configuration.PlayerRoleSnowflake)));
            var remaining = players.ToList();

            var embed = await GetEmbedBuilder("Vote");
            if (results.Count != 0)
            {
                foreach (var entry in results)
                {
                    foreach (var voter in entry.Value)
                    {
                        var toRemove = remaining.SingleOrDefault(o => o.Id == Convert.ToUInt64(voter));
                        if (toRemove != null)
                        remaining.Remove(toRemove);
                    }
                }
                var strRemaining = string.Join(Environment.NewLine, remaining.Select(r => r.Mention));
                embed.Description = $"The following users have not voted:\r\n " + strRemaining;
            }
            else
                embed.Description = Environment.NewLine + "No votes have been cast.";
            await ReplyAsync(embed);
        }

        [Command("vote reset"), Priority(1)]
        [WerewolfNarrator]
        public async Task Reset()
        {
            Service.Reset(Context.Guild);
            await ReplyAsync(await CreateSimpleEmbed("Vote", $"Votes have been reset."));
        }
    }
}
