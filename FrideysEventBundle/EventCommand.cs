using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smod2.Commands;
using Smod2.API;

namespace FrideysEventBundle
{
    class EventCommand : ICommandHandler
    {
		private readonly FrideysEventBundle plugin;

		public EventCommand(FrideysEventBundle plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Get a list and execute different events.";
		}

		public string GetUsage()
		{
			return "EVENT";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			try
			{
				plugin.Debug("Please ignore this message [" + args[0] + "]");//to check if anything is in args[0]
			}
			catch//if not the help text is sent
			{
				return new string[] { "\n[ Frideys Event Bundle ]\nMain commands:\nevent add <event> - Add an event to the qeue.\nevent qeue view - Prints the current event qeue.\nevent qeue clear - Clears the current event qeue.\nEvents:\nnoevent - Use this if you want a break between events.\nchaosvsntf - Needs 4 people. Half of the server is NTF, the others are Chaos. The two must fight it out.\npeanutpocalypse - Needs 4 people. Spawns 1 peanut and the rest are D class. The d class must get to the gates to get MICROs while the peanuts must kill the D class to duplicate.\ndclassbattle - Needs 4 people. Class D Battle Royal.\ndclassinvasion - Needs 4 people. 1 - 3 NTF spawn with 5000 HP and must survive for 7 minutes. The rest are D class that constantly respawn with guns.\nttt - Needs 4 people. 1 murderer that must kill everyone, 1 sherif that must kill the murderer, and the rest are innocent of which can pick up the gun to become a sherif if the sherif dies.\ndeathmatch - Deathmatch, the one with the most kills wins after 7 minutes!" };
			}
			switch (args[0])//check what the first argument is
			{
				default:
					return new string[] { "<color=#FF0000>Error</color> : <color=#FF8383>Please use a valid argument.</color>" };
				case "add":
					try
					{
						plugin.Debug("Please ignore this message [" + args[1] + "]");
					}
					catch
					{
						return new string[] { "<color=#FF0000>Error</color> : <color=#FF8383>Please specify a valid event.</color>" };
					}
					switch (args[1])//add events to the qeue
					{
						default:
							return new string[] { "<color=#FF0000>Error</color> : <color=#FF8383>Please specify a valid event.</color>" };
						case "chaosvsntf":
							plugin.eventQeue.Add("chaosvsntf");
							return new string[] { "Added event Chaos vs NTF to the event qeue.\nNote: If requiered players are not met, a normal round will occur." };
						case "peanutpocalypse":
							plugin.eventQeue.Add("peanutpocalypse");
							return new string[] { "Added event Peanutpocalypse to the event qeue.\nNote: If requiered players are not met, a normal round will occur." };
						case "dclassbattle":
							plugin.eventQeue.Add("dclassbattle");
							return new string[] { "Added event D Class Battle Royal to the event qeue.\nNote: If requiered players are not met, a normal round will occur." };
						case "dclassinvasion":
							plugin.eventQeue.Add("dclassinvasion");
							return new string[] { "Added event D Class Invasion to the event qeue.\nNote: If requiered players are not met, a normal round will occur." };
						case "ttt":
							plugin.eventQeue.Add("ttt");
							return new string[] { "Added event TTT to the event qeue.\nNote: If requiered players are not met, a normal round will occur." };
						case "noevent":
							plugin.eventQeue.Add("noevent");
							return new string[] { "Added normal game to the event qeue.\nNote: If requiered players are not met, a normal round will occur." };
						case "deathmatch":
							plugin.eventQeue.Add("deathmatch");
							return new string[] { "Added Deathmatch to the event qeue.\nNote: If requiered players are not met, a normal round will occur." };
					}
				case "qeue":
					try
					{
						plugin.Debug("Please ignore this message [" + args[1] + "]");
					}
					catch
					{
						return new string[] { "<color=#FF0000>Error</color> : <color=#FF8383>Please specify a valid argument.</color>" };
					}
					switch (args[1])//edit the qeue
					{
						default:
							return new string[] { "<color=#FF0000>Error</color> : <color=#FF8383>Please specify a valid argument.</color>" };
						case "view":
							string str = "Event Qeue:\n";
							if (plugin.eventQeue.Count != 0)
							{
								foreach (string eventName in plugin.eventQeue)
								{
									str += eventName + "\n";
								}
							}
							return new string[] { str };
						case "clear":
							plugin.eventQeue.Clear();
							return new string[] { "Event Qeue cleared." };
					}
			}
		}
	}
}
