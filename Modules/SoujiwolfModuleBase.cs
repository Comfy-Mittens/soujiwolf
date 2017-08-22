using Discord;
using Discord.Addons.InteractiveCommands;
using Soujiwolf.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soujiwolf.Modules
{
    public abstract class SoujiwolfModuleBase : InteractiveModuleBase
    {
        public static readonly string CHECK = ":white_check_mark:";
        public static readonly string CROSS = ":negative_squared_cross_mark:";
        public const string DATETIME_FORMAT = "dddd, MMMM dd yyyy h:mm tt";

        public async Task<IReadOnlyList<IGuildUser>> FindMembers(Func<IGuildUser, bool> filter)
        {
            var users = await Context.Guild.GetUsersAsync();
            var matched = new List<IGuildUser>();
            foreach (var user in users.Where(user => !string.IsNullOrEmpty(user.Username)))
                if (filter(user))
                    matched.Add(user);
            return matched;
        }

        public async Task<IRole> GetBotRole()
        {
            return await GetUserPrimaryRole(Context.Client.CurrentUser);
        }

        public async Task<IRole> GetUserPrimaryRole(IUser user)
        {
            var targetUser = await Context.Guild.GetUserAsync(user.Id);

            var roles = Context.Guild.Roles.Where(o => targetUser.RoleIds.Contains(o.Id) && !o.Color.Equals(Color.Default));
            var role = Context.Guild.EveryoneRole;
            if (roles.Any())
                role = roles.GetOrderedRoles().First();
            return role;
        }

        public async Task<Color> GetUserColor(IUser user)
        {
            return (await GetUserPrimaryRole(user)).Color;
        }

        public async Task<Color> GetBotColor()
        {
            var botRole = await GetBotRole();
            return botRole.Color;
        }

        /// <summary>
        /// Finds members with only the everyone role
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<IGuildUser>> FindMembersWithoutRoles()
        {
            return await FindMembers((member =>
            {
                return member.RoleIds.Count == 1 && member.RoleIds.Contains(Context.Guild.EveryoneRole.Id);
            }));
        }

        public async Task<IReadOnlyList<IGuildUser>> FindMembersOfRole(IRole role)
        {
            return await FindMembers((member =>
            {
                return member.RoleIds.Contains(role.Id);
            }));
        }

        public async Task<IReadOnlyList<IGuildUser>> FindMembersOfRoleWithStatus(IRole role, UserStatus status = UserStatus.Online)
        {
            return await FindMembers((member =>
            {
                return member.RoleIds.Contains(role.Id) && member.Status == status;
            }));
        }

        public async Task<Embed> CreateSimpleEmbed(string title, string description)
        {
            return await GetEmbedBuilder(title, description);
        }

        public async Task<Embed> CreateSimpleEmbed(string title, StringBuilder description)
        {
            return await GetEmbedBuilder(title, description.ToString());
        }

        #region GetEmbedBuilder
        public async Task<EmbedBuilder> GetEmbedBuilder()
        {
            var embed = new EmbedBuilder();
            embed.WithColor(await GetBotColor());
            return embed;
        }

        public async Task<EmbedBuilder> GetEmbedBuilder(string title)
        {
            var embed = await GetEmbedBuilder();
            embed.Title = title.Trim();
            return embed;
        }

        public async Task<EmbedBuilder> GetEmbedBuilder(string title, string description)
        {
            var embed = await GetEmbedBuilder(title);
            embed.Description = description.Trim();
            return embed;
        }

        public async Task<EmbedBuilder> GetEmbedBuilder(string title, StringBuilder description)
        {
            return await GetEmbedBuilder(title, description.ToString());
        }
        #endregion

        public async Task<IUserMessage> ReplyAsync(Embed embed, RequestOptions requestOptions = null)
        {
            return await base.ReplyAsync(string.Empty, false, embed, requestOptions);
        }
    }
}
