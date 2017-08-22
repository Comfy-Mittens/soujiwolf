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
    public class RoleCardManagementModule : SoujiwolfModuleBase
    {
        public RoleCardService RoleSvc { get; private set; }
        public PlayerService PlayerSvc { get; private set; }

        public const int WW_COLOR = 12028040;
        public const int V_COLOR = 10272919;
        public const int N_COLOR = 13684944;

        public const string WW_IMG = "http://i.imgur.com/WU4McdB.png";
        public const string V_IMG = "http://i.imgur.com/zSxHKas.png";
        public const string N_IMG = "http://i.imgur.com/N2ai2FM.png";

        public RoleCardManagementModule(RoleCardService svc, PlayerService playerSvc)
        {
            RoleSvc = svc;
            PlayerSvc = playerSvc;
        }

        private async Task addRole2(GameRole role)
        {
            bool descriptionSet = false;
            do
            {
                await ReplyAsync(await CreateSimpleEmbed("Role Management", "Please enter the description for this role."));
                var descText = await WaitForMessage(Context.Message.Author, Context.Message.Channel, TimeSpan.FromMinutes(5));
                role.Description = descText.Content;

                await ReplyAsync("Here is your Role Card", false, await embedRole(role));

                bool confirmResponse = false;
                do
                {
                    await ReplyAsync(await CreateSimpleEmbed("Role Management", "Enter: " +
                        "\r\n\t``save`` to save this role" +
                        "\r\n\t``cancel`` to discard any changes" +
                        "\r\n\t``retry`` to re-enter the description"));
                    var confirm = await WaitForMessage(Context.Message.Author, Context.Message.Channel, TimeSpan.FromMinutes(1));
                    if (confirm.Content.IndexOf("save", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        descriptionSet = true;
                        confirmResponse = true;
                        break;
                    }
                    else if (confirm.Content.IndexOf("cancel", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        await ReplyAsync(await CreateSimpleEmbed("Role Management", "Cancelled."));
                        return;
                    }
                    else
                    {
                        confirmResponse = true;
                    }
                } while (!confirmResponse);

            } while (!descriptionSet);

            RoleSvc.Add(role);
            await ReplyAsync(await CreateSimpleEmbed("Role Management", "Your role has been added."));
        }

        [Command("add", RunMode = RunMode.Async), Alias("+")]
        [WerewolfNarrator]
        [Priority(5)]
        public async Task AddRole(string name, uint color, string image)
        {
            image = image.Trim('<', '>');
            var role = new GameRole()
            {
                GuildSnowflake = (long)Context.Guild.Id,
                Name = name,
                Color = color,
                ThumbnailImage = image,
                Rate = null
            };

            await addRole2(role);
        }


        [Command("add", RunMode = RunMode.Async), Alias("+")]
        [WerewolfNarrator]
        [Priority(5)]
        public async Task AddRole(string name, uint color, string image, double rate = 0)
        {
            image = image.Trim('<', '>');
            var role = new GameRole()
            {
                GuildSnowflake = (long)Context.Guild.Id,
                Name = name,
                Color = color,
                ThumbnailImage = image,
                Rate = rate
            };

            await addRole2(role);
        }

        [Command("addv", RunMode = RunMode.Async), Alias("addvillager")]
        [WerewolfNarrator]
        [Priority(5)]
        public async Task AddVillager([Remainder]string name)
        {
            await AddRole(name, V_COLOR, V_IMG);
        }

        [Command("addv", RunMode = RunMode.Async), Alias("addvillager")]
        [WerewolfNarrator]
        [Priority(10)]
        public async Task AddVillager(string name, double rate)
        {
            await AddRole(name, V_COLOR, V_IMG, rate);
        }

        [Command("addww", RunMode = RunMode.Async), Alias("addwerewolf")]
        [WerewolfNarrator]
        [Priority(5)]
        public async Task AddWerewolf([Remainder]string name)
        {
            await AddRole(name, WW_COLOR, WW_IMG);
        }

        [Command("addww", RunMode = RunMode.Async), Alias("addwerewolf")]
        [WerewolfNarrator]
        [Priority(10)]
        public async Task AddWerewolf(string name, double rate)
        {
            await AddRole(name, WW_COLOR, WW_IMG, rate);
        }

        [Command("addn", RunMode = RunMode.Async), Alias("addneutral")]
        [WerewolfNarrator]
        [Priority(5)]
        public async Task AddNeutral([Remainder]string name)
        {
            await AddRole(name, N_COLOR, N_IMG);
        }

        [Command("addn", RunMode = RunMode.Async), Alias("addneutral")]
        [WerewolfNarrator]
        [Priority(10)]
        public async Task AddNeutral(string name, double rate)
        {
            await AddRole(name, N_COLOR, N_IMG, rate);
        }

        [Command("remove"), Alias("delete", "-")]
        [Priority(5)]
        [WerewolfNarrator]
        public async Task Remove(string name)
        {
            if (RoleSvc.Remove(Context.Guild, name))
                await ReplyAsync(await CreateSimpleEmbed("Role Management", "The role was removed successfully."));
            else
                await ReplyAsync(await CreateSimpleEmbed("Role Management", "No role could be found."));
        }

        [Command("set rate")]
        [WerewolfNarrator]
        [Priority(5)]
        public async Task SetRate(string name, double rate)
        {
            var role = RoleSvc.GetRole(Context.Guild, name);
            if (role == null)
                await ReplyAsync(await CreateSimpleEmbed("Role Management", "No role could be found."));
            else
            {
                role.Rate = rate;
                RoleSvc.Update(role);
                await ReplyAsync(await CreateSimpleEmbed("Role Management", $"Rate updated to {rate}"));
            }
        }

        [Command("null rate")]
        [WerewolfNarrator]
        [Priority(5)]
        public async Task NulLRate(string name)
        {
            var role = RoleSvc.GetRole(Context.Guild, name);
            if (role == null)
                await ReplyAsync(await CreateSimpleEmbed("Role Management", "No role could be found."));
            else
            {
                role.Rate = null;
                RoleSvc.Update(role);
                await ReplyAsync(await CreateSimpleEmbed("Role Management", $"Rate updated to null. This role will not be assigned."));
            }
        }

        [Command("assign"), Priority(1)]
        [WerewolfNarrator]
        public async Task Assign()
        {
            await PlayerSvc.Assign(Context.Guild);
            await View();
        }

        [Command("reset"), Priority(1)]
        [WerewolfNarrator]
        public async Task ResetRoles()
        {
            PlayerSvc.Reset(Context.Guild);
            await ReplyAsync(await CreateSimpleEmbed("Role Management", "Roles reset."));
        }

        [Command("show"), Priority(1)]
        [WerewolfNarrator]
        public async Task View()
        {
            var roles = PlayerSvc.GetRoles(Context.Guild);
            var embed = await GetEmbedBuilder("Role Manager", "Here are the roles assigned");
            embed.AddField("Total Players", $"{roles.Aggregate(0, (count, entry) => count + entry.Value.Count)}");
            foreach (var entry in roles)
            {
                embed.AddInlineField($"The {entry.Key.Name} ({entry.Value.Count})", entry.Value.Stringify(Environment.NewLine, "<@!", ">"));
            }
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
