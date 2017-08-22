using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soujiwolf.Extensions
{
    public static class IGuildUserExtension
    {
        public static string ToMentions(this IEnumerable<IGuildUser> users, bool useNickname = true)
        {
            string prefix = "<@", suffix = ">";
            if (useNickname)
                prefix += "!";
            return string.Join(", " + prefix, users.Select(user => prefix + user.Id.ToString() + suffix));
        }

        public static TimeSpan GetAge(this IGuildUser user)
        {
            return DateTime.Now - user.JoinedAt.Value;
        }

        public static string GetDisplayName(this IGuildUser user)
        {
            return !string.IsNullOrEmpty(user.Nickname) ? user.Nickname : user.Username;
        }
    }
}
