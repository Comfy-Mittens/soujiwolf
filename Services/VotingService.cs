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

    public class VotingService
    {
        public DiscordSocketClient Client { get; set; }

        public bool CanVote(long guild, long voter)
        {
            using (var context = new SoujiwolfConnection())
            {
                var existingVote = context.Votes.SingleOrDefault(vote => vote.GuildSnowflake == guild && vote.Voter == voter);
                if (existingVote == null)
                    return true;
                return !existingVote.Changed;
            }
        }

        public bool CanVote(ulong guild, ulong voter)
        {
            return CanVote((long)guild, (long)voter);

        }

        public bool CanVote(IGuild guild, IUser voter)
        {
            return CanVote(guild.Id, voter.Id);
        }

        public bool CanVote(IGuildUser voter)
        {
            return CanVote(voter.Guild, voter);
        }

        /// <summary>
        /// Add or update a vote in the database
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="voter"></param>
        /// <param name="voted"></param>
        public void Vote (IGuild guild, IUser voter, IUser voted)
        {
            using (var context = new SoujiwolfConnection())
            {
                var existingVote = context.Votes.SingleOrDefault(vote => vote.GuildSnowflake == (long)guild.Id && vote.Voter == (long)voter.Id);
                if (existingVote == null)
                    context.Votes.Add(new Vote() { GuildSnowflake = (long)guild.Id, Voter = (long)voter.Id, Voted = (long)voted.Id });
                else if (!existingVote.Changed)
                {
                    existingVote.Voted = (long)voted.Id;
                    existingVote.Changed = true;
                }
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Add or update a vote in the database
        /// </summary>
        /// <param name="voter"></param>
        /// <param name="voted"></param>
        public void Vote(IGuildUser voter, IUser voted)
        {
            Vote(voter.Guild, voter, voted);
        }

        public Dictionary<ulong, IReadOnlyList<ulong>> GetResults(IGuild guild)
        {
            return GetResults((long)guild.Id);
        }

        public Dictionary<ulong, IReadOnlyList<ulong>> GetResults(long guildSnowflake)
        {
            var results = new Dictionary<ulong, List<ulong>>();
            using (var context = new SoujiwolfConnection())
            {
                var votesInGuild = from votes in context.Votes
                        where votes.GuildSnowflake == guildSnowflake
                        select votes;
                foreach(var vote in votesInGuild)
                {
                    var voted = Convert.ToUInt64(vote.Voted);
                    var voter = Convert.ToUInt64(vote.Voter);
                    if (!results.ContainsKey(voted))
                        results.Add(voted, new List<ulong>());
                    results[voted].Add(voter);
                }
            }
            return results.ToDictionary(kvp => kvp.Key, kvp=> (IReadOnlyList<ulong>)kvp.Value);
        }

        public void Reset(IGuild guild) => Reset((long)guild.Id);

        public void Reset(long id)
        {
            using (var context = new SoujiwolfConnection())
            {
                var query = from vote in context.Votes
                            where vote.GuildSnowflake == id
                            select vote;
                context.Votes.RemoveRange(query.ToList());
                context.SaveChanges();
            }
        }
    }
}
