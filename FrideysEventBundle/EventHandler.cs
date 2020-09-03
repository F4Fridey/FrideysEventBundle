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
    class EventHandler : IEventHandlerRoundRestart, IEventHandlerRoundStart, IEventHandlerPlayerDie, IEventHandlerRoundEnd, IEventHandlerFixedUpdate, IEventHandlerPlayerJoin, IEventHandlerDoorAccess, IEventHandlerWarheadDetonate
    {
		private readonly FrideysEventBundle plugin;

		public EventHandler(FrideysEventBundle plugin)
		{
			this.plugin = plugin;
		}

		List<string> teamkillers = new List<string>();
		bool endTime = false;

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			bool isEmpty = !plugin.eventQeue.Any();
			bool playerFound = false;
			if ((isEmpty || plugin.eventQeue[0] == "noevent") && !endTime && ev.Player.UserId != ev.Killer.UserId)
			{
				if (ev.Player.TeamRole.Team == ev.Killer.TeamRole.Team || ((ev.Player.TeamRole.Team == TeamType.SCIENTIST || ev.Player.TeamRole.Team == TeamType.NINETAILFOX) && (ev.Killer.TeamRole.Team == TeamType.SCIENTIST || ev.Killer.TeamRole.Team == TeamType.NINETAILFOX)) || ((ev.Player.TeamRole.Team == TeamType.CLASSD || ev.Player.TeamRole.Team == TeamType.CHAOS_INSURGENCY) && (ev.Killer.TeamRole.Team == TeamType.CLASSD || ev.Killer.TeamRole.Team == TeamType.CHAOS_INSURGENCY)))
				{
					try
					{
						foreach (string teamkiller in teamkillers)
						{
							string[] array = teamkiller.Split(',');
							if (array[0] == ev.Killer.UserId)
							{
								int kills = Int32.Parse(array[1]);
								kills += 1;
								teamkillers.Remove(teamkiller);
								string tker = array[0] + "," + kills;
								teamkillers.Add(tker);
								plugin.Info(ev.Killer.Name + " teamkilled again. Now has " + kills + " kills");
								playerFound = true;
								if (kills == 2)
								{
									ev.Killer.ChangeRole(Smod2.API.RoleType.SPECTATOR);
								}
								else if (kills >= 3)
								{
									ev.Killer.Ban(1, "Kicked for teamkilling.");
								}
								else if (kills >= 4)
								{
									ev.Killer.Ban(1200, "Banned for 20 hours for teamkilling.");
								}
								break;
							}
						}
					}
					catch { }
					if (!playerFound)
					{
						string teamkiller = ev.Killer.UserId;
						teamkiller += ",1";
						teamkillers.Add(teamkiller);
						ev.Killer.PersonalBroadcast(10, "<color=#ff0000>WARNING: Teamkilling is not allowed. More infractions will lead to automatic action.</color>", true);
						plugin.Info(ev.Killer.Name + " teamkilled. Now has 1 kill");
					}
				}
			}
		}

		public void OnRoundRestart(RoundRestartEvent ev)//remove current event from the qeue
		{
			warheadDetonated = false;
			timer = -1f;
			announcementStage = 0;
			List<string> teamkillers = new List<string>();
			bool delete;
			plugin.roundState = 0;
			plugin.roundTime = 0f;
			endTime = false;
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
			timer = 0f;
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			endTime = true;
		}

		float timer = 0f;
		int announcementStage = 0;

		public void OnFixedUpdate(FixedUpdateEvent ev)
		{
			bool isEmpty = !plugin.eventQeue.Any();
			if ((isEmpty || plugin.eventQeue[0] == "noevent") && !warheadDetonated)
			{
				if (!(timer < 0) && timer < plugin.timeToPowerOuttage)
				{
					timer += Time.deltaTime;
				}else if (timer >= plugin.timeToPowerOuttage)
				{
					timer = -1f;
					plugin.Server.Map.OverchargeLights(500, false);
					
				}
				if (timer > (plugin.timeToPowerOuttage - 180f) && announcementStage == 0)
				{
					announcementStage++;
					plugin.Server.Map.AnnounceCustomMessage("danger . power system failure detected . please evacuate the facility . power failure in t minus 3 minutes .", true, true);
					plugin.Server.Map.OverchargeLights(5, false);
				}else if ((timer > (plugin.timeToPowerOuttage - 150f) && announcementStage == 1) || (timer > (plugin.timeToPowerOuttage - 120f) && announcementStage == 2) || (timer > (plugin.timeToPowerOuttage - 90f) && announcementStage == 3) || (timer > (plugin.timeToPowerOuttage - 60f) && announcementStage == 4) || (timer > (plugin.timeToPowerOuttage - 30f) && announcementStage == 5))
				{
					announcementStage++;
					plugin.Server.Map.OverchargeLights(5, false);
				}else if ((timer > (plugin.timeToPowerOuttage - 20f) && announcementStage == 6) || (timer > (plugin.timeToPowerOuttage - 10f) && announcementStage == 7))
				{
					announcementStage++;
					plugin.Server.Map.OverchargeLights(2, false);
				}else if (timer > (plugin.timeToPowerOuttage - 5f) && announcementStage == 8)
				{
					announcementStage++;
					plugin.Server.Map.OverchargeLights(2, false);
					plugin.Server.Map.AnnounceCustomMessage("danger . power system failure .g6 . .g4 .g2 . .g1", true, true);
				}
			}

			if (plugin.roundState == 1)
			{
				plugin.roundTime += Time.deltaTime;
			}
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (plugin.roundState == 0 && plugin.welcomeMessages != "")
			{
				string[] messages = plugin.welcomeMessages.Split(';');
				foreach (string message in messages)
				{
					string[] broadcastInfo = message.Split('|');
					bool parseSucces = UInt32.TryParse(broadcastInfo[0], out uint time);
					if (parseSucces)
					{
						ev.Player.PersonalBroadcast(time, broadcastInfo[1], false);
					}
				}
			}
		}

		public void OnDoorAccess(PlayerDoorAccessEvent ev)
		{
			bool isEmpty = !plugin.eventQeue.Any();
			if (isEmpty || plugin.eventQeue[0] == "noevent")
			{
				if (ev.Door.Name == "NUKE_SURFACE" && ev.Door.Open == false)
				{
					ev.Allow = true;
				}
				else if (ev.Door.Name == "NUKE_SURFACE" && ev.Door.Open == true)
				{
					ev.Allow = false;
				}
			}
		}

		bool warheadDetonated = false;

		public void OnDetonate()
		{
			warheadDetonated = true;
		}
	}
}
