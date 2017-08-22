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

    public class WelcomeService
    {
        public DiscordSocketClient Client { get; set; }

        public WelcomeService(DiscordSocketClient client) => Client = client;


    }
}
