using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soujiwolf.Extensions
{
    public static class IRoleExtension
    {
        public static IEnumerable<IRole> GetOrderedRoles(this IEnumerable<IRole> roles)
        {
            return roles.OrderByDescending(o => o.Position).ToList();
        }

        public static string ToDelimitedString(this IEnumerable<IRole> roles, string delimiter = ", ", string surroundWith = "'")
        {
            return string.Join(delimiter, roles.Select(role => surroundWith + role.Name + surroundWith));
        }
    }
}
