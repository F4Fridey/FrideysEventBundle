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
    class EventHandler : IEventHandlerFixedUpdate, IEventHandlerDoorAccess, IEventHandlerPlayerDie, IEventHandlerRoundEnd, IEventHandlerRoundRestart, IEventHandlerRoundStart, IEventHandlerPlayerJoin, IEventHandlerShoot, IEventHandlerPlayerLeave, IEventHandlerPlayerDropItem, IEventHandlerPlayerPickupItem
    {
		private readonly FrideysEventBundle plugin;

		public EventHandler(FrideysEventBundle plugin)
		{
			this.plugin = plugin;
		}

		public void OnDoorAccess(PlayerDoorAccessEvent ev)
		{
			//plugin.Info(ev.Door.Name + ev.Door.Position.ToString());
		}

		public void OnPlayerDropItem(PlayerDropItemEvent ev)
		{
			switch (plugin.currentEvent)
			{
				default:
					break;
				case "ttt":
					ev.Allow = false;
					break;
			}
		}

		public void OnPlayerPickupItem(PlayerPickupItemEvent ev)
		{
			switch (plugin.currentEvent)
			{
				default:
					break;
				case "ttt":
					plugin.tttPlayers.Add(ev.Player);
					ev.Player.SetAmmo(Smod2.API.AmmoType.AMMO9MM, 500);
					ev.Player.PersonalBroadcast(10, "<color=#ffdd00>You are the Sherif</color>\n<color=#fff199>You must kill the murderer, but beware if you kill the wrong person, youll die!</color>", false);
					break;
			}
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			float timeLeft;
			switch (plugin.currentEvent)
			{
				default:
					break;
				case "chaosvsntf":
					ev.Player.OverwatchMode = true;
					ev.Player.PersonalBroadcast(20, "And event is currently underway and you are spectating.", false);
					break;
				case "chaosvsntfENDING":
					ev.Player.OverwatchMode = true;
					ev.Player.PersonalBroadcast(20, "And event is currently underway and you are spectating.", false);
					break;
				case "peanutpocalypse":
					ev.Player.OverwatchMode = true;
					ev.Player.PersonalBroadcast(20, "And event is currently underway and you are spectating.", false);
					break;
				case "peanutpocalypseENDING":
					ev.Player.OverwatchMode = true;
					ev.Player.PersonalBroadcast(20, "And event is currently underway and you are spectating.", false);
					break;
				case "dclassbattle":
					ev.Player.OverwatchMode = true;
					ev.Player.PersonalBroadcast(20, "And event is currently underway and you are spectating.", false);
					break;
				case "dclassinvasion":
					ev.Player.OverwatchMode = true;
					timeLeft = (plugin.time - timer) / 60;
					ev.Player.PersonalBroadcast(20, "And event is currently underway and you are spectating. Event will end in " + timeLeft.ToString("0.0") + " minutes.", false);
					break;
				case "ttt":
					ev.Player.OverwatchMode = true;
					timeLeft = (plugin.time - timer) / 60;
					ev.Player.PersonalBroadcast(20, "And event is currently underway and you are spectating. Event will end in " + timeLeft.ToString("0.0") + " minutes.", false);
					break;
			}
		}

		public void OnPlayerLeave(PlayerLeaveEvent ev)
		{
			switch (plugin.currentEvent)
			{
				default:
					break;
				case "ttt":
					if (ev.Player.UserId == plugin.tttPlayers[0].UserId)
					{
						plugin.Server.Map.Broadcast(10, "<color=#00ff00>Innocents win!</color>", false);
						plugin.Round.RoundLock = false;
					}/*else if (ev.Player == plugin.tttPlayers[1])
					{
						//spawn gun again
					}*/
					break;
			}
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
				case "peanutpocalypseENDING":
					if (ev.Killer.TeamRole.Team == TeamType.CLASSD)
					{
						ev.Player.Teleport(new Vector(ev.Killer.GetPosition().x, ev.Killer.GetPosition().y + 1, ev.Killer.GetPosition().z));
						ev.Player.OverwatchMode = true;
					}
					break;
				case "dclassbattle":
					ev.Player.OverwatchMode = true;
					break;
				case "dclassinvasion":
					break;
				case "ttt":
					break;
			}
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (plugin.currentEvent == "chaosvsntf" || plugin.currentEvent == "chaosvsntfENDING" || plugin.currentEvent == "peanutpocalypse" || plugin.currentEvent == "peanutpocalypseENDING" || plugin.currentEvent == "dclassinvasion" || plugin.currentEvent == "ttt")
			{
				foreach (Player player in GetPlayers())
				{
					player.OverwatchMode = false;
				}
			}
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			plugin.tttPlayers = new List<Player>();
			plugin.currentEvent = "blocked";
			inbetweenTimerActivated = false;
			winCondition = false;
			timer = 0;
			inbetweenTimer = 0;
			secondTimer = 0;
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			plugin.currentEvent = "none";
		}

		public void OnShoot(PlayerShootEvent ev)
		{
			switch (plugin.currentEvent)
			{
				default:
					break;
				case "ttt":
					if (ev.Target != null)
					{
						if (ev.Player.UserId == plugin.tttPlayers[1].UserId && ev.Target.UserId != plugin.tttPlayers[0].UserId)
						{
							plugin.Server.Map.SpawnItem(Smod2.API.ItemType.COM15, new Vector(ev.Player.GetPosition().x, ev.Player.GetPosition().y + 1, ev.Player.GetPosition().z), new Vector(0, 0, 0));
							ev.Player.ChangeRole(Smod2.API.RoleType.SPECTATOR);
							ev.Player.OverwatchMode = true;
							plugin.tttPlayers.RemoveAt(1);
							ev.Target.ChangeRole(Smod2.API.RoleType.SPECTATOR);
							ev.Target.OverwatchMode = true;
						}
						else if (ev.Target.UserId == plugin.tttPlayers[0].UserId && ev.Player.UserId == plugin.tttPlayers[1].UserId)
						{
							ev.Target.ChangeRole(Smod2.API.RoleType.SPECTATOR);
							plugin.Server.Map.Broadcast(10, "<color=#00ff00>Innocents win!</color>", false);
							plugin.Round.RoundLock = false;
						}
						else
						{
							ev.Target.ChangeRole(Smod2.API.RoleType.SPECTATOR);
							ev.Target.OverwatchMode = true;
						}
					}
					break;
			}
		}

		public List<Player> GetPlayers()
		{
			return plugin.Server.GetPlayers();
		}

		float timer;
		float inbetweenTimer;
		float secondTimer;

		bool inbetweenTimerActivated = false;
		bool winCondition = false;

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
					inbetweenTimer += Time.deltaTime;
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
								if (door.Name == "GATE_A" || door.Name == "GATE_B" || door.Name == "NUKE_ARMORY" || door.Name == "LCZ_ARMORY" || door.Name == "914" || door.Name == "HCZ_ARMORY" || door.Name == "096" || door.Name == "106_BOTTOM" || door.Name == "106_PRIMARY" || door.Name == "106_SECONDARY" || door.Name == "079_FIRST" || door.Name == "079_SECOND" || door.Name == "049_ARMORY" || door.Name == "012")
								{
									door.Open = true;
									door.Locked = true;
								}
							}
							plugin.Server.Map.Broadcast(10, "<color=#00ff00>GATE A and B now open, get your MICROs!</color>", false);
							plugin.currentEvent = "peanutpocalypseENDING";
							break;
						case "dclassbattle":
							if (winCondition)
							{
								plugin.Round.RoundLock = false;
							}
							else
							{
								foreach (Player player in GetPlayers())
								{
									if (player.TeamRole.Team == Smod2.API.TeamType.CLASSD)
									{
										player.Teleport(new Vector(-11.5f, 1002, -20.3f));
									}
									player.PersonalBroadcast(3, "<color=#FF0000>Sudden death!</color>", false);
								}
								plugin.currentEvent = "chaosvsntfENDING";
							}
							break;
						case "dclassinvasion":
							winCondition = true;
							foreach (Player player in GetPlayers())
							{
								player.ChangeRole(Smod2.API.RoleType.TUTORIAL, true, false);
							}
							plugin.Server.Map.Broadcast(10, "<color=#0000ff>NTF win!</color>", false);
							plugin.Round.RoundLock = false;
							break;
						case "ttt":
							plugin.Server.Map.Broadcast(10, "<color=#00ff00>Innocents win!</color>", false);
							plugin.Round.RoundLock = false;
							break;
					}
				}

				if (inbetweenTimer > plugin.inbetweenTime && !inbetweenTimerActivated)
				{
					switch (plugin.currentEvent)
					{
						default:
							break;
						case "dclassbattle":
							foreach (Player player in GetPlayers())
							{
								player.SetGodmode(false);
								player.PersonalBroadcast(5, "<color=#ff0000>Invincibility Over. Last man standing wins!\nSudden death in 7 minutes, you wont be warned!</color>", false);
							}
							inbetweenTimerActivated = true;
							break;
						case "dclassinvasion":
							inbetweenTimer = 0;
							Vector vec = new Vector(182.3f, 994, -59.3f);
							Vector rot = new Vector(0, 0, 0);
							for (int i = 0; i < 5; i++)
							{
								plugin.Server.Map.SpawnItem(Smod2.API.ItemType.AMMO556, vec, rot);
								plugin.Server.Map.SpawnItem(Smod2.API.ItemType.AMMO762, vec, rot);
								plugin.Server.Map.SpawnItem(Smod2.API.ItemType.AMMO9MM, vec, rot);
							}
							plugin.Server.Map.SpawnItem(Smod2.API.ItemType.FRAG_GRENADE, vec, rot);
							plugin.Server.Map.SpawnItem(Smod2.API.ItemType.FRAG_GRENADE, vec, rot);
							plugin.Server.Map.SpawnItem(Smod2.API.ItemType.FLASHBANG, vec, rot);
							plugin.Server.Map.SpawnItem(Smod2.API.ItemType.FLASHBANG, vec, rot);
							plugin.Server.Map.Broadcast(10, "Supplies at helicopter dropoff!", false);
							break;
						case "ttt":
							try
							{
								plugin.tttPlayers[0].GiveItem(Smod2.API.ItemType.COM15);
								plugin.tttPlayers[1].GiveItem(Smod2.API.ItemType.COM15);
								plugin.tttPlayers[0].SetAmmo(Smod2.API.AmmoType.AMMO9MM, 500);
								plugin.tttPlayers[1].SetAmmo(Smod2.API.AmmoType.AMMO9MM, 500);
								inbetweenTimerActivated = true;
							}
							catch
							{
								plugin.Info("error");
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
						case "dclassbattle":
							if (!winCondition)
							{
								int dclassLeft = 0;
								foreach (Player player in GetPlayers())
								{
									if (player.TeamRole.Role == Smod2.API.RoleType.CLASSD)
									{
										dclassLeft += 1;
									}
									if (player.GetGodmode())
										player.PersonalBroadcast(1, "<color=#00ff00>Battle Royal! Find weapons and items to fight till the last man standing! You are invincible for " + (180 - inbetweenTimer).ToString("0") + " seconds.</color>", false);
								}
								if (dclassLeft < 2)
								{
									winCondition = true;
									plugin.time = 5f;
									timer = 0;
									string lastPlayerName = "no one";
									foreach (Player player in GetPlayers())
									{
										if (player.TeamRole.Role == Smod2.API.RoleType.CLASSD) { lastPlayerName = player.Name; }
									}
									plugin.Server.Map.Broadcast(10, "<color=#00ff00>Congratulations to " + lastPlayerName + ", the last man standing!</color>", false);
								}
							}
							break;
						case "dclassinvasion":
							if (!winCondition)
							{
								int ntfLeft = 0;
								foreach (Player player in GetPlayers())
								{
									if (player.TeamRole.Role == Smod2.API.RoleType.NTF_COMMANDER)
									{
										ntfLeft += 1;
									}
									if (player.TeamRole.Role == Smod2.API.RoleType.SPECTATOR && player.OverwatchMode == false)
									{
										DCIrespawnDClass(player);
									}
								}
								if (ntfLeft < 1)
								{
									winCondition = true;
									plugin.Server.Map.Broadcast(10, "<color=#00ff00>Class Ds win!</color>", false);
									plugin.Round.RoundLock = false;
								}
							}
							break;
						case "ttt":
							int innocentsLeft = 0;
							foreach (Player player in GetPlayers())
							{
								if (player.UserId != plugin.tttPlayers[0].UserId && player.TeamRole.Role == Smod2.API.RoleType.CLASSD)
								{
									innocentsLeft += 1;
								}
							}
							if (innocentsLeft < 1)
							{
								plugin.Server.Map.Broadcast(10, "<color=#ff0000>The Murderer wins!</color>", false);
								plugin.Round.RoundLock = false;
							}
							break;
					}
					secondTimer = 0;
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

		void DCIrespawnDClass(Player player)
		{
			player.ChangeRole(Smod2.API.RoleType.CLASSD, true, false);
			System.Random rnd = new System.Random();
			int place = rnd.Next(0, 2);
			switch (place)
			{
				default:
					player.Teleport(new Vector(-11, 1002, -43));
					break;
				case 1:
					player.Teleport(new Vector(-11, 1002, -43));
					break;
				case 0:
					player.Teleport(new Vector(10.6f, 989, -48.6f));
					break;
			}
			player.GiveItem(Smod2.API.ItemType.COM15);
			player.SetAmmo(AmmoType.AMMO9MM, 100);
			player.PersonalBroadcast(10, "Kill the NTF!! They are VERY tough! You have 10 minutes", false);
		}
	}
}
