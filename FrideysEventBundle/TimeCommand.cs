using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smod2.Commands;
using Smod2.API;

namespace FrideysEventBundle
{
    class TimeCommand : ICommandHandler
    {
        private readonly FrideysEventBundle plugin;

        public TimeCommand(FrideysEventBundle plugin)
        {
            this.plugin = plugin;
        }

        public string GetCommandDescription()
        {
            return "See how long the game has been running for.";
        }

        public string GetUsage()
        {
            return "TIME";
        }

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            string str = "";
            if (plugin.roundState == 0)
            {
                str += "Round has not started yet...";
            }
            else
            {
                str += "The round has been going on for " + (plugin.roundTime / 60).ToString("0.0") + " minutes.";
            }
            return new string[] { str };
        }
    }
}
