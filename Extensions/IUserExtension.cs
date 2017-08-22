using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soujiwolf.Extensions
{
    public static class IUserExtension
    {
        public static readonly string[] DEFAULT_AVATARS = new string[]
        {
            "6debd47ed13483642cf09e832ed0bc1b",
            "322c936a8c8be1b803cd94861bdfa868",
            "dd4dbc0016779df1378e7812eabaa04d",
            "0e291f67c9274a1abdddeb3fd919cbaa",
            "1cbd08c76f8af6dddce02c5138971129"
        };

        public static readonly string AVATAR_URL = "https://discordapp.com/assets/{0}.png";

        public static string GetAvatarUrlSafe(this IUser user)
        {
            if (!string.IsNullOrEmpty(user.AvatarId))
                return user.GetAvatarUrl();
            return string.Format(AVATAR_URL, DEFAULT_AVATARS[user.DiscriminatorValue % DEFAULT_AVATARS.Length]);
        }
    }
}
