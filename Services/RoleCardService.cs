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

    public class RoleCardService
    {
        public DiscordSocketClient Client { get; set; }

        private SoujiwolfConnection GetContext() => new SoujiwolfConnection();

        public RoleCardService(DiscordSocketClient client) => Client = client;

        public void Add(GameRole role)
        {
            using (var context = GetContext())
            {
                context.GameRoles.Add(role);
                context.SaveChanges();
            }
        }

        public bool Remove(IGuild guild, string name)
        {
            using (var context = GetContext())
            {
                var matches = context.GameRoles.Where(role => role.GuildSnowflake == (long)guild.Id && role.Name.Equals(name)).ToList();
                if (matches.Count > 0)
                {
                    context.GameRoles.RemoveRange(matches);
                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public void Update(GameRole newRole)
        {
            using (var context = GetContext())
            {
                var oldRole = context.GameRoles.SingleOrDefault(role => role.GuildSnowflake == (long)newRole.GuildSnowflake && role.Name.Equals(newRole.Name));
                if (oldRole != null)
                {
                    oldRole.Description = newRole.Description;
                    oldRole.Color = newRole.Color;
                    oldRole.ThumbnailImage = newRole.ThumbnailImage;
                    oldRole.Active = newRole.Active;
                    oldRole.Rate = newRole.Rate;
                    context.SaveChanges();
                }
            }
        }

        public GameRole GetRole(IGuild guild, string name)
        {
            using (var context = GetContext())
                return context.GameRoles.SingleOrDefault(role => role.GuildSnowflake == (long)guild.Id && role.Name.Equals(name));
        }

        public IReadOnlyList<GameRole> GetRoles(IGuild guild, bool activeOnly = false)
        {
            using (var context = GetContext()) {
                var matches = context.GameRoles.Where(role => role.GuildSnowflake == (long)guild.Id);
                if (activeOnly)
                    matches = matches.Where(role => role.Rate.HasValue);
                return matches.ToList();
            }
        }
    }
}
