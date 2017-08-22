using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soujiwolf.Services
{
    public class WerewolfService
    {
        public IRole PlayerRole { get; set; }

        public async Task<int> GetNumberOfPlayers(IGuild guild) {
            if (PlayerRole == null)
                return -1;
            return (await guild.GetUsersAsync()).Count(user => user.RoleIds.Contains(PlayerRole.Id));
        }
    }
}
