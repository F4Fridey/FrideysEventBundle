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
				return new string[] { "\n[ Frideys Event Bundle ]\nMain commands:\nevent add <event> - Add an event to the qeue.\nevent qeue view - Prints the current event qeue.\nevent qeue clear - Clears the current event qeue.\nEvents:\nnoevent\nchaosvsntf\npeanutpocalypse\ndclassbattle\ndclassinvasion\nttt" };
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
					switch (args[1])
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
					switch (args[1])
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
