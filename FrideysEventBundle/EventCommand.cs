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
				return new string[] { "\n[ Frideys Event Bundle ]\nMain command: feb event <event> - Execute and event.\nEvents:\nchaosvsntf\npeanutpocalypse\ndclassbattle\ndclassinvasion\nttt" };
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
									plugin.Round.RoundLock = false;
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
										players[i].SetHealth(150);
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
								if (GetPlayers().Count >= 2)
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
									}/*
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
									}*/
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
												players[i].GiveItem(Smod2.API.ItemType.KEYCARD_ZONE_MANAGER);
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
							case "dclassbattle":
								if (GetPlayers().Count >= 4)
								{
									plugin.Round.RoundLock = true;
									List<Smod2.API.Door> doors = plugin.Server.Map.GetDoors();
									foreach (Smod2.API.Door door in doors)
									{
										if (-100 < door.Position.y && door.Position.y < 100)
										{
											System.Random rnd = new System.Random();
											DCBtrySpawnItems(rnd, door);
											DCBtrySpawnItems(rnd, door);
											int chanceWhatAmmo = rnd.Next(0, 5);
											switch (chanceWhatAmmo)
											{
												default:
													break;
												case 0:
													plugin.Server.Map.SpawnItem(Smod2.API.ItemType.AMMO556, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
													break;
												case 1:
													plugin.Server.Map.SpawnItem(Smod2.API.ItemType.AMMO762, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
													break;
												case 2:
													plugin.Server.Map.SpawnItem(Smod2.API.ItemType.AMMO9MM, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
													break;
											}
											if (door.Name == "SURFACE_GATE" || door.Name == "914" || door.Name == "CHECKPOINT_LCZ_A" || door.Name == "CHECKPOINT_LCZ_B")
											{
												door.Open = false;
												door.Locked = true;
											}
											else
											{
												door.Open = false;
											}
										}else if (door.Name == "SURFACE_GATE")
										{
											door.Open = false;
											door.Locked = true;
										}
									}
									foreach (Player player in GetPlayers())
									{
										player.ChangeRole(Smod2.API.RoleType.CLASSD);
										player.PersonalBroadcast(1, "<color=#00ff00>Battle Royal! Find weapons and items to fight till the last man standing! You are invincible for 180 seconds.</color>", false);
										
										player.SetGodmode(true);
									}
									List<Elevator> lifts = plugin.Server.Map.GetElevators();
									foreach (Elevator lift in lifts)
									{
										try { lift.Locked = true; }
										catch { plugin.Debug(lift.ToString() + " is not lockable."); }
									}

									plugin.time = 600;
									plugin.inbetweenTime = 180f;
									plugin.currentEvent = "dclassbattle";
									string str = "<color=#00FF00>Executing event: </color><color=#99FF99>" + args[1] + "</color>";
									return new string[] { str };
								}
								else
								{
									return new string[] { "<color=#FF0000>Error</color> : <color=#FF8383>4 or more players are required for this event.</color>" };
								}
							case "dclassinvasion":
								if (GetPlayers().Count >= 4)
								{
									List<Smod2.API.Door> doors = plugin.Server.Map.GetDoors();
									foreach (Smod2.API.Door door in doors)
									{
										if (door.Name == "ESCAPE_INNER")
										{
											door.Open = false;
											door.Locked = true;
										}
										if (door.Name == "SURFACE_GATE")
										{
											door.Open = false;
										}
									}
									plugin.Round.RoundLock = true;
									List<Elevator> lifts = plugin.Server.Map.GetElevators();
									foreach (Elevator lift in lifts)
									{
										try { lift.Locked = true; }
										catch { plugin.Debug(lift.ToString() + " is not lockable."); }
									}
									if (GetPlayers().Count < 7)
									{
										List<Player> players = GetPlayers();
										DCIspawnAllAsSpec(players);
										System.Random rnd = new System.Random();
										int ntf1 = rnd.Next(0, players.Count);
										players[ntf1].ChangeRole(Smod2.API.RoleType.NTF_COMMANDER);
										ClearPlayerInventory(players[ntf1]);
										DCIGiveItems(players[ntf1], 0);
										players[ntf1].SetHealth(5000);
										DCIspawnDClass(players);
									}
									else if (7 <= GetPlayers().Count && GetPlayers().Count < 13)
									{
										List<Player> players = GetPlayers();
										DCIspawnAllAsSpec(players);
										System.Random rnd = new System.Random();
										int ntf1 = rnd.Next(0, 7);
										int ntf2 = rnd.Next(7, 13);
										players[ntf1].ChangeRole(Smod2.API.RoleType.NTF_COMMANDER);
										players[ntf2].ChangeRole(Smod2.API.RoleType.NTF_COMMANDER);
										ClearPlayerInventory(players[ntf1]);
										ClearPlayerInventory(players[ntf2]);
										DCIGiveItems(players[ntf1], 0);
										DCIGiveItems(players[ntf2], 0);
										players[ntf1].SetHealth(5000);
										players[ntf2].SetHealth(5000);
										DCIspawnDClass(players);
									}
									else
									{
										List<Player> players = GetPlayers();
										DCIspawnAllAsSpec(players);
										System.Random rnd = new System.Random();
										int ntf1 = rnd.Next(0, 7);
										int ntf2 = rnd.Next(7, 13);
										int ntf3 = rnd.Next(13, players.Count);
										players[ntf1].ChangeRole(Smod2.API.RoleType.NTF_COMMANDER);
										players[ntf2].ChangeRole(Smod2.API.RoleType.NTF_COMMANDER);
										players[ntf3].ChangeRole(Smod2.API.RoleType.NTF_COMMANDER);
										ClearPlayerInventory(players[ntf1]);
										ClearPlayerInventory(players[ntf2]);
										ClearPlayerInventory(players[ntf3]);
										DCIGiveItems(players[ntf1], 0);
										DCIGiveItems(players[ntf2], 0);
										DCIGiveItems(players[ntf3], 0);
										players[ntf1].SetHealth(5000);
										players[ntf2].SetHealth(5000);
										players[ntf3].SetHealth(5000);
										DCIspawnDClass(players);
									}
									plugin.time = 600;
									plugin.inbetweenTime = 120f;
									plugin.currentEvent = "dclassinvasion";
									string str = "<color=#00FF00>Executing event: </color><color=#99FF99>" + args[1] + "</color>";
									return new string[] { str };
								}
								else
								{
									return new string[] { "<color=#FF0000>Error</color> : <color=#FF8383>4 or more players are required for this event.</color>" };
								}
							case "ttt":
								if (GetPlayers().Count >= 3)
								{
									plugin.Round.RoundLock = true;
									for (int i = 0; i <= 35; i++)
										foreach (Smod2.API.Item item in plugin.Server.Map.GetItems((Smod2.API.ItemType)i, true))
											item.Remove();
									List<Smod2.API.Door> doors = plugin.Server.Map.GetDoors();
									foreach (Smod2.API.Door door in doors)
									{
										if (door.Name == "914" || door.Name == "CHECKPOINT_LCZ_A" || door.Name == "CHECKPOINT_LCZ_B")
										{
											door.Open = false;
											door.Locked = true;
										}
									}
									Player murderer;
									plugin.Info("1");
									Player sherif;
									plugin.Info("2");
									List<Player> innocents = GetPlayers();
									System.Random rnd = new System.Random();
									int murderInt = rnd.Next(0, innocents.Count);
									murderer = innocents[murderInt];
									innocents.Remove(murderer);
									plugin.Info("3");
									int shrifInt = rnd.Next(0, innocents.Count);
									sherif = innocents[shrifInt];
									innocents.Remove(sherif);
									plugin.Info("4");
									plugin.tttPlayers.Add(murderer);
									plugin.tttPlayers.Add(sherif);
									plugin.Info("5");
									murderer.ChangeRole(Smod2.API.RoleType.CLASSD);
									murderer.PersonalBroadcast(10, "<color=#ff0000>You are the murderer</color>\n<color=#ff8f8f>You will recieve your weapon in 30 seconds.</color>", false);
									murderer.PersonalBroadcast(20, "<color=#ff8f8f>You must kill everyone, but beware of the sherif who can kill you.</color>", false);
									sherif.ChangeRole(Smod2.API.RoleType.CLASSD);
									sherif.PersonalBroadcast(10, "<color=#ffdd00>You are the Sherif</color>\n<color=#fff199>You will recieve your gun in 30 seconds.</color>", false);
									sherif.PersonalBroadcast(20, "<color=#fff199>You must kill the murderer, but beware if you kill the wrong person, youll die!</color>", false);
									plugin.Info("6");
									foreach (Player player in innocents)
									{
										player.ChangeRole(Smod2.API.RoleType.CLASSD);
										player.PersonalBroadcast(10, "<color=#00ff00>You are an Innocent</color>\n<color=#a3ffa3>You must not die.</color>", false);
										player.PersonalBroadcast(20, "<color=#a3ffa3>If the sherif dies, you can pick up their gun and become the new sherif.</color>", false);
									}
									plugin.time = 600;
									plugin.inbetweenTime = 30;
									plugin.currentEvent = "ttt";
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

		void DCIGiveItems(Player player, int playerClass/*1=dboi|0=ntf*/)
		{
			switch (playerClass)
			{
				default:
					player.GiveItem(Smod2.API.ItemType.GUN_MP7);
					player.GiveItem(Smod2.API.ItemType.GUN_PROJECT90);
					player.GiveItem(Smod2.API.ItemType.GUN_USP);
					player.GiveItem(Smod2.API.ItemType.E11_STANDARD_RIFLE);
					player.GiveItem(Smod2.API.ItemType.COM15);
					player.SetAmmo(AmmoType.AMMO556, 200);
					player.SetAmmo(AmmoType.AMMO762, 200);
					player.SetAmmo(AmmoType.AMMO9MM, 200);
					player.PersonalBroadcast(15, "You have 5000 HP, survive for 10 minutes.", false);
					break;
				case 1:
					player.GiveItem(Smod2.API.ItemType.COM15);
					player.SetAmmo(AmmoType.AMMO9MM, 100);
					player.PersonalBroadcast(10, "Kill the NTF!! They are VERY tough! You have 10 minutes", false);
					break;
			}
			
		}

		void DCIrandomTeleport(Player player)
		{
			player.Teleport(new Vector(-11, 1002, -43));
		}

		void DCIspawnDClass(List<Player> players)
		{
			foreach (Player player in players)
			{
				if (player.TeamRole.Role != Smod2.API.RoleType.NTF_COMMANDER)
				{
					player.ChangeRole(Smod2.API.RoleType.CLASSD, true, false);
					DCIGiveItems(player, 1);
					DCIrandomTeleport(player);
				}
			}
		}

		void DCIspawnAllAsSpec(List<Player> players)
		{
			foreach (Player player in players)
			{
				player.ChangeRole(Smod2.API.RoleType.SPECTATOR);
			}
		}

		void DCBtrySpawnItems(System.Random rnd, Smod2.API.Door door)
		{
			int chanceWhatItem = rnd.Next(0, 10);
			switch (chanceWhatItem)
			{
				default:
					break;
				case 0:
					plugin.Server.Map.SpawnItem(Smod2.API.ItemType.COM15, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
					break;
				case 1:
					plugin.Server.Map.SpawnItem(Smod2.API.ItemType.MEDKIT, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
					break;
				case 2:
					plugin.Server.Map.SpawnItem(Smod2.API.ItemType.SCP500, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
					break;
				case 3:
					plugin.Server.Map.SpawnItem(Smod2.API.ItemType.SCP207, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
					break;
				case 4:
					plugin.Server.Map.SpawnItem(Smod2.API.ItemType.P90, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
					break;
				case 5:
					plugin.Server.Map.SpawnItem(Smod2.API.ItemType.USP, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
					break;
				case 6:
					plugin.Server.Map.SpawnItem(Smod2.API.ItemType.SCP018, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
					break;
				case 7:
					plugin.Server.Map.SpawnItem(Smod2.API.ItemType.FRAG_GRENADE, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
					break;
				case 8:
					plugin.Server.Map.SpawnItem(Smod2.API.ItemType.FLASHBANG, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
					break;
				case 9:
					plugin.Server.Map.SpawnItem(Smod2.API.ItemType.MP7, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
					break;
			}
		}

		List<Player> GetPlayers()
		{
			return plugin.Server.GetPlayers();
		}
	}
}
