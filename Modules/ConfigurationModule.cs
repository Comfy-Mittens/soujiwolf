using Discord;
using Discord.Commands;
using Soujiwolf.Preconditions;
using Soujiwolf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soujiwolf.Extensions;

namespace Soujiwolf.Modules
{

    [Group("config")]
    public class ConfigurationModule : SoujiwolfModuleBase
    {
        public ConfigurationService Service { get; set; }

        public ConfigurationModule(ConfigurationService svc)
        {
            Service = svc;
        }

        [Command("")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task InitializeGame(IRole narrator, IRole player, ITextChannel village, ITextChannel werewolf, IRole dead)
        {
            Service.SetConfiguration(Context.Guild, narrator, player, dead, village, werewolf);
            await EmbedConfig();
        }

        [WerewolfConfigExists]
        [Command("set narrator")]
        public async Task SetNarrator(IRole narrator)
        {
            Service.SetNarrator(Context.Guild, narrator);
            // await ReplyAsync(string.Empty, false, await CreateSimpleEmbed("Config", $"Narrator Role: {narrator.Name}"));
            await EmbedConfig();
        }

        [WerewolfConfigExists]
        [Command("set player")]
        public async Task SetPlayer(IRole player)
        {
            Service.SetPlayer(Context.Guild, player);
            //await ReplyAsync(string.Empty, false, await CreateSimpleEmbed("Config", $"Player Role: {player.Name}"));
            await EmbedConfig();
        }

        [WerewolfConfigExists]
        [Command("set village")]
        public async Task SetVillage(ITextChannel channel)
        {
            Service.SetVillage(Context.Guild, channel);
            //await ReplyAsync(string.Empty, false, await CreateSimpleEmbed("Config", $"Village Channel: {channel.Mention}"));
            await EmbedConfig();
        }

        [WerewolfConfigExists]
        [Command("set werewolf")]
        public async Task SetWerewolf(ITextChannel channel)
        {
            Service.SetWerewolf(Context.Guild, channel);
            //await ReplyAsync(string.Empty, false, await CreateSimpleEmbed("Config", $"Werewolf Channel: {channel.Mention}"));
            await EmbedConfig();
        }

        [WerewolfConfigExists]
        [Command("set dead")]
        public async Task SetDead(IRole dead)
        {
            Service.SetDead(Context.Guild, dead);
            //await ReplyAsync(string.Empty, false, await CreateSimpleEmbed("Config", $"Dead Role: {dead.Name}"));
            await EmbedConfig();
        }

        [Command("view")]
        [WerewolfConfigExists]
        [WerewolfNarrator]
        public async Task View()
        {
            await EmbedConfig();
        }

        private async Task EmbedConfig()
        {
            var config = Service.GetConfiguration(Context.Guild);
            await ReplyAsync(string.Empty, false, await CreateSimpleEmbed("Config", $"Narrator Role: {config.NarratorRoleSnowflake.ToIRole(Context.Guild).Mention}\r\n" +
                $"Player Role: {config.PlayerRoleSnowflake.ToIRole(Context.Guild).Mention}\r\n" +
                $"Village Channel: <#{config.VillageChannelSnowflake}>\r\n" +
                $"Werewolf Channel: <#{config.WerewolfChannelSnowflake}>\r\n" +
                $"Dead Role: {config.DeadRoleSnowflake.ToIRole(Context.Guild).Mention}"));
        }
    }
}
