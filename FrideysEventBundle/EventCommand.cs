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
			return "FEB";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			try
			{
				plugin.Debug("Please ignore this message [" + args[0] + "]");
			}
			catch
			{
				return new string[] { "\n|| <color=#04FF00>Frideys</color> <color=#029700>Event</color> <color=#04FF00>Bundle</color> ||\n<color=#7BFFF1>Main command:</color> feb event <event> <color=#7BFFF1>- Execute and event.</color>\n<color=#04FF00>Events:</color>\n<color=#A8FFA6>chaosvsntf\npeanutpocalypse</color>" };
			}
			if (plugin.currentEvent == "none")
			{
				switch (args[0])
				{
					default:
						return new string[] { "<color=#FF0000>Error</color> : <color=#FF8383>Please use a valid argument.</color>" };
					case "event":
						try
						{
							plugin.Debug("Please ignore this message [" + args[1] + "]");
						}
						catch
						{
							return new string[] { "<color=#FF0000>Error</color> : <color=#FF8383>Please specify a valid event.</color>" };
						}
						switch (args[1])
						{
							default:
								return new string[] { "<color=#FF0000>Error</color> : <color=#FF8383>Please specify a valid event.</color>" };
							case "chaosvsntf":
								if (GetPlayers().Count >= 2)
								{
									List<Elevator> lifts = plugin.Server.Map.GetElevators();
									foreach (Elevator lift in lifts)
									{
										try { lift.Locked = true; }
										catch { plugin.Debug(lift.ToString() + " is not lockable."); }
									}
									List<Smod2.API.Door> doors = plugin.Server.Map.GetDoors();
									foreach (Smod2.API.Door door in doors)
									{
										if (door.Name == "SURFACE_GATE")
										{
											door.Open = false;
											break;
										}
									}
									List<Player> players = GetPlayers();
									int half = players.Count / 2;
									for (int i = 0; i < half; i++)
									{
										players[i].ChangeRole(Smod2.API.RoleType.CHAOS_INSURGENCY);
										ClearPlayerInventory(players[i]);
										ChaosVNTFGiveItems(players[i]);
										players[i].PersonalBroadcast(7, "<color=#FF0000>Objective: </color><color=#FF7C00>Kill all NTF!</color>", false);
										players[i].PersonalBroadcast(7, "<color=#B00000>Deathmatch will occur in 5 minutes, you wont be warned!</color>", false);
									}
									for (int i = half; i < players.Count; i++)
									{
										players[i].ChangeRole(Smod2.API.RoleType.NTF_COMMANDER);
										ClearPlayerInventory(players[i]);
										ChaosVNTFGiveItems(players[i]);
										players[i].PersonalBroadcast(7, "<color=#FF0000>Objective: </color><color=#FF7C00>Kill all Chaos Insurgents!</color>", false);
										players[i].PersonalBroadcast(7, "<color=#B00000>Sudden death will occur in 5 minutes, you wont be warned!</color>", false);
									}
									plugin.time = 300f;
									plugin.Round.RoundLock = false;
									plugin.currentEvent = "chaosvsntf";
									string str = "<color=#00FF00>Executing event: </color><color=#99FF99>" + args[1] + "</color>";
									return new string[] { str };
								}
								else
								{
									return new string[] { "<color=#FF0000>Error</color> : <color=#FF8383>2 or more players are required for this event.</color>" };
								}
							case "peanutpocalypse":
								if (GetPlayers().Count >= 3)
								{
									List<Elevator> lifts = plugin.Server.Map.GetElevators();
									foreach (Elevator lift in lifts)
									{
										List<Vector> vecs = lift.GetPositions();
										foreach (Vector vec in vecs)
										{
											if (vec.y > 990)
											{
												lift.Locked = true;
												foreach (Vector vec2 in vecs)
												{
													if (vec2.y < -900)
													{
														for (int i = 0; i < 10; i++)
														{
															plugin.Server.Map.SpawnItem(Smod2.API.ItemType.MICROHID, new Vector(vec2.x, vec2.y + 3, vec2.z), new Vector(0, 0, 0));
														}
														break;
													}
												}
												break;
											}
										}
									}
									List<Smod2.API.Door> doors = plugin.Server.Map.GetDoors();
									foreach (Smod2.API.Door door in doors)
									{
										switch (door.Name)
										{
											default:
												break;
											case "GATE_A":
												door.Open = true;
												door.Locked = true;
												break;
											case "GATE_B":
												door.Open = true;
												door.Locked = true;
												break;
										}
									}
									List<Player> players = GetPlayers();
									for (int i = 0; i < players.Count; i++)
									{
										switch (i)
										{
											case 1:
												players[i].ChangeRole(Smod2.API.RoleType.SCP_173);
												players[i].PersonalBroadcast(7, "<color=#FF0000>Kill D-Class to duplicate yourself!</color>", false);
												break;
											default:
												players[i].ChangeRole(Smod2.API.RoleType.CLASSD);
												players[i].GiveItem(Smod2.API.ItemType.KEYCARD_SCIENTIST);
												players[i].PersonalBroadcast(10, "<color=#FF8383>You have a keycard, so go to the </color><color=#FF0000>Opened Gates </color><color=#FF8383>to get Micro-HIDs and kill the peanuts! </color>", false);
												players[i].PersonalBroadcast(5, "<color=#FF0000>Dont die or you'll become one!</color>", false);
												break;
										}
									}
									plugin.time = 180f;
									plugin.Round.RoundLock = false;
									plugin.currentEvent = "peanutpocalypse";
									string str = "<color=#00FF00>Executing event: </color><color=#99FF99>" + args[1] + "</color>";
									return new string[] { str };
								}
								else
								{
									return new string[] { "<color=#FF0000>Error</color> : <color=#FF8383>3 or more players are required for this event.</color>" };
								}
						}
				}
			}
			else
			{
				return new string[] { "<color=#FF0000>Error</color> : <color=#FF8383>An event is currently running or the round hasnt started.</color>" };
			}
		}

		private void ClearPlayerInventory(Player player)
		{
			foreach (Smod2.API.Item item in player.GetInventory())
				item.Remove();
		}

		private void ChaosVNTFGiveItems(Player player)
		{
			player.GiveItem(Smod2.API.ItemType.GUN_MP7);
			player.GiveItem(Smod2.API.ItemType.GUN_PROJECT90);
			player.GiveItem(Smod2.API.ItemType.GUN_USP);
			player.GiveItem(Smod2.API.ItemType.MEDKIT);
			player.GiveItem(Smod2.API.ItemType.FRAG_GRENADE);
			player.GiveItem(Smod2.API.ItemType.FLASHBANG);
			player.SetAmmo(AmmoType.AMMO556, 200);
			player.SetAmmo(AmmoType.AMMO762, 200);
			player.SetAmmo(AmmoType.AMMO9MM, 200);
		}

		List<Player> GetPlayers()
		{
			return plugin.Server.GetPlayers();
		}
	}
}
