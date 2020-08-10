using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using UnityEngine;

namespace FrideysEventBundle
{
    class EventHandler : IEventHandlerRoundRestart, IEventHandlerRoundStart
    {
		private readonly FrideysEventBundle plugin;

		public EventHandler(FrideysEventBundle plugin)
		{
			this.plugin = plugin;
		}

		public void OnRoundRestart(RoundRestartEvent ev)//remove current event from the qeue
		{
			bool delete;
			plugin.roundState = 0;
			try
			{
				plugin.Debug("Removing " + plugin.eventQeue[0] + " from the qeue.");
				delete = true;
			}
			catch
			{
				plugin.Debug("No event in qeue.");
				delete = false;
				if (plugin.auto_add_to_qeue)
				{
					for (int i = 0; i < plugin.normal_rounds_between_auto_event; i++)
					{
						plugin.eventQeue.Add("noevent");
					}
					System.Random rnd = new System.Random();
					int eventID = rnd.Next(0, 5);
					switch (eventID)
					{
						default:
							break;
						case 0:
							plugin.eventQeue.Add("chaosvsntf");
							break;
						case 1:
							plugin.eventQeue.Add("peanutpocalypse");
							break;
						case 2:
							plugin.eventQeue.Add("dclassbattle");
							break;
						case 3:
							plugin.eventQeue.Add("dclassinvasion");
							break;
						case 4:
							plugin.eventQeue.Add("ttt");
							break;
					}
				}
			}
			if (delete)
			{
				plugin.eventQeue.RemoveAt(0);
			}
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			plugin.roundState = 1;
		}
	}
}
