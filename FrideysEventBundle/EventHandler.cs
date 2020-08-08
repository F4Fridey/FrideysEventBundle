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
    class EventHandler : IEventHandlerRoundRestart
    {
		private readonly FrideysEventBundle plugin;

		public EventHandler(FrideysEventBundle plugin)
		{
			this.plugin = plugin;
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			bool delete;
			try
			{
				plugin.Debug("Removing " + plugin.eventQeue[0] + " from the qeue.");
				delete = true;
			}
			catch
			{
				plugin.Debug("No event in qeue.");
				delete = false;
			}
			if (delete)
			{
				plugin.eventQeue.RemoveAt(0);
			}
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			
		}
	}
}
