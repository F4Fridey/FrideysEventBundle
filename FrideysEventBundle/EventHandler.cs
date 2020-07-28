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
    class EventHandler : IEventHandlerFixedUpdate, IEventHandlerDoorAccess, IEventHandlerPlayerDie, IEventHandlerRoundEnd, IEventHandlerRoundRestart, IEventHandlerRoundStart
    {
		private readonly FrideysEventBundle plugin;

		public EventHandler(FrideysEventBundle plugin)
		{
			this.plugin = plugin;
		}

		public void OnDoorAccess(PlayerDoorAccessEvent ev)
		{
			//plugin.Info(ev.Door.Name);
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			switch (plugin.currentEvent)
			{
				default:
					break;
				case "chaosvsntf":
					CVNoverwatchSpectators(ev.Player);
					break;
				case "chaosvsntfENDING":
					CVNoverwatchSpectators(ev.Player);
					break;
				case "peanutpocalypse":
					if (ev.Killer.TeamRole.Team == TeamType.CLASSD)
					{
						ev.Player.Teleport(new Vector(ev.Killer.GetPosition().x, ev.Killer.GetPosition().y + 1, ev.Killer.GetPosition().z));
						ev.Player.OverwatchMode = true;
					}
					break;
			}
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (plugin.currentEvent == "chaosvsntf" || plugin.currentEvent == "chaosvsntfENDING" || plugin.currentEvent == "peanutpocalypse")
			{
				foreach (Player player in GetPlayers())
				{
					player.OverwatchMode = false;
				}
			}
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			plugin.currentEvent = "blocked";
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			plugin.currentEvent = "none";
		}

		public List<Player> GetPlayers()
		{
			return plugin.Server.GetPlayers();
		}

		float timer;
		float secondTimer;

		public void OnFixedUpdate(FixedUpdateEvent ev)
		{
			/*foreach (Player player in GetPlayers())
			{
				plugin.Info(player.GetPosition().ToString());
			}*/
			if (plugin.currentEvent != "none" && plugin.currentEvent != "blocked")
			{
				if (timer < plugin.time)
				{
					timer += Time.deltaTime;
				}
				else
				{
					switch (plugin.currentEvent)
					{
						default:
							break;
						case "chaosvsntf":
							CVNbeginSuddenDeath();
							break;
						case "peanutpocalypse":
							List<Smod2.API.Door> doors = plugin.Server.Map.GetDoors();
							foreach (Smod2.API.Door door in doors)
							{
								if (door.Name == "NUKE_ARMORY" || door.Name == "LCZ_ARMORY" || door.Name == "914" || door.Name == "HCZ_ARMORY" || door.Name == "096" || door.Name == "106_BOTTOM" || door.Name == "106_PRIMARY" || door.Name == "106_SECONDARY" || door.Name == "079_FIRST" || door.Name == "079_SECOND" || door.Name == "049_ARMORY" || door.Name == "012")
								{
									door.Open = true;
									door.Locked = true;
								}
							}
							break;
					}
				}
				if (secondTimer < 1)
				{
					secondTimer += Time.deltaTime;
				}
				else
				{
					switch (plugin.currentEvent)
					{
						default:
							break;
						case "chaosvsntf":
							break;
						case "peanutpocalypse":
							foreach (Player player in GetPlayers())
							{
								if (player.OverwatchMode == false && player.TeamRole.Role != Smod2.API.RoleType.CLASSD && player.TeamRole.Role != Smod2.API.RoleType.SCP_173)
								{
									player.ChangeRole(Smod2.API.RoleType.SCP_173, true, false);
								}
							}
							break;
					}
				}
			}
		}

		void CVNbeginSuddenDeath()
		{
			List<Smod2.API.Door> doors = plugin.Server.Map.GetDoors();
			foreach (Smod2.API.Door door in doors)
			{
				if (door.Name == "SURFACE_GATE")
				{
					door.Open = false;
					door.Locked = true;
					break;
				}
			}
			foreach (Player player in GetPlayers())
			{
				if (player.TeamRole.Team == Smod2.API.TeamType.NINETAILFOX)
				{
					player.Teleport(new Vector(-11.5f, 1002, -20.3f));
				}else if (player.TeamRole.Team == Smod2.API.TeamType.CHAOS_INSURGENCY)
				{
					player.Teleport(new Vector(12.5f, 1002, 1.3f));
				}
				player.PersonalBroadcast(7, "<color=#FF0000>Sudden death!</color>", false);
			}
			plugin.currentEvent = "chaosvsntfENDING";
		}

		void CVNoverwatchSpectators(Player player)
		{
			player.OverwatchMode = true;
		}
	}
}
