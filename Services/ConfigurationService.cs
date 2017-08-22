using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Soujiwolf.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soujiwolf.Services
{

    public class ConfigurationService
    {
        public DiscordSocketClient Client { get; set; }

        public ConfigurationService(DiscordSocketClient client) => Client = client;

        public Werewolf GetConfiguration(IGuild guild) => GetConfiguration(guild.Id);
        public Werewolf GetConfiguration()
        {
            using (var context = new SoujiwolfConnection())
                return context.WerewolfGame.FirstOrDefault();
        }

        public Werewolf GetConfiguration(ulong guild) => GetConfiguration((long)guild);

        public Werewolf GetConfiguration(long guild)
        {
            using (var context = new SoujiwolfConnection())
                return context.WerewolfGame.SingleOrDefault(configItem => configItem.GuildSnowflake == guild);
        }

        public bool HasConfiguration(IGuild guild) => HasConfiguration(guild.Id);
        public bool HasConfiguration(ulong guild) => HasConfiguration((long)guild);

        private bool HasConfiguration(long guild)
        {
            using (var context = new SoujiwolfConnection())
                return context.WerewolfGame.Any(configItem => configItem.GuildSnowflake == guild);
        }

        public void SetConfiguration(Werewolf wolf)
        {
            using (var context = new SoujiwolfConnection())
            {
                var config = context.WerewolfGame.SingleOrDefault(configItem => configItem.GuildSnowflake == wolf.GuildSnowflake);
                if (config!= null)
                {
                    config.NarratorRoleSnowflake = wolf.NarratorRoleSnowflake;
                    config.PlayerRoleSnowflake = wolf.PlayerRoleSnowflake;
                    config.VillageChannelSnowflake = wolf.VillageChannelSnowflake;
                    config.WerewolfChannelSnowflake = wolf.WerewolfChannelSnowflake;
                    config.DeadRoleSnowflake = wolf.DeadRoleSnowflake;
                } else
                {
                    context.WerewolfGame.Add(wolf);
                }
                context.SaveChanges();
            }
        }

        public void SetConfiguration(IGuild guild, IRole narrator, IRole player)
        {
            SetConfiguration(new Werewolf()
            {
                GuildSnowflake = (long)guild.Id,
                NarratorRoleSnowflake = (long)narrator.Id,
                PlayerRoleSnowflake = (long)player.Id
            });
        }

        public void SetConfiguration(IGuild guild, IRole narrator, IRole player, ITextChannel village, ITextChannel werewolf)
        {
            SetConfiguration(new Werewolf()
            {
                GuildSnowflake = (long)guild.Id,
                NarratorRoleSnowflake = (long)narrator.Id,
                PlayerRoleSnowflake = (long)player.Id,
                VillageChannelSnowflake = (long)village.Id,
                WerewolfChannelSnowflake = (long)werewolf.Id,
                Status = ""
            });
        }

        public void SetConfiguration(IGuild guild, IRole narrator, IRole player, IRole dead, ITextChannel village, ITextChannel werewolf)
        {
            SetConfiguration(new Werewolf()
            {
                GuildSnowflake = (long)guild.Id,
                NarratorRoleSnowflake = (long)narrator.Id,
                PlayerRoleSnowflake = (long)player.Id,
                VillageChannelSnowflake = (long)village.Id,
                WerewolfChannelSnowflake = (long)werewolf.Id,
                DeadRoleSnowflake = (long)dead.Id,
                Status = ""
            });
        }

        public void SetNarrator(IGuild guild, IRole narrator)
        {
            using (var context = new SoujiwolfConnection())
            {
                var config = context.WerewolfGame.SingleOrDefault(configItem => configItem.GuildSnowflake == (long)guild.Id);
                config.NarratorRoleSnowflake = (long)narrator.Id;
                context.SaveChanges();
            }
        }

        public void SetPlayer(IGuild guild, IRole player)
        {
            using (var context = new SoujiwolfConnection())
            {
                var config = context.WerewolfGame.SingleOrDefault(configItem => configItem.GuildSnowflake == (long)guild.Id);
                config.PlayerRoleSnowflake = (long)player.Id;
                context.SaveChanges();
            }
        }

        public void SetDead(IGuild guild, IRole dead)
        {
            using (var context = new SoujiwolfConnection())
            {
                var config = context.WerewolfGame.SingleOrDefault(configItem => configItem.GuildSnowflake == (long)guild.Id);
                config.DeadRoleSnowflake = (long)dead.Id;
                context.SaveChanges();
            }
        }

        public void SetVillage(IGuild guild, ITextChannel channel)
        {
            using (var context = new SoujiwolfConnection())
            {
                var config = context.WerewolfGame.SingleOrDefault(configItem => configItem.GuildSnowflake == (long)guild.Id);
                config.VillageChannelSnowflake = (long)channel.Id;
                context.SaveChanges();
            }
        }

        public void SetWerewolf(IGuild guild, ITextChannel channel)
        {
            using (var context = new SoujiwolfConnection())
            {
                var config = context.WerewolfGame.SingleOrDefault(configItem => configItem.GuildSnowflake == (long)guild.Id);
                config.WerewolfChannelSnowflake = (long)channel.Id;
                context.SaveChanges();
            }
        }

        public void SetStatus(IGuild guild, string status)
        {
            using (var context = new SoujiwolfConnection())
            {
                var config = context.WerewolfGame.SingleOrDefault(configItem => configItem.GuildSnowflake == (long)guild.Id);
                config.Status = status;
                context.SaveChanges();
            }
        }
    }
}
