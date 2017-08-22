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

    public class BroadcastService
    {
        public DiscordSocketClient Client { get; set; }

        public BroadcastService(DiscordSocketClient client) => Client = client;

        public Guid CreateBroadcast(IGuild guild, ITextChannel channel)
        {
            using (var context = new SoujiwolfConnection())
            {
                if (context.Broadcasts.Any(_b => _b.GuildSnowflake == (long)channel.GuildId && _b.ChannelSnowflake == (long)channel.Id))
                    return Guid.Empty;
            }
            var broadcast = new Broadcast()
            {
                BroadcastId = Guid.NewGuid(),
                GuildSnowflake = (long)guild.Id,
                ChannelSnowflake = (long)channel.Id
            };
            using (var context = new SoujiwolfConnection())
            {
                context.Broadcasts.Add(broadcast);
                context.SaveChanges();
            }
            return broadcast.BroadcastId;
        }

        public bool Listen(string guid, IGuild guild, ITextChannel channel)
        {
            var fullGuid = Guid.Parse(guid);
            using (var context = new SoujiwolfConnection())
            {
                var broadcast = context.Broadcasts.SingleOrDefault(_b => _b.BroadcastId.Equals(fullGuid));
                if (broadcast != null)
                {
                    var listener = new BroadcastListener()
                    {
                        Broadcast = broadcast,
                        GuildSnowflake = (long)guild.Id,
                        ChannelSnowflake = (long)channel.Id
                    };
                    context.BroadcastListeners.Add(listener);
                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool RemoveListener(string guid, IGuild guild, ITextChannel channel)
        {
            var fullGuid = Guid.Parse(guid);
            using (var context = new SoujiwolfConnection())
            {
                var broadcast = context.Broadcasts.SingleOrDefault(_b => _b.BroadcastId.Equals(fullGuid));
                if (broadcast != null)
                {
                    context.BroadcastListeners.RemoveRange(broadcast.BroadcastListeners.Where(_l => _l.GuildSnowflake == (long)channel.GuildId && _l.ChannelSnowflake == (long)channel.Id).ToList());
                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool RemoveBroadcast(string guid)
        {
            var fullGuid = Guid.Parse(guid);
            using (var context = new SoujiwolfConnection())
            {
                var broadcast = context.Broadcasts.SingleOrDefault(_b => _b.BroadcastId.Equals(fullGuid));
                if (broadcast != null)
                {
                    if (broadcast.BroadcastListeners.Any())
                        return false;
                    context.Broadcasts.Remove(broadcast);
                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool RemoveBroadcast(IGuild guild, ITextChannel channel)
        {
            using (var context = new SoujiwolfConnection())
            {
                var broadcast = context.Broadcasts.SingleOrDefault(_b => _b.GuildSnowflake == (long)channel.GuildId && _b.ChannelSnowflake == (long)channel.Id);
                if (broadcast != null)
                {
                    if (broadcast.BroadcastListeners.Any())
                        return false;
                    context.Broadcasts.Remove(broadcast);
                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public Broadcast GetBroadcast(IGuild guild, ITextChannel channel)
        {
            using (var context = new SoujiwolfConnection())
            {
                var original = context.Broadcasts.SingleOrDefault(_b => _b.GuildSnowflake == (long)channel.GuildId && _b.ChannelSnowflake == (long)channel.Id);
                return new Broadcast() { BroadcastId = original.BroadcastId, GuildSnowflake = original.GuildSnowflake, ChannelSnowflake = original.ChannelSnowflake, BroadcastListeners = original.BroadcastListeners.ToList() };
            }
        }

        public async Task OnMessage(SocketMessage arg)
        {
            // Bail out if it's a System Message.
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            IGuildChannel channel = (IGuildChannel)msg.Channel;
            if (channel == null) return;

            using (var context = new SoujiwolfConnection())
            {
                var broadcast = context.Broadcasts.SingleOrDefault(_b => _b.GuildSnowflake == (long)channel.GuildId && _b.ChannelSnowflake == (long)channel.Id);
                if (broadcast != null)
                {
                    var sourceGuild = Client.GetGuild(Convert.ToUInt64(broadcast.GuildSnowflake));
                    var sourceUser = sourceGuild.GetUser(msg.Author.Id);
                    var embed = new EmbedBuilder();
                    embed.WithAuthor(new EmbedAuthorBuilder()
                    {
                        Name = $"{sourceUser.Nickname ?? sourceUser.Username}",
                        IconUrl = msg.Author.GetAvatarUrl()
                    });
                    if (!string.IsNullOrWhiteSpace(msg.Content))
                        embed.Description = msg.Content;
                    else
                    {
                        var oEmbed = msg.Embeds.FirstOrDefault();
                        if (oEmbed != null)
                        {
                            embed.Description = oEmbed.Description;
                            foreach(var field in oEmbed.Fields)
                            {
                                if (field.Inline)
                                    embed.AddInlineField(field.Name, field.Value);
                                else
                                    embed.AddField(field.Name, field.Value);
                            }
                            if (oEmbed.Footer.HasValue)
                                embed.Footer = new EmbedFooterBuilder() { IconUrl = oEmbed.Footer.Value.IconUrl, Text = oEmbed.Footer.Value.Text };
                            if (oEmbed.Thumbnail.HasValue)
                                embed.WithThumbnailUrl(oEmbed.Thumbnail.Value.Url);
                            if (oEmbed.Timestamp.HasValue)
                                embed.WithTimestamp(oEmbed.Timestamp.Value);
                        }
                    }

                    foreach (var item in broadcast.BroadcastListeners)
                    {
                        var destinationGuild = Client.GetGuild(Convert.ToUInt64(item.GuildSnowflake));
                        var destinationChannel = destinationGuild.GetTextChannel(Convert.ToUInt64(item.ChannelSnowflake));
                        await destinationChannel.SendMessageAsync(string.Empty, false, embed);
                    }
                }
            }
        }
    }
}