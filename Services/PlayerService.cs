using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Soujiwolf.Entities;
using Soujiwolf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class PlayerService
{
    public DiscordSocketClient Client { get; set; }

    public PlayerService(DiscordSocketClient client) => Client = client;

    private SoujiwolfConnection GetContext() => new SoujiwolfConnection();

    internal class RoleManager
    {
        internal IEnumerable<GameRole> Roles { get; private set; }
        internal IEnumerable<IGuildUser> Users { get; private set; }
        internal int Count { get; private set; }
        internal Dictionary<GameRole, List<IGuildUser>> Results { get; private set; }
        internal Dictionary<GameRole, int> Limits { get; private set; }

        public RoleManager(IEnumerable<GameRole> roles, IEnumerable<IGuildUser> users)
        {
            Roles = roles;
            Users = users;
            Count = users.Count();
            Results = new Dictionary<GameRole, List<IGuildUser>>();
            Limits = new Dictionary<GameRole, int>();
            var sum = 0d;
            foreach (var role in roles)
            {
                sum += role.Rate.Value;
                Results.Add(role, new List<IGuildUser>());
            }
            GenerateLimits();
            if (sum - 1.0d < 0)
                throw new Exception("Role spawn rates invalid. They don't add up to 1.");
        }

        private void GenerateLimits()
        {
            var limitTotal = 0;
            var retryCount = 0;
            do
            {
                limitTotal = 0;
                Limits = new Dictionary<GameRole, int>();
                foreach (var entry in Results.Keys.ToList())
                {
                    Limits.Add(entry, (int)Math.Max(1, Math.Floor(entry.Rate.Value * Count)));
                    limitTotal += Limits[entry];
                }
                var difference = Count - limitTotal;
                for (var i = difference; i > 0; i--)
                {
                    Limits[Next()]++;
                    limitTotal++;
                }
                retryCount++;
            } while (limitTotal != Count && retryCount < 10);

            if (Limits.Aggregate(0, (current, item) => current + item.Value) != Count)
                throw new Exception("Unable to generate limits");
        }

        public void Assign()
        {
            foreach(var user in Users)
            {
                GameRole role = null;
                do
                {
                    role = Next();
                } while (!Validate(role));
                Results[role].Add(user);
            }
        }

        private GameRole Next()
        {
            // Generate a random double
            double value = RNG.NextDouble();

            double weight_sum = 0d;
            foreach(var role in Roles)
            {
                weight_sum += role.Rate.Value;
                if (value <= weight_sum)
                    return role;
            }
            return null;
        }

        private bool Validate(GameRole role)
        {
            return Results[role].Count < Limits[role];
        }
    }

    public async Task Assign(IGuild guild)
    {
        using (var context = GetContext())
        {
            var roles = context.GameRoles.Where(_role => _role.GuildSnowflake == (long)guild.Id && _role.Rate.HasValue).ToList();
            var config = context.WerewolfGame.Single(_config => _config.GuildSnowflake == (long)guild.Id);
            var users = (await guild.GetUsersAsync()).Where(_user => _user.RoleIds.Contains(Convert.ToUInt64(config.PlayerRoleSnowflake)));
            var roleMgr = new RoleManager(roles, users);
            roleMgr.Assign();

            foreach (var entry in roleMgr.Results)
            {
                foreach (var user in entry.Value)
                {
                    context.PlayerRoles.Add(new PlayerRole() { PlayerSnowflake = (long)user.Id, GameRole = entry.Key });
                    context.SaveChanges();
                }
            }
        }
    }

    public Dictionary<GameRole, List<ulong>> GetRoles(IGuild guild)
    {
        var roles = new Dictionary<GameRole, List<ulong>>();
        using (var context = GetContext())
        {
            foreach(var entry in context.PlayerRoles.Where(_role => _role.GameRole.GuildSnowflake == (long)guild.Id).ToList())
            {
                if (!roles.ContainsKey(entry.GameRole))
                    roles.Add(entry.GameRole, new List<ulong>());
                roles[entry.GameRole].Add(Convert.ToUInt64(entry.PlayerSnowflake));
            }
        }
        return roles;
    }

    public void Reset(IGuild guild)
    {
        using (var context = GetContext())
        {
            var matches = context.PlayerRoles.Where(_role => _role.GameRole.GuildSnowflake == (long)guild.Id).ToList();
            if (matches.Any())
            {
                context.PlayerRoles.RemoveRange(matches);
                context.SaveChanges();
            }
        }
    }
}