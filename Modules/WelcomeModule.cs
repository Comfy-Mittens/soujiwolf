using Discord;
using Discord.Commands;
using Soujiwolf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soujiwolf.Modules
{

    public class WelcomeModule
    {
        public WelcomeService WelcomeSvc { get; set; }

        public WelcomeModule(WelcomeService svc) => WelcomeSvc = svc;
    }
}
