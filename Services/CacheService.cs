using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soujiwolf.Services
{
    public class CacheService
    {
        public DiscordSocketClient Client { get; set; }

        public CacheService(DiscordSocketClient client) => Client = client;
    }
}
