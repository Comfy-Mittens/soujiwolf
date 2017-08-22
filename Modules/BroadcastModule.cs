using Discord;
using Discord.Commands;
using Soujiwolf.Services;
using Soujiwolf.Preconditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soujiwolf.Modules
{

    [Group("broadcast")]
    public class BroadcastModule : SoujiwolfModuleBase
    {
        public BroadcastService Broadcaster {get;set;}

        public BroadcastModule(BroadcastService service) => Broadcaster = service;

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("")]
        public async Task Broadcast()
        {
            var guid = Broadcaster.CreateBroadcast(Context.Guild, (ITextChannel)Context.Channel);
            if (guid != Guid.Empty)
            {
                await ReplyAsync(await CreateSimpleEmbed("Broadcast", "Broadcast created!"));
                var dm = await Context.Message.Author.GetOrCreateDMChannelAsync();

                var msg = new StringBuilder();
                msg.AppendLine("Broadcast created!");
                msg.AppendLine($"Source Server: {Context.Guild.Name}");
                msg.AppendLine($"Source Channel: {Context.Message.Channel.Name}");
                msg.AppendLine($"Key: {guid}");
                await dm.SendMessageAsync(string.Empty, false, await CreateSimpleEmbed("Broadcast", msg));
            }
            else
            {
                await ReplyAsync(await CreateSimpleEmbed("Broadcast", "Broadcast could not be created!"));
            }
        }

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("")]
        public async Task Broadcast(ITextChannel channel)
        {
            var guid = Broadcaster.CreateBroadcast(Context.Guild, channel);
            if (guid != null)
            {
                await ReplyAsync(await CreateSimpleEmbed("Broadcast", "Broadcast created!"));
                var dm = await Context.Message.Author.GetOrCreateDMChannelAsync();

                var msg = new StringBuilder();
                msg.AppendLine("Broadcast created!");
                msg.AppendLine($"Source Server: {Context.Guild.Name}");
                msg.AppendLine($"Source Channel: {channel.Name}");
                msg.AppendLine($"Key: {guid}");
                await dm.SendMessageAsync(string.Empty, false, await CreateSimpleEmbed("Broadcast", msg));
            }
            else
                await ReplyAsync(await CreateSimpleEmbed("Broadcast", "Broadcast could not be created!"));
        }

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("listen"), Priority(1)]
        public async Task Listen(string guid)
        {
            await Listen(guid, (ITextChannel)Context.Message.Channel);
        }

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("listen"), Priority(1)]
        public async Task Listen(string guid, ITextChannel channel)
        {
            if (Broadcaster.Listen(guid, Context.Guild, channel))
                await channel.SendMessageAsync(string.Empty, false, await CreateSimpleEmbed("Broadcast", "Now listening!"));
            else
                await ReplyAsync(await CreateSimpleEmbed("Broadcast", "Broadcast was not created."));
        }

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("disconnect"), Priority(1)]
        public async Task Disconnect(string guid)
        {
            if (Broadcaster.RemoveListener(guid, Context.Guild, (ITextChannel)Context.Message.Channel))
                await ReplyAsync(await CreateSimpleEmbed("Broadcast", "Broadcast disconnected!"));
            else
                await ReplyAsync(await CreateSimpleEmbed("Broadcast", "Broadcast was not created."));
        }

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("disconnect"), Priority(1)]
        public async Task Disconnect(string guid, ITextChannel channel)
        {
            if (Broadcaster.RemoveListener(guid, Context.Guild, channel))
                await channel.SendMessageAsync(string.Empty, false, await CreateSimpleEmbed("Broadcast", "Broadcast disconnected!"));
            else
                await ReplyAsync(await CreateSimpleEmbed("Broadcast", "Broadcast was not created."));
        }

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("stop"), Priority(1)]
        public async Task StopBroadcast(string guid)
        {
            if (Broadcaster.RemoveBroadcast(guid))
                await ReplyAsync(await CreateSimpleEmbed("Broadcast", "Broadcast removed!"));
            else
                await ReplyAsync(await CreateSimpleEmbed("Broadcast", "Broadcast was not removed."));
        }

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("stop"), Priority(5)]
        public async Task StopBroadcast(ITextChannel channel)
        {
            if (Broadcaster.RemoveBroadcast(Context.Guild, channel))
                await ReplyAsync(await CreateSimpleEmbed("Broadcast", "Broadcast removed!"));
            else
                await ReplyAsync(await CreateSimpleEmbed("Broadcast", "Broadcast was not removed."));
        }

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("stop"), Priority(1)]
        public async Task StopBroadcast()
        {
            await StopBroadcast((ITextChannel)Context.Message.Channel);
        }

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("display"), Priority(5)]
        public async Task Display(bool resolve = false)
        {

            await Display(Context.Message.Channel as ITextChannel, resolve);
        }

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("display"), Priority(5)]
        public async Task Display(ITextChannel channel, bool resolve = false)
        {
            var broadcast = Broadcaster.GetBroadcast(Context.Guild, channel);
            if (broadcast!= null)
            {
                var embed = await GetEmbedBuilder("Broadcast");
                embed.Description = $"This channel has {broadcast.BroadcastListeners.Count} listeners.";
                foreach (var item in broadcast.BroadcastListeners)
                {
                    if (resolve)
                    {
                        var lGuild = await Context.Client.GetGuildAsync(Convert.ToUInt64(item.GuildSnowflake));
                        var lChannel = await lGuild.GetTextChannelAsync(Convert.ToUInt64(item.ChannelSnowflake));
                        embed.AddField($"{lGuild.Name}", lChannel.Name);
                    }
                    else
                        embed.AddField($"{item.GuildSnowflake}", item.ChannelSnowflake);
                }
                await ReplyAsync(embed);
            }
            else
                await ReplyAsync(await CreateSimpleEmbed("Broadcast", "This channel is not being broadcast."));

        }
    }
}
