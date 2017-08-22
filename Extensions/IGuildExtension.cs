using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soujiwolf.Extensions
{
    public static class IGuildExtension
    {
        public static IRole GetRoleByID(this IGuild guild, ulong id)
        {
            return guild.Roles.Where(role => role.Id == id).FirstOrDefault();
        }

        public static IRole GetRoleByName(this IGuild guild, string name)
        {
            return guild.Roles.Where(role => role.Name == name).FirstOrDefault();
        }

        public static IEnumerable<IRole> GetOrderedRoles(this IGuildUser guildUser)
        {
            return guildUser.Guild.GetRoles(guildUser).GetOrderedRoles();
        }

        public static IEnumerable<IRole> GetOrderedUserRoles(this IGuild guild, IGuildUser guildUser)
        {
            return guild.GetRoles(guildUser).GetOrderedRoles();
        }

        public static IEnumerable<IRole> GetRoles(this IGuild guild, IGuildUser user)
        {
            return guild.Roles.Where(currentRole => currentRole.Id != user.Guild.EveryoneRole.Id && user.RoleIds.Contains(currentRole.Id));
        }
    }
}
