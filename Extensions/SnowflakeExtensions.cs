using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soujiwolf.Extensions
{
    public static class SnowflakeExtensions
    {
        public static string Stringify(this IReadOnlyList<ulong> snowflakes, string separator, string prefix = "", string suffix = "")
        {
            return string.Join(separator, snowflakes.Select(snowflake => prefix + snowflake.ToString() + suffix));
        }

        public static async Task<IGuildUser> ToIGuildUser(this ulong snowflake, IGuild guild)
        {
            return await guild.GetUserAsync(snowflake);
        }

        public static IRole ToIRole(this long snowflake, IGuild guild)
        {
            return Convert.ToUInt64(snowflake).ToIRole(guild);
        }

        public static IRole ToIRole(this ulong snowflake, IGuild guild)
        {
            return guild.GetRoleByID(snowflake);
        }

        public static async Task<IChannel> ToIChannel(this long snowflake, IGuild guild) => await Convert.ToUInt64(snowflake).ToIChannel(guild);


        public static async Task<IChannel> ToIChannel(this ulong snowflake, IGuild guild)
        {
            return await guild.GetTextChannelAsync(snowflake);
        }
    }
}
